using System;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Records.Logbooks.Models;

using Shift.Common;
using Shift.Constant;

using static InSite.Admin.Records.Logbooks.CompetencyHelper;

namespace InSite.UI.Portal.Records.Logbooks
{
    public partial class OutlineEntry : PortalBasePage, IHasParentLinkParameters
    {
        private Guid ExperienceIdentifier => Guid.TryParse(Request["experience"], out var id) ? id : Guid.Empty;

        private Guid JournalSetupIdentifier
        {
            get => (Guid)ViewState[nameof(JournalSetupIdentifier)];
            set => ViewState[nameof(JournalSetupIdentifier)] = value;
        }

        private string JournalSetupName
        {
            get => (string)ViewState[nameof(JournalSetupName)];
            set => ViewState[nameof(JournalSetupName)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            if (Request.QueryString["panel"] == "comments")
                CommentsPanel.IsSelected = true;

            PageHelper.AutoBindHeader(this, null, GetTitle());
        }

        private void LoadData()
        {
            var experience = ServiceLocator.JournalSearch.GetExperience(ExperienceIdentifier, x => x.Journal.JournalSetup, x => x.Journal.Experiences);

            if (experience == null
                || experience.Journal.JournalSetup.OrganizationIdentifier != Organization.OrganizationIdentifier
                || experience.Journal.UserIdentifier != User.UserIdentifier
                )
            {
                HttpResponseHelper.Redirect("/ui/portal/records/logbooks/search");
                return;
            }

            var journalSetup = experience.Journal.JournalSetup;
            JournalSetupIdentifier = journalSetup.JournalSetupIdentifier;
            JournalSetupName = journalSetup.JournalSetupName;

            Detail.LoadData(JournalSetupIdentifier, experience);

            CompetenciesPanel.Visible = LoadCompetencies();

            Comments.LoadData(JournalSetupIdentifier, ExperienceIdentifier, experience.Journal.Experiences.ToList());
        }

        public bool LoadCompetencies()
        {
            var areas = CompetencyHelper.GetAreasByExperience(ExperienceIdentifier, JournalSetupIdentifier, User.UserIdentifier, Identity.Language, true);
            if (areas == null)
                return false;

            AreaRepeater.ItemDataBound += AreaRepeater_ItemDataBound;
            AreaRepeater.DataSource = areas;
            AreaRepeater.DataBind();

            return true;
        }

        private void AreaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var area = (CompetencyHelper.AreaItem)e.Item.DataItem;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = area.Competencies;
            competencyRepeater.DataBind();
        }

        protected string GetSatisfactionLevelHtml(string satisfactionLevel)
        {
            var level = satisfactionLevel.ToEnum(ExperienceCompetencySatisfactionLevel.None);

            string @class, text;

            switch (level)
            {
                case ExperienceCompetencySatisfactionLevel.None:
                    return string.Empty;
                case ExperienceCompetencySatisfactionLevel.NotSatisfied:
                    @class = "danger";
                    text = "Not Satisfied";
                    break;
                case ExperienceCompetencySatisfactionLevel.PartiallySatisfied:
                    @class = "warning";
                    text = "Partially Satisfied";
                    break;
                case ExperienceCompetencySatisfactionLevel.Satisfied:
                    @class = "success";
                    text = "Satisfied";
                    break;
                default:
                    throw new ArgumentException($"Unsupported satisfaction level: {satisfactionLevel}");
            }

            return $"<span class='badge bg-{@class}'>{text}</span>";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"journalsetup={JournalSetupIdentifier}"
                : null;
        }

        private string GetTitle()
        {
            var content = ServiceLocator.ContentSearch.GetBlock(JournalSetupIdentifier);
            return content?.Title?.Text.Get(Identity.Language) ?? JournalSetupName;
        }
    }
}