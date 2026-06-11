using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonConverter(typeof(FormAddendumConverter))]
    public class FormAddendum
    {
        [JsonIgnore]
        public bool IsEmpty => Acronyms.IsEmpty() && Formulas.IsEmpty() && Figures.IsEmpty();

        public List<FormAddendumItem> Acronyms { get; private set; } = new List<FormAddendumItem>();
        public List<FormAddendumItem> Formulas { get; private set; } = new List<FormAddendumItem>();
        public List<FormAddendumItem> Figures { get; private set; } = new List<FormAddendumItem>();
        public List<FormAddendumItem> Obsolete { get; private set; } = new List<FormAddendumItem>();

        public bool ShouldSerializeAcronyms() => Acronyms.IsNotEmpty();
        public bool ShouldSerializeFormulas() => Formulas.IsNotEmpty();
        public bool ShouldSerializeFigures() => Figures.IsNotEmpty();
        public bool ShouldSerializeObsolete() => Obsolete.IsNotEmpty();

        public IEnumerable<FormAddendumItem> EnumerateAllItems() =>
            Acronyms.Concat(Formulas).Concat(Figures);

        public FormAddendum Clone() => new FormAddendum
        {
            Acronyms = Acronyms.Select(x => x.Clone()).ToList(),
            Formulas = Formulas.Select(x => x.Clone()).ToList(),
            Figures = Figures.Select(x => x.Clone()).ToList(),
            Obsolete = Obsolete.Select(x => x.Clone()).ToList(),
        };

        #region JsonConverter

        private class FormAddendumConverter : JsonConverter<FormAddendum>
        {
            public override bool CanWrite => false;

            public override FormAddendum ReadJson(JsonReader reader, Type objectType, FormAddendum existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                if (reader.TokenType == JsonToken.StartArray)
                {
                    var array = JArray.Load(reader);
                    return new FormAddendum
                    {
                        Obsolete = array.Select(x => new FormAddendumItem { Asset = x.Value<int>(), Version = -1 }).ToList()
                    };
                }
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    var obj = JObject.Load(reader);
                    var result = new FormAddendum();

                    using (var sr = obj.CreateReader())
                        serializer.Populate(sr, result);

                    return result;
                }

                throw new JsonSerializationException($"Unexpected token: {reader.TokenType}");
            }

            public override void WriteJson(JsonWriter writer, FormAddendum value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
