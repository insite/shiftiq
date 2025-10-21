using System;
using System.Data.SqlClient;

using Shift.Common;

namespace InSite.Persistence
{
    public class TGroupSettingStore
    {
        public static void Insert(Guid groupId, string settingName, string settingValue)
        {
            using (var db = new InternalDbContext())
            {
                var entity = new TGroupSetting()
                {
                    GroupIdentifier = groupId,
                    SettingName = settingName,
                    SettingValue = settingValue,
                    SettingIdentifier = UniqueIdentifier.Create()
                };

                db.TGroupSetting.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(Guid groupId, string settingName, string[] settingValues)
        {
            Delete(groupId, settingName);

            foreach (var setting in settingValues)
            {
                Insert(groupId, settingName, setting);
            }
        }

        public static void Delete(Guid groupId, string settingName)
        {
            const string query = @"
            DELETE FROM [InSite].[contacts].[TGroupSetting]
            WHERE
                GroupIdentifier = @groupId
                AND SettingName = @settingName
            ";

            using (var db = new InternalDbContext())
            {
                db.Database.ExecuteSqlCommand(query,
                    new SqlParameter("@groupId", groupId),
                    new SqlParameter("@settingName", settingName)
                );
            }
        }
    }
}
