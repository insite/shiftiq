using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Custom.CMDS.Admin.Standards.Profiles
{
    public partial class Compare : AdminBasePage, ICmdsUserControl, IHasParentLinkParameters, IOverrideWebRouteParent
    {
        #region DifferenceItem

        protected class DifferenceItem
        {
            public Guid CompetencyStandardIdentifier { get; set; }
            public string LeftText { get; set; }
            public string RightText { get; set; }
            public bool IsRightEmpty { get; set; }
        }

        #endregion

        #region IHasParentLinkParameters & IOverrideWebRouteParent

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent != null && parent.Name.EndsWith("/edit")
                ? $"id={ProfileStandardIdentifier}"
                : null;
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();

        protected override string GetReturnUrl()
            => ProfileStandardIdentifier != Guid.Empty ? "/ui/cmds/admin/standards/profiles/edit" : null;

        #endregion

        #region Constants

        private const string NA = "N/A";

        #endregion

        #region Properties

        private Guid ProfileStandardIdentifier => Guid.TryParse(Request.QueryString["id"], out Guid value) ? value : Guid.Empty;

        protected bool IsLeftProfileLocked { get; set; }

        protected bool IsRightProfileLocked { get; set; }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Differences.ItemCommand += Differences_ItemCommand;

            Similarities.ItemCommand += Differences_ItemCommand;

            Profile1.AutoPostBack = true;
            Profile1.ValueChanged += Profile_ValueChanged;

            Profile2.AutoPostBack = true;
            Profile2.ValueChanged += Profile_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(
                this,
                qualifier: Organization.CompanyName);

            InitSelectors();
            LoadData();

            CloseButton.Visible = ProfileStandardIdentifier != Guid.Empty;
            CloseButton.NavigateUrl = $"/ui/cmds/admin/standards/profiles/edit?id={ProfileStandardIdentifier}";
        }

        #endregion

        #region Event handlers

        private void Differences_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var competencyID = Guid.Parse((string)e.CommandArgument);

            if (StringHelper.Equals(e.CommandName, "AddCompetency"))
                AddCompetency(competencyID);
            else if (StringHelper.Equals(e.CommandName, "DeleteCompetency"))
                DeleteCompetency(competencyID);

            LoadData();
        }

        private void Profile_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        #endregion

        #region Load data

        private void InitSelectors()
        {
            var profile = StandardSearch.Select(ProfileStandardIdentifier);
            if (profile == null)
                return;

            Profile1.Value = profile.ParentStandardIdentifier;
            Profile2.Value = profile.StandardIdentifier;
        }

        private void LoadData()
        {
            LoadProfileDetails1();
            LoadProfileDetails2();

            var id1 = Profile1.Value ?? Guid.Empty;
            var id2 = Profile2.Value ?? Guid.Empty;

            var differences1 = ProfileCompetencyRepository.SelectDifferences(id1, id2);
            var differences2 = ProfileCompetencyRepository.SelectDifferences(id2, id1);
            var differenceItems = new List<DifferenceItem>();

            AddLeftFilledColumn(differences1, differenceItems);
            AddRightFilledColumn(differences2, differenceItems);

            Differences.DataSource = differenceItems;
            Differences.DataBind();

            var similarities = ProfileCompetencyRepository.SelectSimilarities(id1, id2);
            var similarityItems = new List<DifferenceItem>();

            AddSimilarities(similarities, similarityItems);

            Similarities.DataSource = similarityItems;
            Similarities.DataBind();

            CountText.Visible = differenceItems.Count > 0 || similarityItems.Count > 0;
            CountText.Text = string.Format("There are <strong>{0}</strong> unique competencies in this profile comparison table.", differenceItems.Count + similarityItems.Count);
        }

        private void LoadProfileDetails1()
        {
            ProfileDetails1.Visible = Profile1.HasValue;
            EditProfile1.Enabled = Profile1.HasValue;

            if (!Profile1.HasValue)
                return;

            var count = ProfileCompetencyRepository.SelectCount(Profile1.Value.Value, false);

            CompetencyCount1.Text = string.Format("{0:n0} Competenc{1}", count, count == 1 ? "y" : "ies");
            EditProfile1.NavigateUrl = string.Format("/ui/cmds/admin/standards/profiles/edit?id={0}", Profile1.Value);

            var profile = StandardSearch.Select(Profile1.Value.Value);

            IsLeftProfileLocked = profile.IsLocked;
        }

        private void LoadProfileDetails2()
        {
            ProfileDetails2.Visible = Profile2.HasValue;
            EditProfile2.Enabled = Profile2.HasValue;

            if (!Profile2.HasValue)
                return;

            var count = ProfileCompetencyRepository.SelectCount(Profile2.Value.Value, false);

            CompetencyCount2.Text = string.Format("{0:n0} Competenc{1}", count, count == 1 ? "y" : "ies");
            EditProfile2.NavigateUrl = string.Format("/ui/cmds/admin/standards/profiles/edit?id={0}", Profile2.Value);

            var profile = StandardSearch.Select(Profile2.Value.Value);

            IsRightProfileLocked = profile.IsLocked;
        }

        private void AddLeftFilledColumn(DataTable table, List<DifferenceItem> differenceItems)
        {
            foreach (DataRow row in table.Rows)
            {
                DifferenceItem item = new DifferenceItem
                {
                    CompetencyStandardIdentifier = (Guid)row["CompetencyStandardIdentifier"],
                    LeftText = GetCompetencyText(row),
                    RightText = Profile2.HasValue ? NA : null,
                    IsRightEmpty = true
                };

                differenceItems.Add(item);
            }
        }

        private void AddRightFilledColumn(DataTable table, List<DifferenceItem> differenceItems)
        {
            foreach (DataRow row in table.Rows)
            {
                DifferenceItem item = new DifferenceItem
                {
                    CompetencyStandardIdentifier = (Guid)row["CompetencyStandardIdentifier"],
                    LeftText = Profile1.HasValue ? NA : null,
                    RightText = GetCompetencyText(row),
                    IsRightEmpty = false
                };

                differenceItems.Add(item);
            }
        }

        private static void AddSimilarities(DataTable table, List<DifferenceItem> similarityItems)
        {
            foreach (DataRow row in table.Rows)
            {
                DifferenceItem item = new DifferenceItem
                {
                    CompetencyStandardIdentifier = (Guid)row["CompetencyStandardIdentifier"],
                    LeftText = GetCompetencyText(row)
                };
                item.RightText = item.LeftText;
                item.IsRightEmpty = false;

                similarityItems.Add(item);
            }
        }

        private static string GetCompetencyText(DataRow row)
        {
            return string.Format(
                "<a target='_blank' href='/ui/cmds/admin/standards/competencies/edit?id={0}'>{1}</a>. {2} <span class='form-text'><br />{3}</span>",
                row["CompetencyStandardIdentifier"],
                row["Number"],
                row["Summary"],
                row["NumberOld"]
                );
        }

        #endregion

        #region Add & Delete

        private void AddCompetency(Guid competencyID)
        {
            if (!StandardContainmentSearch.Exists(x => x.ParentStandardIdentifier == Profile2.Value.Value && x.ChildStandardIdentifier == competencyID))
                StandardContainmentStore.Insert(Profile2.Value.Value, competencyID);

            // If the profile is company-specific, and the competency is not already assigned to the company, then assign it now.

            var profile = ProfileRepository.Select(Profile2.Value.Value);

            if (profile.Visibility == "Organization")
            {
                var assetOrganization = StandardOrganizationSearch.SelectFirst(x => x.OrganizationIdentifier == profile.OrganizationIdentifier && x.StandardIdentifier == competencyID);

                if (assetOrganization == null)
                {
                    assetOrganization = new StandardOrganization
                    {
                        OrganizationIdentifier = profile.OrganizationIdentifier,
                        StandardIdentifier = competencyID
                    };

                    StandardOrganizationStore.Insert(assetOrganization);
                }
            }
        }

        private void DeleteCompetency(Guid competencyID)
        {
            StandardContainmentStore.Delete(x => x.ParentStandardIdentifier == Profile2.Value.Value && x.ChildStandardIdentifier == competencyID);
        }

        #endregion
    }
}