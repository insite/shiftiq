using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ViewExperienceList : UserControl
    {
        private Guid UserId
        {
            get => (Guid)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

        public void BindModelToControls(Person person)
        {
            UserId = person.UserIdentifier;

            BindList();
        }

        private void BindList()
        {
            var data = TCandidateExperienceSearch
                .Select(x => x.UserIdentifier == UserId, null)
                .OrderByDescending(x => x.ExperienceDateTo != null ? x.ExperienceDateTo.Value : DateTime.Now)
                .ThenBy(x => x.EmployerName)
                .ToList();

            WorkExperienceList.DataSource = data;
            WorkExperienceList.DataBind();

            WorkExperienceList.Visible = data.Count > 0;
            NoWorkExperienceItems.Visible = data.Count == 0;
        }

        protected string EvalFormat(string format, params string[] names)
        {
            var dataItem = Page.GetDataItem();
            var args = names.Select(x => DataBinder.Eval(dataItem, x)).ToArray();
            var text = string.Format(format, args);

            return HttpUtility.HtmlEncode(text);
        }

        protected string TryGetParagraph(string text)
        {
            text = text?.Trim();

            return text.IsEmpty()
                ? string.Empty
                : $"<p style='white-space:pre-wrap;'>{HttpUtility.HtmlEncode(text)}</p>";
        }

        protected string GetYearString(string name)
        {
            var dataItem = Page.GetDataItem();
            var date = (DateTime?)DataBinder.Eval(dataItem, name);

            return date.HasValue
                ? date.Value.Year.ToString()
                : "Current";
        }

        protected string GetJobTitle()
        {
            var dataItem = Page.GetDataItem();
            var text = (string)DataBinder.Eval(dataItem, "ExperienceJobTitle");

            return HttpUtility.HtmlEncode(text);
        }
    }
}