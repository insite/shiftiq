using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ExperienceList : UserControl
    {
        private Guid UserId
        {
            get => (Guid)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

        public int ItemsCount
        {
            get => (int)(ViewState[nameof(ItemsCount)] ?? 0);
            private set => ViewState[nameof(ItemsCount)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemsRequiredValidator.ServerValidate += ItemsRequiredValidator_ServerValidate;

            WorkExperienceList.ItemCommand += WorkExperienceList_ItemCommand;
        }

        private void ItemsRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ItemsCount > 0;
        }

        private void WorkExperienceList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var id = Guid.Parse(e.CommandArgument.ToString());

            TCandidateExperienceStore.Delete(id);

            BindList();
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

            ItemsCount = data.Count;

            WorkExperienceList.DataSource = data;
            WorkExperienceList.DataBind();

            WorkExperienceList.Visible = ItemsCount > 0;
            NoWorkExperienceItems.Visible = ItemsCount == 0;
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

        protected string GetEditUrl()
        {
            var dataItem = Page.GetDataItem();
            var experienceId = (Guid)DataBinder.Eval(dataItem, "ExperienceIdentifier");

            return $"/ui/portal/job/candidates/profile/edit-work-experience?id={experienceId}";
        }
    }
}