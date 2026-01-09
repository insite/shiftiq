using System;
using System.Linq;

using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class AlertComboBox : ComboBox
    {
        public string[] ExcludeValues
        {
            get => (string[])ViewState[nameof(ExcludeValues)];
            set => ViewState[nameof(ExcludeValues)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var all = Notifications.All
                .AsQueryable()
                .Where(x => !x.IsObsolete
                    && (
                        x.Organizations == null
                         || x.Organizations.Length == 0
                         || x.Organizations.Any(o => o == CurrentSessionState.Identity.Organization.Code)
                    )
                );

            if (ExcludeValues.IsNotEmpty())
                all = all.Where(x => !ExcludeValues.Contains(x.Slug, StringComparer.OrdinalIgnoreCase));

            var list = new ListItemArray();

            foreach (var i in all.OrderBy(x => x.Slug))
                list.Add(i.Slug);

            return list;
        }
    }
}