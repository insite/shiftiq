using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TUserSettingStore
    {
        public static void Delete(Guid organization, Guid user, string settingName, string valueType)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.TUserSettings.SingleOrDefault(x =>
                    x.OrganizationIdentifier == organization &&
                    x.UserIdentifier == user &&
                    x.Name == settingName &&
                    x.ValueType == valueType);
                if (entity == null)
                    return;

                db.TUserSettings.Remove(entity);

                db.SaveChanges();
            }
        }

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
        private static JSchemaGenerator _schemaGenerator = new JSchemaGenerator();
        private static ConcurrentDictionary<string, JSchema> _schemaStorage = new ConcurrentDictionary<string, JSchema>();

        public static T GetValue<T>(Guid organization, Guid user, string name, string type, bool validate)
        {
            string json = null;

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
            {
                using (var db = new InternalDbContext())
                {
                    json = db.TUserSettings
                        .Where(x => x.OrganizationIdentifier == organization && x.UserIdentifier == user && x.Name == name && x.ValueType == type)
                        .Select(x => x.ValueJson)
                        .SingleOrDefault();
                }
            }

            if (json == null || validate && !IsValid<T>(json))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
            }
            catch (JsonSerializationException jsex)
            {
                if (jsex.Message.StartsWith("Error resolving type specified in JSON"))
                    return default;

                throw;
            }
        }

        private static bool IsValid<T>(string json)
        {
            var type = typeof(T);
            var schema = _schemaStorage.GetOrAdd(type.FullName, x => _schemaGenerator.Generate(type));

            try
            {
                var jsonObject = JObject.Parse(json);

                jsonObject.IsValid(schema, out IList<string> errors);

                if (errors.Count > 0)
                    return false;
            }
            catch (JsonException)
            {
                return false;
            }

            return true;
        }

        public static void SetValue<T>(Guid organization, Guid user, string name, string type, T value)
        {
            if (string.IsNullOrEmpty(name))
                return;

            using (var db = new InternalDbContext())
            {
                var entity = db.TUserSettings
                    .SingleOrDefault(x => x.OrganizationIdentifier == organization && x.UserIdentifier == user && x.Name == name && x.ValueType == type);

                if (entity != null && EqualityComparer<T>.Default.Equals(value, default(T)))
                {
                    db.TUserSettings.Remove(entity);
                }
                else
                {
                    if (entity == null)
                    {
                        entity = new TUserSetting
                        {
                            SettingIdentifier = UniqueIdentifier.Create(),
                            OrganizationIdentifier = organization,
                            UserIdentifier = user,
                            Name = name,
                            ValueType = type,
                        };

                        db.TUserSettings.Add(entity);
                    }

                    entity.ValueJson = JsonConvert.SerializeObject(value, Formatting.None, _jsonSettings);
                }

                db.SaveChanges();
            }
        }
    }
}