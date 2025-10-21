using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using InSite.Common.Web.UI;
using InSite.Domain.Reports;
using InSite.Persistence.Plugin.NCSHA;
using InSite.UI.Layout.Admin;

namespace InSite.Custom.NCSHA.History.Forms
{
    public partial class Search : SearchPage<HistoryFilter>
    {
        public class EventInfo
        {

            public string Group { get; }
            public string Name { get; }
            public bool Visible { get; }
            public Type Type { get; }

            public EventInfo(NcshaHistoryEventAttribute attr, Type type)
            {
                Group = attr.Type;
                Name = attr.Name;
                Visible = attr.Visible;
                Type = type;
            }
        }

        public override string EntityName => "History Record";

        public static EventInfo[] DeclaredEvents { get; }

        static Search()
        {
            var historyEvents = new List<EventInfo>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.StartsWith("InSite.")))
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        var attributes = type.GetCustomAttributes(typeof(NcshaHistoryEventAttribute), false);
                        if (attributes.Length == 1)
                            historyEvents.Add(new EventInfo((NcshaHistoryEventAttribute)attributes[0], type));
                    }
                }
                catch (ReflectionTypeLoadException)
                {

                }
            }

            DeclaredEvents = historyEvents.ToArray();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(this);
        }

        protected override IEnumerable<DownloadColumn> GetExportColumns()
        {
            return new[]
            {
                new DownloadColumn("UserName", "User", null, 30),
                new DownloadColumn("UserEmail", "Email", null, 30),
                new DownloadColumn("UserGroup", "Group", null, 30),
                new DownloadColumn("EventGroup", "Event Type", null, 20),
                new DownloadColumn("EventName", "Event Name", null, 20),
                new DownloadColumn("RecordTimeFormated", "Event Time", null, 30),
                new DownloadColumn("Name", "Name", null, 75),
                new DownloadColumn("Region", "Region", null, 30),
                new DownloadColumn("Year", "Year", null, 20),
                new DownloadColumn("Options", "Options", null, 20),
                new DownloadColumn("Axis", "Axis", null, 20),
            };
        }
    }
}