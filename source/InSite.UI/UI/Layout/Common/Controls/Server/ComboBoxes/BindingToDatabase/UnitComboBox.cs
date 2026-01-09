using System;
using System.Linq;

using InSite.Persistence;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class UnitComboBox : ComboBox
    {
        public Guid? CourseID
        {
            get => (Guid?)ViewState[nameof(CourseID)];
            set => ViewState[nameof(CourseID)] = value;
        }

        public bool ShowCodes
        {
            get => (bool?)ViewState[nameof(ShowCodes)] ?? false;
            set => ViewState[nameof(ShowCodes)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var data = CourseSearch
                .BindUnits(
                    x => new
                    {
                        x.UnitSequence,
                        x.UnitCode,
                        x.UnitName,
                        x.UnitIdentifier
                    },
                    x => x.CourseIdentifier == CourseID
                )
                .Select(x => new
                {
                    Sequence = x.UnitSequence,
                    Text = ShowCodes && !string.IsNullOrEmpty(x.UnitCode) ? $"{x.UnitCode} {x.UnitName}" : x.UnitName,
                    Value = x.UnitIdentifier
                })
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.Text)
                .ToList();

            var list = new ListItemArray
            {
                TotalCount = data.Count
            };

            foreach (var item in data)
                list.Add(item.Value.ToString(), item.Text);

            return list;
        }
    }
}