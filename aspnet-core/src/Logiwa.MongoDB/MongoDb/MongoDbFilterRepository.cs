#region

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.Users;

#endregion

namespace Logiwa.MongoDB
{
    public class MongoDbFilterRepository<TMongoDbContext, TEntity, TKey>
        : MongoDbRepository<TMongoDbContext, TEntity, TKey>
        where TMongoDbContext : IAbpMongoDbContext
        where TEntity : class, IEntity<TKey>
    {
        
        public MongoDbFilterRepository(IMongoDbContextProvider<TMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        
         protected IAggregateFluent<TEntity> ApplyFilter(
            IAggregateFluent<TEntity> query, Dictionary<string, Type> fields,string filters)
        {
            if (filters != null)
            {
                var filtersObject = JsonSerializer.Deserialize<Dictionary<string, object>>(filters);
                if (filtersObject.ContainsKey("global"))
                {
                    var globalFilter =JsonSerializer.Deserialize<Dictionary<string, string>>(filtersObject["global"].ToString()); 
               
                 var filtersToApply = new List<FilterDefinition<TEntity>>();
                    foreach (var field in fields)
                    {
                        filtersToApply.Add(Builders<TEntity>.Filter.Regex("Children."+field.Key,
                            BsonRegularExpression.Create(new Regex(
                                ".*" + Regex.Escape(globalFilter?["value"] ?? string.Empty).Replace("i", "[ıiIİ]") + ".*", RegexOptions.IgnoreCase))));
                        filtersToApply.Add(Builders<TEntity>.Filter.Regex(field.Key,
                            BsonRegularExpression.Create(new Regex(
                                ".*" + Regex.Escape(globalFilter?["value"] ?? string.Empty).Replace("i", "[ıiIİ]") + ".*", RegexOptions.IgnoreCase))));
                    }
                    query = query.Match(Builders<TEntity>.Filter.Or(filtersToApply));
                    filtersObject.Remove("global");
                }
                foreach (var key in filtersObject.Keys.Where(key => filtersObject[key] is JObject))
                {
                    if (!((JObject)filtersObject[key]).ContainsKey("operator"))
                    {
                        ((JObject)filtersObject[key]).Add("operator", "and");
                    }
                    filtersObject[key] = new JArray { filtersObject[key] };
                }
                filters = JsonSerializer.Serialize(filtersObject);
                var values = JsonSerializer.Deserialize<Dictionary<string, List<Dictionary<string, string>>>>(filters);
                foreach (var (key, value) in fields)
                {
                    if (values.ContainsKey(key) && (key=="ParentId" || key=="Agreements_.id" || values[key][0]["value"] != null ))
                    {
                        query = ApplyFilterForField(query, key, value, values[key]);
                    }
                }
            }
            return query;

        }
        protected virtual IAggregateFluent<TEntity> ApplyFilterForField(
            IAggregateFluent<TEntity> query,
            string filterField,
            Type filterFieldType,
            List<Dictionary<string, string>> filterItems)
        {
            var filters = new List<FilterDefinition<TEntity>>();

            foreach (var filterItem in filterItems)
            {
                if (filterItem["matchMode"].Equals("equals") || filterItem["matchMode"].Equals("is") ||
                    filterItem["matchMode"].Equals("dateIs"))
                {
                    if (filterFieldType == typeof(string))
                    {
                        filters.Add(Builders<TEntity>.Filter.Regex(filterField,
                            new BsonRegularExpression(new Regex("^" + Regex.Escape(filterItem["value"]) + "$",
                                RegexOptions.IgnoreCase))));
                    }
                    else if (filterFieldType == typeof(Guid))
                    {
                        filters.Add(filterItem["value"] == null
                            ? Builders<TEntity>.Filter.Eq(filterField, BsonNull.Value)
                            : Builders<TEntity>.Filter.Eq(filterField, Guid.Parse(filterItem["value"])));
                    }
                    else if (filterFieldType == typeof(bool))
                    {
                        if (bool.TryParse(filterItem["value"], out var value))
                        {
                            filters.Add(Builders<TEntity>.Filter.Eq(filterField, value));
                        }
                    }

                    else if (filterFieldType == typeof(short))
                    {
                        if (short.TryParse(filterItem["value"], out var value))
                        {
                            filters.Add(Builders<TEntity>.Filter.Eq(filterField, value));
                        }
                    }
                    else
                    {
                        filters.Add(Builders<TEntity>.Filter.Eq(filterField, filterItem["value"]));
                    }
                }

                if (filterItem["matchMode"].Equals("notEquals") || filterItem["matchMode"].Equals("isNot") ||
                    filterItem["matchMode"].Equals("dateIsNot"))
                {
                    if (filterFieldType == typeof(string))
                    {
                        filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Regex(filterField,
                            new BsonRegularExpression(new Regex("^" + Regex.Escape(filterItem["value"]) + "$",
                                RegexOptions.IgnoreCase)))));
                    }
                    else if (filterFieldType == typeof(Guid))
                    {
                        filters.Add(Builders<TEntity>.Filter.Ne(filterField, Guid.Parse(filterItem["value"])));
                    }
                    else if (filterFieldType == typeof(bool))
                    {
                        if (bool.TryParse(filterItem["value"], out var value))
                        {
                            filters.Add(Builders<TEntity>.Filter.Ne(filterField, value));
                        }
                    }
                    else if (filterFieldType == typeof(short))
                    {
                        if (short.TryParse(filterItem["value"], out var value))
                        {
                            filters.Add(Builders<TEntity>.Filter.Ne(filterField, value));
                        }
                    }
                    else
                    {
                        filters.Add(Builders<TEntity>.Filter.Ne(filterField, filterItem["value"]));
                    }
                }

                if (filterItem["matchMode"].Equals("startsWith"))
                {
                    filters.Add(Builders<TEntity>.Filter.Regex(filterField,
                        new BsonRegularExpression(new Regex("^" + Regex.Escape(filterItem["value"]) + ".*",
                            RegexOptions.IgnoreCase))));
                }

                if (filterItem["matchMode"].Equals("endsWith"))
                {
                    filters.Add(Builders<TEntity>.Filter.Regex(filterField,
                        new BsonRegularExpression(new Regex(".*" + Regex.Escape(filterItem["value"]) + "$",
                            RegexOptions.IgnoreCase))));
                }

                if (filterItem["matchMode"].Equals("contains"))
                {
                    if (filterFieldType == typeof(string))
                    {
                        filters.Add(Builders<TEntity>.Filter.Regex(filterField,
                            new BsonRegularExpression(new Regex(".*" + Regex.Escape(filterItem["value"]) + ".*",
                                RegexOptions.IgnoreCase))));
                    }
                    else
                    {
                        filters.Add(Builders<TEntity>.Filter.In(filterField, filterItem["value"]));
                    }
                }

                if (filterItem["matchMode"].Equals("notContains"))
                {
                    if (filterFieldType == typeof(string))
                    {
                        filters.Add(Builders<TEntity>.Filter.Not(Builders<TEntity>.Filter.Regex(filterField,
                            new BsonRegularExpression(new Regex(".*" + Regex.Escape(filterItem["value"]) + ".*",
                                RegexOptions.IgnoreCase)))));
                    }
                    else
                    {
                        filters.Add(Builders<TEntity>.Filter.Nin(filterField, filterItem["value"]));
                    }
                }

                if (filterItem["matchMode"].Equals("lt") || filterItem["matchMode"].Equals("before") ||
                    filterItem["matchMode"].Equals("dateBefore"))
                {
                    filters.Add(Builders<TEntity>.Filter.Lt(filterField, filterItem["value"]));
                }

                if (filterItem["matchMode"].Equals("lte"))
                {
                    filters.Add(Builders<TEntity>.Filter.Lte(filterField, filterItem["value"]));
                }

                if (filterItem["matchMode"].Equals("gt") || filterItem["matchMode"].Equals("after") ||
                    filterItem["matchMode"].Equals("dateAfter"))
                {
                    filters.Add(Builders<TEntity>.Filter.Gt(filterField, filterItem["value"]));
                }

                if (filterItem["matchMode"].Equals("gte"))
                {
                    filters.Add(Builders<TEntity>.Filter.Gte(filterField, filterItem["value"]));
                }
            }

            return filterItems[0]["operator"] is "or"
                ? query.Match(Builders<TEntity>.Filter.Or(filters))
                : filterItems[0]["operator"] is "and"
                    ? query.Match(Builders<TEntity>.Filter.And(filters))
                    : query;
        }
    }
        
       
    
    }
