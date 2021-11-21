using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;


namespace Logiwa.Codes
{
    [BsonSerializer(typeof(CodeSerializer))]
    [JsonConverter(typeof(CodeConverter))]
    public class Code 
    {
        public Code(string code)
        {
            var split = code.Split('-');
            var prefix = split[0];
            var parts = split[1].Split('.').Select(int.Parse).ToList();
            Prefix = prefix;
            Parts = parts;
        }

        public Code(string prefix, List<int> parts)
        {
            Prefix = prefix;
            Parts = parts;
        }

        public string Prefix { get; set; }
        public List<int> Parts { get; set; }
        public int Length => Parts.Count;

        public override string ToString()
        {
            return Prefix + "-" + Parts.JoinAsString(".");
        }
    }

    public class CodeSerializer : IBsonSerializer
    {
        public Type ValueType { get; } = typeof(Code);

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var type = context.Reader.GetCurrentBsonType();
            switch (type)
            {
                case BsonType.String:
                    return new Code(context.Reader.ReadString());
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return default(Code);
                default:
                    throw new NotSupportedException($"Cannot convert a {type} to a {nameof(Code)}.");
            }
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
            }
            else
            {
                context.Writer.WriteString(value.ToString());
            }
        }
    }
    
    public class CodeConverter : Newtonsoft.Json.JsonConverter<Code>
    {      
        public override void WriteJson(JsonWriter writer, Code value, JsonSerializer serializer)
        {
            writer.WriteValue(value == null ? string.Empty : value.ToString());
        }
    
        [CanBeNull]
        public override Code ReadJson(JsonReader reader, Type objectType, Code existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            return reader.Value!=null ? new Code((string)reader.Value): default;
        }
    }

    public static class CodeExtensions
    {
        public static Code Clone(this Code source)
        {
            return new Code(source.ToString());
        }

        public static Code GetRelativeCode(this Code code, Code parentCode)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode == null)
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            code.Parts.RemoveRange(0, parentCode.Length);
            return code;
        }

        public static Code Append(this Code parentCode, Code childCode)
        {
            if (childCode == null)
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode == null)
            {
                return childCode;
            }

            parentCode.Parts.AddRange(childCode.Parts);
            return parentCode;
        }

        public static Code Append(this Code parentCode, int number)
        {
            var childCode = new Code(parentCode.Prefix, new List<int> {number});
            return parentCode.Append(childCode);
        }


        public static Code Add(this Code code, int number)
        {
            code.Parts[^1] += number;
            return code.Clone();
        }

        public static long GetNumber(this Code code)
        {
            return Convert.ToInt64(code.Parts.JoinAsString(string.Empty));
        }
    }
}