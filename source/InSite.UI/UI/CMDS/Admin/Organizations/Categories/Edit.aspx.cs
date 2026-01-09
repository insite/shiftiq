using System;

using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;
using CategoryEntity = InSite.Persistence.VAchievementCategory;

namespace InSite.Cmds.Actions.Contact.Category
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/cmds/admin/organizations/search";

        public Guid? CurrentKey => Guid.TryParse(Request.QueryString["id"], out var value) ? value : (Guid?)null;

        private string OrgUrl => $"/ui/cmds/admin/organizations/edit?id={CategoryOrganization.OrganizationIdentifier}";

        private CategoryEntity _entity;
        private bool _isEntityLoaded;
        private CategoryEntity CategoryEntity
        {
            get
            {
                if (!_isEntityLoaded)
                {
                    _entity = CurrentKey.HasValue ? VAchievementCategorySearch.Select(CurrentKey.Value) : null;
                    _isEntityLoaded = true;
                }

                return _entity;
            }
        }

        private OrganizationState _organization;
        private bool _isOrganizationLoaded;

        private OrganizationState CategoryOrganization
        {
            get
            {
                if (!_isOrganizationLoaded)
                {
                    _organization = OrganizationSearch.Select(CategoryEntity.OrganizationIdentifier);
                    if (CategoryEntity != null)
                        _isOrganizationLoaded = true;
                }

                return _organization;
            }
        }

        private Guid? OrganizationIdentifier
        {
            get => (Guid?)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (CategoryOrganization != null)
                {
                    Open();

                    CancelButton.NavigateUrl = OrgUrl;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            DeleteButton.Click += DeleteButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();
            Open();

            HttpResponseHelper.Redirect(OrgUrl);
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            var redirectUrl = OrgUrl;

            TAchievementCategoryStore.Delete(CurrentKey.Value);

            HttpResponseHelper.Redirect(redirectUrl);
        }

        private void Open()
        {
            if (CategoryEntity == null || CategoryOrganization == null)
                HttpResponseHelper.Redirect(SearchUrl);

            OrganizationIdentifier = CategoryOrganization.OrganizationIdentifier;

            PageHelper.BindHeader(this, new BreadcrumbItem[]
                {
                    new BreadcrumbItem("Organizations", SearchUrl),
                    new BreadcrumbItem("Edit", OrgUrl),
                    new BreadcrumbItem("Category", null)
                }, new BreadcrumbItem("Add New Category", $"/ui/cmds/admin/organizations/categories/create?organizationId={CategoryOrganization.OrganizationIdentifier}"),
                CategoryOrganization.CompanyName
            );

            Details.SetInputValues(CategoryEntity);
        }

        private void Save()
        {
            var category = new CategoryEntity
            {
                CategoryIdentifier = CurrentKey.Value,
                OrganizationIdentifier = CategoryEntity.OrganizationIdentifier
            };

            Details.GetInputValues(category);

            TAchievementCategoryStore.Update(category);

            _isEntityLoaded = false;
            _isOrganizationLoaded = false;
        }
    }
}