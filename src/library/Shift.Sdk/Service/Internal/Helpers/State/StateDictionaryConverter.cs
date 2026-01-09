using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain
{
    public abstract class StateDictionaryConverter<TKey> : JsonConverter where TKey : struct, Enum
    {
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(Dictionary<TKey, object>).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.StartObject);

            var dateParseHandling = reader.DateParseHandling;
            var floatParseHandling = reader.FloatParseHandling;

            reader.DateParseHandling = DateParseHandling.None;
            reader.FloatParseHandling = FloatParseHandling.Decimal;

            var result = new Dictionary<TKey, object>();
            var mapping = GetMappingDictionary();

            while (true)
            {
                ReadReader();

                if (reader.TokenType == JsonToken.EndObject)
                    break;

                if (reader.TokenType != JsonToken.PropertyName)
                    throw WrongTokenTypeException(reader.TokenType, JsonToken.PropertyName);

                var name = (string)reader.Value;
                var key = name.ToEnum<TKey>();

                ReadReader();

                if (reader.TokenType == JsonToken.Null)
                    continue;

                var value = mapping[key](reader);

                result.Add(key, value);
            }

            reader.DateParseHandling = dateParseHandling;
            reader.FloatParseHandling = floatParseHandling;

            return result;

            void ReadReader()
            {
                if (!reader.Read())
                    throw ApplicationError.Create("Unexpected end of object");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static ApplicationError WrongTokenTypeException(JsonToken actualType, JsonToken expectedType)
        {
            throw ApplicationError.Create(
                "Unexpected token type: get {0} while expected {1}",
                actualType.GetName(),
                expectedType.GetName());
        }

        #region Methods (mapping)

        protected abstract IReadOnlyDictionary<TKey, Func<JsonReader, object>> GetMappingDictionary();

        protected static IReadOnlyDictionary<TKey, Func<JsonReader, object>> CreateMappingDictionary(IEnumerable<KeyValuePair<TKey, IStateFieldMeta>> fields)
        {
            var result = new Dictionary<TKey, Func<JsonReader, object>>();

            foreach (var field in fields)
            {
                Func<JsonReader, object> readFn;

                var fieldType = field.Value.FieldType;
                if (fieldType == StateFieldType.Text)
                    readFn = ReadText;
                else if (fieldType == StateFieldType.DateOffset)
                    readFn = ReadDateOffset;
                else if (fieldType == StateFieldType.Date)
                    readFn = ReadDate;
                else if (fieldType == StateFieldType.Bool)
                    readFn = ReadBool;
                else if (fieldType == StateFieldType.Int)
                    readFn = ReadInt;
                else if (fieldType == StateFieldType.Guid)
                    readFn = ReadGuid;
                else if (fieldType == StateFieldType.Decimal)
                    readFn = ReadDecimal;
                else
                    throw new ApplicationError("Unexpected state field type: " + fieldType.GetName());

                result.Add(field.Key, readFn);
            }

            return new ReadOnlyDictionary<TKey, Func<JsonReader, object>>(result);
        }

        private static object ReadText(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.String)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.String);

            return reader.Value;
        }

        private static object ReadDateOffset(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.String)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.String);

            return DateTimeOffset.Parse((string)reader.Value);
        }

        private static object ReadDate(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.String)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.String);

            return DateTime.Parse((string)reader.Value);
        }

        private static object ReadBool(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.Boolean)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.Boolean);

            return reader.Value;
        }

        private static object ReadInt(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.Integer)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.Integer);

            return (int)(long)reader.Value;
        }

        private static object ReadGuid(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.String)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.Date);

            return Guid.Parse((string)reader.Value);
        }

        private static object ReadDecimal(JsonReader reader)
        {
            if (reader.TokenType != JsonToken.Float)
                throw WrongTokenTypeException(reader.TokenType, JsonToken.Float);

            return (decimal)reader.Value;
        }

        #endregion
    }
}
