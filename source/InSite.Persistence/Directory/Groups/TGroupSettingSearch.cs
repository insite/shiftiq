using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TGroupSettingSearch
    {
        private class ReadHelper : ReadHelper<TGroupSetting>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TGroupSetting>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TGroupSetting.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static TGroupSetting Select(Guid identifier)
        {
            using (var db = new InternalDbContext())
            {
                return db.TGroupSetting.FirstOrDefault(x => x.SettingIdentifier == identifier);
            }
        }

        #region Select (TGroupSettingComboBox)

        public static TGroupSetting[] Select(Guid GroupIdentifire, string settingName)
        {
            using (var db = new InternalDbContext())
            {
                return db.TGroupSetting
                    .Where(x => x.GroupIdentifier == GroupIdentifire
                    && x.SettingName == settingName)
                    .ToArray();
            }
        }

        #endregion

        #region Select (filter)

        public static int Count(TGroupSettingFilter filter) =>
            ReadHelper.Instance.Count((IQueryable<TGroupSetting> query) => FilterQuery(query, filter));

        private static IQueryable<TGroupSetting> FilterQuery(IQueryable<TGroupSetting> query, TGroupSettingFilter filter)
        {
            if (filter.GroupIdentifier.HasValue)
                query = query.Where(x => x.GroupIdentifier == filter.GroupIdentifier.Value);

            if (filter.SettingIdentifier.HasValue)
                query = query.Where(x => x.SettingIdentifier == filter.SettingIdentifier.Value);

            if (!string.IsNullOrEmpty(filter.SettingName))
                query = query.Where(x => x.SettingName.Contains(filter.SettingName));

            if (!string.IsNullOrEmpty(filter.SettingValue))
                query = query.Where(x => x.SettingValue == filter.SettingValue);

            return query;
        }

        #endregion
    }
}
