using Newtonsoft.Json.Converters;

namespace InSite.Persistence.Plugin.CMDS
{
    public class SnapshotDateConverter : IsoDateTimeConverter
    {
        public SnapshotDateConverter()
        {
            DateTimeFormat = "yyyy-MM-dd";
        }
    }
}
