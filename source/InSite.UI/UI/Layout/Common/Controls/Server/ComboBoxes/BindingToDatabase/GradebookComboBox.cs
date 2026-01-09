using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GradebookComboBox : ComboBox
    {
        public QGradebookFilter ListFilter => (QGradebookFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QGradebookFilter()));

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var gradebooks = ServiceLocator.RecordSearch.GetGradebooks(ListFilter)
                .OrderBy(x => x.GradebookTitle)
                .Select(x => new { Text = x.GradebookTitle, Value = x.GradebookIdentifier });

            foreach (var gradebook in gradebooks)
            {
                list.Add(gradebook.Value.ToString(), gradebook.Text);
            }

            return list;
        }
    }
}