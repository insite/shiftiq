using System;
using System.Net;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn), JsonConverter(typeof(InternalConverter))]
    public abstract class BaseLinkedData : BaseObject
    {
        #region Classes (JsonConverter)

        protected class InternalConverter : JsonConverter<BaseLinkedData>
        {
            public override BaseLinkedData ReadJson(JsonReader reader, Type type, BaseLinkedData value, bool hasExistingValue, JsonSerializer serializer)
            {
                throw ApplicationError.Create("Implement custom converter");
            }

            public override void WriteJson(JsonWriter writer, BaseLinkedData value, JsonSerializer serializer)
            {
                throw ApplicationError.Create("Implement custom converter");
            }
        }

        protected abstract class BaseLinkedDataConverter<T> : JsonConverter<T> where T : BaseLinkedData
        {
            public override bool CanRead => _canRead || !(_canRead = true);

            public override bool CanWrite => _canWrite || !(_canWrite = true);

            private bool _canRead = true;
            private bool _canWrite = true;

            protected abstract T CreateInstance(string id);

            public override T ReadJson(JsonReader reader, Type type, T value, bool hasExistingValue, JsonSerializer serializer)
            {
                T instance;

                if (reader.TokenType == JsonToken.String)
                {
                    instance = CreateInstance(reader.Value?.ToString());
                    instance.IsLoaded = false;
                    instance.IsParsed = true;
                }
                else if (reader.TokenType == JsonToken.StartObject)
                {
                    _canRead = false;

                    instance = serializer.Deserialize<T>(reader);

                    _canRead = true;

                    instance.IsLoaded = true;
                }
                else
                {
                    throw ApplicationError.Create("Unexpected object type");
                }

                return instance;
            }

            public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            {
                if (value.IsParsed && !value.IsLoaded)
                    writer.WriteValue(value.Id.ToString());
                else
                {
                    _canWrite = false;
                    serializer.Serialize(writer, value);
                    _canWrite = true;
                }
            }
        }

        #endregion

        #region Constants

        protected const string DefaultContext = "https://w3id.org/openbadges/v2";

        #endregion

        #region Properties

        [JsonProperty("@context", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Context
        {
            get => GetValue<string>();
            protected set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Type
        {
            get => GetValue<string>();
            protected set => SetValue(value.NullIfWhiteSpace());
        }

        [JsonProperty("id", Required = Required.Always)]
        public Uri Id
        {
            get => GetValue<Uri>();
            private set => SetValue(value);
        }

        public bool IsLoaded { get; protected set; }

        #endregion

        #region Fields

        private string _hostedData;

        #endregion

        #region Construction

        protected BaseLinkedData()
        {

        }

        protected BaseLinkedData(Uri id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        #endregion

        #region Methods (load hosted)

        protected string GetHostedData()
        {
            if (_hostedData == null)
            {
                try
                {
                    using (var webClient = new WebClient())
                        _hostedData = webClient.DownloadString(Id).EmptyIfNull();
                }
                catch (Exception ex)
                {
                    throw ApplicationError.Create(ex, "Unable to read the hosted data: " + ex.Message);
                }
            }

            return _hostedData;
        }

        public void Load()
        {
            if (!IsLoaded)
            {
                var data = GetHostedData();

                IsParsed = false;

                JsonConvert.PopulateObject(data, this);

                IsLoaded = true;
            }

            LoadLinkedObjects();
        }

        protected abstract void LoadLinkedObjects();

        #endregion

        #region Methods (verify)

        public virtual void Verify()
        {
            if (IsParsed && !IsLoaded)
                Load();

            VerifyObject();
        }

        #endregion

        #region Methods ((de)serialize)

        private bool ShouldSerializeContext() => Context.IsNotEmpty();

        private bool ShouldSerializeType() => Type.IsNotEmpty();

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            if (Id == null)
                throw CreateSerializationRequiredFieldError(nameof(Id));
        }

        #endregion

        #region Methods (helpers)

        public static Uri GetUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                var isSchemaValid = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
                if (isSchemaValid)
                    return uri;
            }

            return null;
        }

        #endregion
    }
}