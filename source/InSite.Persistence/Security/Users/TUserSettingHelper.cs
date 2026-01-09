using System.Linq;

namespace InSite.Persistence
{
    public static class TUserSettingHelper
    {
        public static IQueryable<TUserSetting> Filter(this IQueryable<TUserSetting> query, TUserSettingFilter filter)
        {
            if (filter.UserIdentifier.HasValue)
                query = query.Where(x => x.UserIdentifier == filter.UserIdentifier);

            return query;
        }
    }
}
