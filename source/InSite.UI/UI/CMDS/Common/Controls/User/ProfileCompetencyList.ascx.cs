using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;

namespace InSite.Cmds.Controls.Profiles.Profiles
{
    public partial class ProfileCompetencyList : UserControl
    {
        #region Events

        public event IntValueHandler Refreshed;
        private void OnRefreshed(int count) => Refreshed?.Invoke(this, new IntValueArgs(count));

        public event AlertHandler Alert;
        private void OnAlert(AlertType type, string text) => Alert?.Invoke(this, new AlertArgs(type, text));

        #endregion

        #region Properties

        private Guid ProfileStandardIdentifier
        {
            get { return (Guid)ViewState[nameof(ProfileStandardIdentifier)]; }
            set { ViewState[nameof(ProfileStandardIdentifier)] = value; }
        }

        private bool IsNewCompetenciesSearched
        {
            get { return (bool)(ViewState[nameof(IsNewCompetenciesSearched)] ?? false); }
            set { ViewState[nameof(IsNewCompetenciesSearched)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteCompetencyButton.Click += DeleteCompetencyButton_Click;
            AddCompetencyButton.Click += AddCompetencyButton_Click;
            AddMultipleButton.Click += AddMultipleButton_Click;

            FilterButton.Click += FilterButton_Click;
            ClearButton.Click += ClearButton_Click;
        }

        protected override void OnPreRender(EventArgs e)
        {
            SelectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', true);", AddedCompetencies.ClientID);
            UnselectAllButton.OnClientClick = string.Format("return setCheckboxes('{0}', false);", AddedCompetencies.ClientID);

            DeleteCompetencyButton.OnClientClick = "return confirm('Are you sure you want to delete selected qualifications?');";

            SelectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', true);", NewCompetencies.ClientID);
            UnselectAllButton2.OnClientClick = string.Format("return setCheckboxes('{0}', false);", NewCompetencies.ClientID);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void AddMultipleButton_Click(object sender, EventArgs e)
        {
            AddMultipleCompetencies();
        }

        private void DeleteCompetencyButton_Click(object sender, EventArgs e)
        {
            DeleteCompetencies();
        }

        private void AddCompetencyButton_Click(object sender, EventArgs e)
        {
            AddCompetency();
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = true;
            LoadCompetencies();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            IsNewCompetenciesSearched = false;
            SearchText.Text = null;
            SearchProfile.Value = null;
            LoadCompetencies();
        }

        private void RefreshCompetenciesButton_Click(object sender, EventArgs e)
        {
            LoadCompetencies();
        }

        #endregion

        #region Public methods

        public int LoadData(Standard profile)
        {
            ProfileStandardIdentifier = profile.StandardIdentifier;

            SearchProfile.Filter.ExcludeProfileStandardIdentifier = ProfileStandardIdentifier;

            CompetencyButtons.Visible = !profile.IsLocked;
            NewCompetencyTab.Visible = !profile.IsLocked;
            AddMultipleQualificationsTab.Visible = !profile.IsLocked;

            return LoadCompetencies();
        }

        #endregion

        #region Load competencies

        private int LoadCompetencies()
        {
            DataTable data = CompetencyRepository.SelectProfileCompetencies(ProfileStandardIdentifier);

            AddedCompetencies.DataSource = data;
            AddedCompetencies.DataBind();
            CompetencyTab.SetTitle("Competencies", data.Rows.Count);
            CompetencyTab.Visible = data.Rows.Count > 0;

            if (IsNewCompetenciesSearched)
            {
                DataTable newCompetencies = CompetencyRepository.SelectNewProfileCompetencies(ProfileStandardIdentifier, SearchText.Text, SearchProfile.Value);

                NewCompetencies.DataSource = newCompetencies;
                NewCompetencies.DataBind();

                CompetencyList.Visible = newCompetencies.Rows.Count > 0;

                FoundCompetency.Visible = true;

                if (newCompetencies.Rows.Count > 0)
                    FoundCompetency.InnerHtml = string.Format("Found {0} competencies:", newCompetencies.Rows.Count);
                else
                    FoundCompetency.InnerHtml = "No competencies found";
            }
            else
            {
                NewCompetencies.DataSource = null;
                NewCompetencies.DataBind();

                CompetencyList.Visible = false;
                FoundCompetency.Visible = false;
            }

            return data.Rows.Count;
        }

        #endregion

        #region Save & Delete Profile Competencies

        private void AddCompetency()
        {
            var list = new List<StandardContainment>();

            foreach (RepeaterItem item in NewCompetencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                if (!StandardContainmentSearch.Exists(x => x.ParentStandardIdentifier == ProfileStandardIdentifier && x.ChildStandardIdentifier == competencyID))
                    list.Add(new StandardContainment { ParentStandardIdentifier = ProfileStandardIdentifier, ChildStandardIdentifier = competencyID });
            }

            StandardContainmentStore.Insert(list);

            AddCompetenciesToPersons();

            VCmdsCompetencyOrganizationRepository.InsertProfileCompetencies(ProfileStandardIdentifier);

            var totalCount = LoadCompetencies();
            OnRefreshed(totalCount);
        }

        private void AddMultipleCompetencies()
        {
            var text = MultipleCompetencyNumbers.Text;

            if (string.IsNullOrEmpty(text))
                return;

            int count = 0;

            string[] numbers = StringHelper.Split(text);

            var competencyIDs = new HashSet<Guid>();

            foreach (var number in numbers)
            {
                var competencyList = CompetencyRepository.SelectByNumber(number);

                foreach (var competency in competencyList)
                    competencyIDs.Add(competency.StandardIdentifier);
            }

            var list = new List<StandardContainment>();

            foreach (var competencyID in competencyIDs)
            {
                if (!StandardContainmentSearch.Exists(x => x.ParentStandardIdentifier == ProfileStandardIdentifier && x.ChildStandardIdentifier == competencyID))
                    list.Add(new StandardContainment { ParentStandardIdentifier = ProfileStandardIdentifier, ChildStandardIdentifier = competencyID });
            }

            StandardContainmentStore.Insert(list);

            AddCompetenciesToPersons();

            VCmdsCompetencyOrganizationRepository.InsertProfileCompetencies(ProfileStandardIdentifier);

            OnAlert(AlertType.Success, $"{count:n0} competencies have been added to this profile");

            MultipleCompetencyNumbers.Text = String.Empty;

            var totalCount = LoadCompetencies();
            OnRefreshed(totalCount);
        }

        private void AddCompetenciesToPersons()
        {
            var employeeIDs = UserProfileRepository
                .SelectByProfileStandardIdentifier(ProfileStandardIdentifier)
                .Select(x => x.UserIdentifier)
                .Distinct()
                .ToList();

            UserCompetencyRepository.AddNewCompetencies(ProfileStandardIdentifier, employeeIDs);
        }

        private void DeleteCompetencies()
        {
            var competencyIDs = new List<Guid>();

            foreach (RepeaterItem item in AddedCompetencies.Items)
            {
                var competency = (ICheckBoxControl)item.FindControl("Competency");
                if (!competency.Checked)
                    continue;

                var competencyIDControl = (ITextControl)item.FindControl("CompetencyStandardIdentifier");
                var competencyID = Guid.Parse(competencyIDControl.Text);

                competencyIDs.Add(competencyID);
            }

            StandardContainmentStore.Delete(x => x.ParentStandardIdentifier == ProfileStandardIdentifier && competencyIDs.Contains(x.ChildStandardIdentifier));
            DepartmentProfileCompetencyRepository2.DeleteUnusedByProfileId(ProfileStandardIdentifier);
            VCmdsCompetencyOrganizationRepository.DeleteUnusedByProfileId(ProfileStandardIdentifier);

            var totalCount = LoadCompetencies();
            OnRefreshed(totalCount);
        }

        #endregion
    }
}