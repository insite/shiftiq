using System;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Cmds.User.Competencies.Controls;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

using Control = System.Web.UI.Control;

namespace InSite.Cmds.Controls.Talents.EmployeeCompetencies
{
    public abstract class EmployeeCompetencyEditorController : AdminBasePage, ICmdsUserControl
    {
        #region Security

        public override void ApplyAccessControlForCmds()
        {
            base.ApplyAccessControlForCmds();

            if (!Access.Read)
                base.ApplyAccessControlForCmds();

            if (!Access.Read)
                return;

            if (CurrentIdentifier == Guid.Empty)
                return;

            if (!VCmdsProfileOrganizationRepository.IsCompetencyInCompany(CurrentIdentityFactory.ActiveOrganizationIdentifier, CurrentIdentifier))
                Access = Access.SetAll(false);
        }

        #endregion

        #region Event handlers

        private void MoveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (MustSaveOnMove)
                Save();

            var moveButton = (IButton)sender;

            var action = ((IAdminPage)Page).Route.Name;

            var url = string.Format(
                "/{0}?id={1}&userID={2}&keepWorkingSet=yes"
                , action
                , moveButton.CommandArgument
                , UserId
            );

            Response.Redirect(url);
        }

        #endregion

        #region Init position

        private void InitPosition()
        {
            if (!StringHelper.Equals(Request["keepWorkingSet"], "yes"))
                CompetencyPositionHelper.ClearWorkingSet(SearchRouteAction);

            var position = CompetencyPositionHelper.GetPosition(new CompetencyPositionParameter
            {
                CompetencyStandardIdentifier = CurrentIdentifier,
                UserIdentifier = UserId,
                CriteriaType = typeof(EmployeeCompetencySearchResults),
                SearchRouteAction = SearchRouteAction,
                IsCompetenciesToValidate = IsCompetenciesToValidate
            });

            if (position == null)
            {
                _position.Visible = false;
                return;
            }

            _position.Text = string.Format("Competency {0} of {1}", position.CurrentNumber, position.Count);
            _nextButton.CommandArgument = position.NextCompetencyStandardIdentifier.ToString();

            if (_prevButton != null)
                _prevButton.CommandArgument = position.PrevCompetencyStandardIdentifier.ToString();
        }

        #endregion

        #region Field

        private IButton _nextButton;
        private IButton _prevButton;
        private Repeater _profiles;
        private Control _statusHistorySection;
        private Repeater _statusHistory;
        private System.Web.UI.WebControls.Literal _position;
        private CompetencyAchievementAndDownloadViewer _competencyAchievements;
        private Control _competencyAchievementsSection;

        #endregion

        #region Properties

        protected Guid UserId
        {
            get
            {
                var employee = Request["userID"];
                if (string.IsNullOrEmpty(employee))
                    return User.UserIdentifier;

                if (Guid.TryParse(employee, out Guid result))
                    return result;

                Response.Redirect(SearchUrl);
                return Guid.Empty;
            }
        }

        protected virtual bool IsCompetenciesToValidate => false;

        protected virtual bool MustSaveOnMove => true;

        protected string SearchUrl => "/" + SearchRouteAction;

        protected abstract string SearchRouteAction { get; }

        protected Guid CurrentIdentifier => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        #endregion

        #region Initialization

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            InitControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitControls();

            if (!IsPostBack)
            {
                InitPosition();
                Open();
            }
        }

        private void InitControls()
        {
            if (_nextButton != null)
                return;

            _nextButton = (IButton)ControlHelper.GetControl(this, "NextButton");
            _nextButton.Click += MoveButton_Click;

            _prevButton = (IButton)ControlHelper.GetControl(this, "PrevButton");

            if (_prevButton != null)
                _prevButton.Click += MoveButton_Click;

            _profiles = (Repeater)ControlHelper.GetControl(this, "Profiles");

            _statusHistorySection = ControlHelper.GetControl(this, "StatusHistorySection");
            _statusHistory = (Repeater)ControlHelper.GetControl(this, "StatusHistory");

            _position = (System.Web.UI.WebControls.Literal)ControlHelper.GetControl(this, "Position");

            _competencyAchievements = (CompetencyAchievementAndDownloadViewer)ControlHelper.GetControl(this, "CompetencyAchievements");
            _competencyAchievementsSection = ControlHelper.GetControl(this, "CompetencyAchievementsSection");
        }

        #endregion

        #region Load & Save

        protected virtual void Open()
        {
            var info = StandardValidationSearch.SelectFirst(x => x.UserIdentifier == UserId && x.StandardIdentifier == CurrentIdentifier);

            if (info == null)
                Response.Redirect(SearchUrl);

            SetInputValues(info);

            _profiles.DataSource = ProfileRepository.SelectEmployeeProfiles(UserId, CurrentIdentifier, CurrentIdentityFactory.ActiveOrganizationIdentifier);
            _profiles.DataBind();

            BindStatusHistory();

            var hasData = _competencyAchievements.LoadData(CurrentIdentifier, UserSearch.Select(UserId).UserIdentifier, Organization.Identifier);

            if (_competencyAchievementsSection != null)
                _competencyAchievementsSection.Visible = hasData;
        }

        protected abstract void Save();

        #endregion

        #region Setting and getting input values

        protected abstract void SetInputValues(StandardValidation info);

        #endregion

        #region Data binding

        protected void BindStatusHistory()
        {
            var eventTable = StandardValidationChangeSearch.SelectStatusHistory(CurrentIdentifier, UserId);
            _statusHistory.DataSource = eventTable;
            _statusHistory.DataBind();

            if (_statusHistorySection != null)
                _statusHistorySection.Visible = eventTable.Count > 0;
        }

        protected string GetStatusDate(object statusDate)
        {
            return statusDate == DBNull.Value
                ? "&nbsp;"
                : TimeZones.Format((DateTimeOffset)statusDate, User.TimeZone, true);
        }

        protected string GetUserFullName(object name)
        {
            return name == null || name == DBNull.Value ? "" : $" by {name}";
        }

        protected static string GetCategories(Guid competencyIdentifier)
        {
            var categories = StandardClassificationSearch
                .Bind(x => x.CategoryIdentifier, x => x.StandardIdentifier == competencyIdentifier)
                .Select(x => TCollectionItemCache.GetName(x))
                .Where(x => x.IsNotEmpty())
                .OrderBy(x => x)
                .ToList();

            if (categories.Count == 0)
                return null;

            var html = new StringBuilder();

            foreach (var category in categories)
            {
                if (html.Length > 0)
                    html.Append(", ");

                html.Append(category);
            }

            return html.ToString();
        }

        #endregion
    }
}