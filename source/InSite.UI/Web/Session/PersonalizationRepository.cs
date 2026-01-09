using System;
using System.Collections.Generic;
using System.Web;

using InSite.Persistence;

using Shift.Common;

namespace InSite
{
    public static class PersonalizationRepository
    {
        public static T GetValue<T>(Guid organization, Guid user, string name, bool validate)
            => GetValue<T>(organization, user, name, GetValueType<T>(), validate);

        private static string GetValueType<T>() => typeof(T).ToString();

        private static T GetValue<T>(Guid organization, Guid user, string name, string type, bool validate) =>
            HttpContext.Current.User.Identity.IsAuthenticated
                ? TUserSettingStore.GetValue<T>(organization, user, name, type, validate)
                : GetSessionValue<T>(organization, name, type);

        private static T GetSessionValue<T>(Guid organization, string name, string type)
        {
            var cache = GetSessionCache;
            var key = GetSessionCacheKey(organization, name, type);

            T result = default;

            try
            {
                if (cache.ContainsKey(key))
                    result = (T)cache[key];
            }
            catch (InvalidCastException)
            {

            }

            return result;
        }

        private static Dictionary<MultiKey, object> GetSessionCache
        {
            get => CurrentSessionState.PersonalizationCache
                ?? (CurrentSessionState.PersonalizationCache = new Dictionary<MultiKey, object>());
            set => CurrentSessionState.PersonalizationCache = value;
        }

        private static MultiKey GetSessionCacheKey(Guid organization, string name, string type) => new MultiKey(organization, name, type);

        public static void SetValue<T>(Guid organization, Guid user, string name, T value)
            => SetValue(organization, user, name, GetValueType<T>(), value);

        private static void SetValue<T>(Guid organization, Guid user, string name, string type, T value)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                TUserSettingStore.SetValue<T>(organization, user, name, type, value);
            else
                SetSessionValue(organization, name, type, value);
        }

        private static void SetSessionValue<T>(Guid organization, string name, string type, T value)
        {
            var cache = GetSessionCache;
            var key = GetSessionCacheKey(organization, name, type);

            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                cache.Remove(key);
            }
            else
            {
                if (cache.ContainsKey(key))
                    cache[key] = value;
                else
                    cache.Add(key, value);
            }
        }
    }
}
