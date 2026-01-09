using System;
using System.Linq;

using InSite.Application.Achievements.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class AddPrerequisite : AdminBasePage, IHasParentLinkParameters
    {
        private Guid AchievementIdentifier => Guid.TryParse(Request["achievement"], out var value) ? value : Guid.Empty;

        public string GetParentLinkParameters(IWebRoute parent)
            => parent.Name.EndsWith("/outline") ? $"id={AchievementIdentifier}" : null;

        private QAchievement _achievement;
        private ListItemArray _prerequisites
        {
            get { return (ListItemArray)ViewState[nameof(_prerequisites)]; }
            set { ViewState[nameof(_prerequisites)] = value; }
        }

        public string OutlineUrl
            => $"/ui/admin/records/achievements/outline?id={AchievementIdentifier}";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AchievementCombo.Filter.OrganizationIdentifiers.Add(Organization.OrganizationIdentifier);

            AddButton.Click += (x, y) => AddToList(AchievementCombo.Value);
            UndoButton.Click += (x, y) => UndoList();

            SaveButton.Click += (x, y) => SaveChanges();
            CancelButton.NavigateUrl = OutlineUrl;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!InitModel())
                return;

            if (!IsPostBack)
                BindModelToControls();
        }

        private bool InitModel()
        {
            _achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier);
            if (_achievement == null)
                AchievementNotFound();

            if (_prerequisites == null)
                _prerequisites = new ListItemArray();

            return ActionPanel.Visible;
        }

        private void AchievementNotFound()
        {
            Status.AddMessage(AlertType.Error, $"<strong><i class='fas fa-exclamation-stop'></i> Achievement Not Found</strong>: {AchievementIdentifier}");
            ActionPanel.Visible = false;
        }

        private void AddToList(Guid? value)
        {
            if (value != null && value.Value != AchievementIdentifier)
            {
                var achievement = ServiceLocator.AchievementSearch.GetAchievement(value.Value);
                if (achievement != null)
                    _prerequisites.Add(achievement.AchievementIdentifier.ToString(), achievement.AchievementTitle);
            }

            AchievementCombo.Value = null;

            BindModelToControls();
        }

        private void UndoList()
        {
            _prerequisites = new ListItemArray();
            BindModelToControls();
        }

        private void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this, null, _achievement.AchievementTitle);
            AchievementRepeater.DataSource = _prerequisites;
            AchievementRepeater.DataBind();
            AchievementRepeaterEmpty.Visible = !_prerequisites.Any();
            AchievementRepeater.Visible = _prerequisites.Any();
        }

        private void SaveChanges()
        {
            var prerequisite = UniqueIdentifier.Create();
            var conditions = _prerequisites.Items.Select(x => Guid.Parse(x.Value)).ToArray();
            var add = new AddAchievementPrerequisite(AchievementIdentifier, prerequisite, conditions);
            ServiceLocator.SendCommand(add);
            HttpResponseHelper.Redirect(OutlineUrl);
        }
    }
}
