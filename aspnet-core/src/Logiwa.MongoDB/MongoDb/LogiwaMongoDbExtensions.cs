#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Humanizer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MongoDB;

#endregion

namespace Logiwa.MongoDB
{
    public static class LogiwaMongoDbExtensions
    {
         public static IAggregateFluent<TEntity> Include<TEntity, TLookupEntity>(this IAggregateFluent<TEntity> query,
            IAbpMongoDbContext dbContext, string localFieldName = null, string entityName = null)
            where TEntity : class, IEntity
        {
            if (string.IsNullOrEmpty(entityName))
            {
                entityName = typeof(TLookupEntity).Name;
            }

            if (string.IsNullOrEmpty(localFieldName))
            {
                localFieldName = entityName + "Id";
            }

            var queryBson = query.As<BsonDocument>();

            // Perform a lookup. If it exists then it will be included in an array called entityName
            queryBson = queryBson.Lookup(dbContext.Collection<TLookupEntity>().CollectionNamespace.CollectionName,
                localFieldName, "_id", entityName);

            // convert the single value in the array to a property instead (use the options to handle the scenario that the foreign entity does not exist)
            queryBson = queryBson.Unwind(new StringFieldDefinition<BsonDocument>(entityName),
                new AggregateUnwindOptions<BsonDocument>() {PreserveNullAndEmptyArrays = true});

            return queryBson.As<TEntity>();
        }
      /*  public static IAggregateFluent<TEntity> IncludeReverse<TEntity, TLookupEntity>(this IAggregateFluent<TEntity> query,
            IAbpMongoDbContext dbContext, string foreignFieldName = null, string localEntityName = null, string foreignEntityName = null)
            where TEntity : class, IEntity
        {
            if (string.IsNullOrEmpty(localEntityName))
            {
                localEntityName = typeof(TEntity).Name;
            }
            if (string.IsNullOrEmpty(foreignEntityName))
            {
                foreignEntityName = typeof(TLookupEntity).Name;
            }

            if (string.IsNullOrEmpty(foreignFieldName))
            {
                foreignFieldName = localEntityName + "Id";
            }

            var queryBson = query.As<BsonDocument>();

            // Perform a lookup. If it exists then it will be included in an array called entityName
            queryBson = queryBson.Lookup(dbContext.Collection<TLookupEntity>().CollectionNamespace.CollectionName,
                 "_id",foreignFieldName, foreignEntityName.Pluralize());
            
            return queryBson.As<TEntity>();
        } */

      /*  public static IAggregateFluent<TEntity> IncludeMany<TEntity, TLookupEntity>(
            this IAggregateFluent<TEntity> query, IAbpMongoDbContext dbContext, string localFieldName = null,
            string entityName = null)
            where TEntity : class, IEntity
        {
            if (string.IsNullOrEmpty(entityName))
            {
                entityName = typeof(TLookupEntity).Name;
            }

            if (string.IsNullOrEmpty(localFieldName))
            {
                localFieldName = entityName + "Ids";
            }

            var queryBson = query.As<BsonDocument>();

            // Perform a lookup. If it exists then it will be included in an array called entityName
            queryBson = queryBson.Lookup(dbContext.Collection<TLookupEntity>().CollectionNamespace.CollectionName,
                localFieldName, "_id", entityName.Pluralize());

            return queryBson.As<TEntity>();
        } */

        public static TEntity GetIncluded<TEntity>(this IHasExtraProperties entity, string fieldName = null)
            where TEntity : class, IEntity
        {
            var properties = entity.ExtraProperties;

            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = typeof(TEntity).Name;
            }

            if (properties.TryGetValue(fieldName, out var entityObject) &&
                entityObject is Dictionary<string, object> dictionary)
            {
                var result = BsonSerializer.Deserialize<TEntity>(dictionary.ToBsonDocument());
                foreach (var dictEntry in dictionary)
                {
                    //TODO Fix this hack. (Reflections may be useful.)
                  /*  if (dictEntry.Key == "Files")
                    {
                        var list = new List<File>();
                        foreach (Dictionary<string, object> fileEntry in (List<object>) dictEntry.Value)
                        {
                            list.Add(new File(fileEntry));
                        }

                        result.GetType().GetProperty("Files")?.SetValue(result, list, null);
                    } */
                }

                return result;
            }

            return null;
        }

     /*   public static List<TEntity> GetIncludedMany<TEntity>(this IHasExtraProperties entity, string fieldName = null)
            where TEntity : class, IEntity
        {
            var properties = entity.ExtraProperties;

            if (string.IsNullOrEmpty(fieldName))
            {
                fieldName = typeof(TEntity).Name.Pluralize();
            }

            if (properties.TryGetValue(fieldName, out var entityObject) && entityObject is List<object> list)
            {
                var dictionaryList = list.OfType<Dictionary<string, object>>();
                var result = new List<TEntity>();
                foreach (var dictionary in dictionaryList)
                {
                    result.Add(BsonSerializer.Deserialize<TEntity>(dictionary.ToBsonDocument()));
                }

                return result;
            }

            return null;
        } */

        /// <summary>
        /// Support sort of the format "myColumn asc" or "myOtherColumn desc".
        /// The first letter of any sort field is automatically capitalized.
        /// Can also sort on multiple columns "myColumn asd, myOtherColumn desc".
        /// Can also sort by sub properties "myColumn.something desc"
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="query"></param>
        /// <param name="sorting"></param>
        /// <param name="defaultSorting"></param>
        /// <returns></returns>
      /*  public static IAggregateFluent<TEntity> Sort<TEntity>(this IAggregateFluent<TEntity> query, string sorting,
            string defaultSorting)
        {
            sorting = string.IsNullOrWhiteSpace(sorting) ? defaultSorting : sorting;

            if (string.IsNullOrEmpty(sorting))
            {
                return query;
            }

            var sortParts = sorting.Split(',');

            var sortDefinitions = new List<SortDefinition<TEntity>>();

            // check if a sort definition is sorting using a property of the top level entity
            var topLevelEntitySortPrefix = typeof(TEntity).Name + ".";

            foreach (var sortPart in sortParts)
            {
                var parts = sortPart.Split(" ");

                if (parts.Length == 0)
                {
                    continue;
                }

                //string fieldName = CorrectCase(parts[0]); //TODO Fix.
                var fieldName = parts[0];

                if (fieldName.StartsWith(topLevelEntitySortPrefix))
                {
                    fieldName = fieldName.Substring(topLevelEntitySortPrefix.Length);
                }

                if (parts.Length == 1)
                {
                    sortDefinitions.Add(Builders<TEntity>.Sort.Ascending(fieldName));
                }

                if (parts.Length > 1)
                {
                    sortDefinitions.Add(PartIsDescending(parts[1])
                        ? Builders<TEntity>.Sort.Descending(fieldName)
                        : Builders<TEntity>.Sort.Ascending(fieldName));
                }
            }

            if (sortDefinitions.Any())
            {
                query = query.Sort(Builders<TEntity>.Sort.Combine(sortDefinitions));
            }

            query.Options.Collation =
                new Collation(CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                    strength: CollationStrength.Secondary,numericOrdering:true);
            return query;
        } */

     /*   private static bool PartIsDescending(string part)
        {
            if (string.IsNullOrEmpty(part))
            {
                return false;
            }

            return part.ToLower().Contains("des");
        } */

     /*   public static string CorrectCase(string input)
        {
            var parts = input.Split('.');

            return string.Join(".", parts.Select(FirstCharToUpper));
        } */

      /*  private static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return input.First().ToString().ToUpper() + input.Substring(1);
        } */
    }
}