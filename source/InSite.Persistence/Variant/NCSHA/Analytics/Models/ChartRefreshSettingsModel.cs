using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Plugin.NCSHA
{
    [JsonObject]
    public class ChartRefreshSettingsModel
    {
        #region Constants

        private static readonly string SettingName = "InSite.Custom.NCSHA.AnalyticsLastRefreshed";

        #endregion

        #region Properties

        [JsonProperty]
        public Guid UserId { get; private set; }

        [JsonProperty]
        public DateTime UtcDate { get; private set; }

        [JsonProperty]
        public string[] Surveys { get; private set; }

        [JsonProperty]
        public int[] Years { get; private set; }

        #endregion

        #region Construction

        private ChartRefreshSettingsModel()
        {

        }

        #endregion

        public static ChartRefreshSettingsModel Get()
        {
            var json = UpgradeSearch.GetData(SettingName);

            return !string.IsNullOrEmpty(json)
                ? JsonConvert.DeserializeObject<ChartRefreshSettingsModel>(json)
                : null;
        }

        public static void Set(Guid userId, string[] surveys, int[] years)
        {
            var model = new ChartRefreshSettingsModel
            {
                UserId = userId,
                UtcDate = DateTime.UtcNow,
                Surveys = surveys,
                Years = years
            };
            var json = JsonConvert.SerializeObject(model);

            var item = UpgradeSearch.Select(SettingName);

            if (item != null)
            {
                item.UtcUpgraded = DateTimeOffset.UtcNow;
                item.ScriptData = json;

                UpgradeStore.Update(item);
            }
            else
            {
                UpgradeStore.Save(SettingName, DateTimeOffset.UtcNow, json);
            }
        }
    }
}