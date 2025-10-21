using System;
using System.Data.Entity;
using System.Linq;

namespace InSite.Persistence
{
    public static class TUserSettingSearch
    {
        private class TUserSettingReadHelper : ReadHelper<TUserSetting>
        {
            public static readonly TUserSettingReadHelper Instance = new TUserSettingReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TUserSetting>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TUserSettings.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static int Count(TUserSettingFilter filter) =>
            TUserSettingReadHelper.Instance.Count(
                (IQueryable<TUserSetting> query) => query.Filter(filter));
    }
}
