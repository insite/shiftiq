using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using InSite.Application.Journals.Write;
using InSite.Application.JournalSetups.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private static readonly Regex SqlReferenceErrorRegex = new Regex(
            "The DELETE statement conflicted with the REFERENCE constraint \"(?<FkName>.+?)\". The conflict occurred in database \"(?<DbName>.+?)\", table \"(?<TableName>.+?)\", column '(?<ColumnName>.+?)'.",
            RegexOptions.Compiled);

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? GetEditorParameters()
                : null;
        }

        #endregion

        #region Methods

        private void DeleteAsset()
        {
            if (!AssetID.HasValue)
                return;

            var allIds = new HashSet<Guid>
            {
                Asset.StandardIdentifier
            };

            AppendDownstreamIds(Asset.StandardIdentifier, allIds);

            var experienceCompetencies = ServiceLocator.JournalSearch.GetExperienceCompetencies(
                x => allIds.Contains(x.CompetencyStandardIdentifier),
                x => x.Experience);
            foreach (var ec in experienceCompetencies)
                ServiceLocator.SendCommand(new DeleteExperienceCompetency(
                    ec.Experience.JournalIdentifier,
                    ec.ExperienceIdentifier,
                    ec.CompetencyStandardIdentifier));

            var competencyRequirements = ServiceLocator.JournalSearch.GetCompetencyRequirements(x => allIds.Contains(x.CompetencyStandardIdentifier));
            foreach (var cr in competencyRequirements)
                ServiceLocator.SendCommand(new DeleteCompetencyRequirement(
                    cr.JournalSetupIdentifier,
                    cr.CompetencyStandardIdentifier));

            if (StandardStore.Delete(AssetID.Value) && ParentAssetID.HasValue)
                StandardStore.Resequence(ParentAssetID.Value);
        }

        #endregion

        #region Helper methods

        private void GoBack()
        {
            var returnUrl = string.IsNullOrEmpty(ReturnUrl)
                ? FinderRelativePath
                : EditorRelativePath;

            HttpResponseHelper.Redirect(returnUrl);
        }

        private string GetEditorParameters()
        {
            return $"id={AssetThumbprint}";
        }

        #endregion

        #region Properties

        private Guid AssetThumbprint
        {
            get
            {
                if (Request.QueryString["number"] != null)
                {
                    var number = int.TryParse(Request.QueryString["number"], out var value) ? value : -1;
                    return StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.AssetNumber == number && x.OrganizationIdentifier == Organization.OrganizationIdentifier);
                }

                return Guid.TryParse(Request["asset"], out var asset) ? asset : Guid.Empty;
            }
        }

        private Standard _asset;
        private Standard Asset
        {
            get
            {
                if (_asset == null)
                    _asset = StandardSearch.SelectFirst(x => x.StandardIdentifier == AssetThumbprint);

                return _asset;
            }
        }

        private Guid? AssetID => (Guid?)ViewState[nameof(AssetID)] ?? (Guid?)(ViewState[nameof(AssetID)] = Asset?.StandardIdentifier);

        private Guid? ParentAssetID => (Guid?)ViewState[nameof(ParentAssetID)] ?? (Guid?)(ViewState[nameof(ParentAssetID)] = Asset?.ParentStandardIdentifier);

        private string ReturnUrl
        {
            get
            {
                var returnUrl = new ReturnUrl();
                return returnUrl.GetReturnUrl() ?? Request.QueryString["returnUrl"];
            }
        }

        private string EditorRelativePath
        {
            get
            {
                if (!string.IsNullOrEmpty(ReturnUrl))
                    return ReturnUrl;

                var editorRelativePath = "/ui/admin/standards/edit";
                var parameters = GetEditorParameters();

                return HttpResponseHelper.BuildUrl(editorRelativePath, parameters);
            }
        }

        private const string FinderRelativePath = "/ui/admin/standards/standards/search";

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DeleteButton.Click += DeleteButton_Click;

            CancelButton.NavigateUrl = EditorRelativePath;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!CanDelete || Asset == null)
                GoBack();

            LoadData();
        }

        private void LoadData()
        {
            PageHelper.AutoBindHeader(this, null, $"{Asset.ContentTitle} <span class='form-text'>{Asset.StandardType} Asset #{Asset.AssetNumber}</span>");

            var allIds = new HashSet<Guid>();

            AppendDownstreamIds(Asset.StandardIdentifier, allIds);

            ContainedAssetsCount.Text = allIds.Count.ToString();

            allIds.Add(Asset.StandardIdentifier);

            var categoryCount = StandardClassificationSearch.Count(x => allIds.Contains(x.StandardIdentifier));
            CategoriesCount.Text = categoryCount.ToString();

            var upstreamRelationshipCount = StandardContainmentSearch.Count(asset => allIds.Contains(asset.ChildStandardIdentifier));
            UpstreamRelationshipsCount.Text = upstreamRelationshipCount.ToString();

            var downstreamRelationshipCount = StandardContainmentSearch.Count(standard => allIds.Contains(standard.ParentStandardIdentifier));
            DownstreamRelationshipsCount.Text = downstreamRelationshipCount.ToString();

            var upstreamConnectionCount = StandardConnectionSearch.Count(asset => allIds.Contains(asset.ToStandardIdentifier));
            UpstreamConnectionsCount.Text = upstreamConnectionCount.ToString();

            var downstreamConnectionCount = StandardConnectionSearch.Count(standard => allIds.Contains(standard.FromStandardIdentifier));
            DownstreamConnectionsCount.Text = downstreamConnectionCount.ToString();

            var validationsCount = StandardValidationSearch.Count(x => allIds.Contains(x.StandardIdentifier));
            ValidationsCount.Text = validationsCount.ToString();

            var departmentProfileCompetencyCount = DepartmentProfileCompetencySearch.Count(x => allIds.Contains(x.ProfileStandardIdentifier) || allIds.Contains(x.CompetencyStandardIdentifier));
            ProfilesCount.Text = departmentProfileCompetencyCount.ToString();

            var departmentProfileUsersCount = DepartmentProfileUserSearch.Count(x => allIds.Contains(x.ProfileStandardIdentifier));
            SettingsCount.Text = departmentProfileUsersCount.ToString();

            var activityCompetenciesCount = CourseSearch.CountActivityCompetencies(x => allIds.Contains(x.CompetencyIdentifier));
            ActivityCompetenciesCount.Text = activityCompetenciesCount.ToString();

            var departmentStandardsCount = TDepartmentStandardSearch.Count(x => allIds.Contains(x.StandardIdentifier));
            DepartmentStandardsCount.Text = departmentStandardsCount.ToString();

            var standardOrganizationsCount = StandardOrganizationSearch.Count(x => allIds.Contains(x.StandardIdentifier));
            StandardOrganizationsCount.Text = standardOrganizationsCount.ToString();

            var experienceCompetenciesCount = ServiceLocator.JournalSearch.CountExperienceCompetencies(new QExperienceCompetencyFilter
            {
                CompetencyStandardIdentifier = allIds.ToArray()
            });
            ExperienceCompetenciesCount.Text = experienceCompetenciesCount.ToString();

            var competencyRequirementsCount = ServiceLocator.JournalSearch.CountCompetencyRequirements(x => allIds.Contains(x.CompetencyStandardIdentifier));
            CompetencyRequirementsCount.Text = competencyRequirementsCount.ToString();

            StandardDetails.BindStandard(Asset);
        }

        private static void AppendDownstreamIds(Guid standardIdentifier, ICollection<Guid> collection)
        {
            var children = StandardSearch.Bind(x => x.StandardIdentifier, x => x.ParentStandardIdentifier == standardIdentifier);

            foreach (var id in children)
            {
                if (collection.Contains(id))
                    continue;

                collection.Add(id);

                AppendDownstreamIds(id, collection);
            }
        }

        #endregion

        #region Event handlers

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            DeleteAsset();
            GoBack();
        }

        #endregion
    }
}