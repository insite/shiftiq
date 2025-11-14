using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.MyProfile
{
    public partial class View : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            BindModelToControls(CreateModel());
        }

        private ViewModel CreateModel()
        {
            var model = new ViewModel
            {
                Person = PersonSearch.Select(Organization.Identifier, User.Identifier)
            };

            if (Guid.TryParse(Request.QueryString["group"], out Guid id))
                model.Employer = ServiceLocator.GroupSearch.GetGroup(id, x => x.Parent);
            else if (model.Person?.EmployerGroupIdentifier != null)
                model.Employer = ServiceLocator.GroupSearch.GetGroup(model.Person.EmployerGroupIdentifier.Value, x => x.Parent);

            if (model.Employer != null)
            {
                model.EmployerAddress = ServiceLocator.GroupSearch.GetAddress(model.Employer.GroupIdentifier, AddressType.Physical);
                model.Settings = TGroupSettingSearch.Select(model.Employer.GroupIdentifier, "Company Sector");
            }

            return model;
        }

        private void BindModelToControls(ViewModel model)
        {
            if (!BindGroup(model.Employer, model.EmployerAddress))
                return;

            BindPerson(model.Person);
            BindSectors(model.Settings);
            BindPhotos(model.Employer.GroupIdentifier);
            BindOccupations(model.Employer.GroupIdentifier);
        }

        private void BindOccupations(Guid group)
        {
            var occupations = Edit.GetOccupations(group).Where(x => x.IsSelected).ToList();
            OccupationList.DataSource = occupations;
            OccupationList.DataBind();
            OccupationsField.Visible = occupations.Count > 0;
        }

        private void BindPerson(Person person)
        {
            if (person == null)
            {
                StatusAlert.AddMessage(AlertType.Error, $"Your your account is not registered with {Organization.Name}. Please contact your administrator to resolve this.");
                return;
            }

            if (person.JobsApproved.HasValue)
                EmpAccountStatus.Text = "<span class='text-success'><i class='far fa-check-circle me-1'></i>Account is Verified</span>";
            else
                EmpAccountStatus.Text = "<span class='text-danger'><i class='far fa-times-circle me-1'></i>Account is Pending Approval</span>";
        }

        private bool BindGroup(QGroup employer, QGroupAddress address)
        {
            if (employer == null)
            {
                StatusAlert.AddMessage(AlertType.Error, "No employer is selected for your account. Please contact your administrator to resolve this.");
                return false;
            }

            ViewDetail.Visible = true;
            MyJobOpportunityGrid.Visible = true;
            PhotoGalleryPanel.Visible = true;

            EmployerName.Text = employer.GroupName.IfNullOrEmpty("N/A");
            BindGroupTags(employer.GroupIdentifier, EmployerTags);

            EmpIndustry.Text = string.IsNullOrEmpty(employer.GroupIndustry)
                ? "N/A"
                : (!string.IsNullOrEmpty(employer.GroupIndustryComment) && string.Equals(employer.GroupIndustry, "Other", StringComparison.OrdinalIgnoreCase)
                    ? employer.GroupIndustryComment
                    : employer.GroupIndustry
                );

            EmpNumberText.Text = employer.GroupSize.IfNullOrEmpty("N/A");
            EmpPhone.Text = employer.GroupPhone.IfNullOrEmpty("N/A");

            GroupDescription.Text = Markdown.ToHtml(employer.GroupDescription);
            EditButton.NavigateUrl = $"/ui/portal/job/employers/profile/edit?group={employer.GroupIdentifier}";

            if (address != null)
            {
                EmpAddressLine.Text = address.Street1;
                EmpAddressCity.Text = address.City;
                EmpAddressProvince.Text = address.Province;
                EmpAddressCountry.Text = address.Country;
                EmpAddressPostalCode.Text = address.PostalCode;
            }

            AddressField.Visible = !string.IsNullOrEmpty(EmpAddressLine.Text)
                || !string.IsNullOrEmpty(EmpAddressCity.Text)
                || !string.IsNullOrEmpty(EmpAddressProvince.Text)
                || !string.IsNullOrEmpty(EmpAddressCountry.Text)
                || !string.IsNullOrEmpty(EmpAddressPostalCode.Text);

            WebSiteUrl.Text = !string.IsNullOrEmpty(employer.GroupWebSiteUrl)
                ? $"<a href='{employer.GroupWebSiteUrl}' target=_blank>{employer.GroupWebSiteUrl}</a>"
                : "N/A";

            BindSocialMediaUrls(employer);
            BindParent(employer);
            BindSites(employer.GroupIdentifier);

            MyJobOpportunityGrid.LoadData(employer.GroupIdentifier);

            return true;
        }

        public static void BindGroupTags(Guid groupId, Literal literal)
        {
            var selections = ServiceLocator.GroupSearch.GetGroupTags(groupId);
            var tags = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                CollectionName = CollectionName.Contacts_Groups_Classification_Tag
            });

            var html = new StringBuilder();
            foreach (var tag in tags)
                if (selections.Any(x => StringHelper.Equals(x, tag.ItemName)))
                    html.Append($"<span class='badge bg-info me-1 mb-1'>{tag.ItemName}</span>");

            literal.Text = html.ToString();
        }

        private void BindSocialMediaUrls(QGroup group)
        {
            var socialMediaUrls = QGroupStore.GetSocialMediaUrls(group);
            var sb = new StringBuilder();

            var links = socialMediaUrls.OrderBy(x => x.Key).ToList();
            foreach (var link in links)
                sb.Append($"<div class='mb-2'><i class='fab fa-{link.Key.ToLower()}'></i> <a target='_blank' href='{link.Value}'>{link.Key}</a></div>");

            SocialMediaUrls.Text = sb.ToString();
            SocialMediaPanel.Visible = socialMediaUrls.Count > 0;
        }

        private void BindParent(QGroup group)
        {
            var companyHeading = (group.GroupLabel ?? "Site") + " Information";
            var employerHeading = (group.GroupLabel ?? "Site");

            CompanyHeading.InnerText = companyHeading;
            EmployerLabel.InnerText = employerHeading;

            var parent = group.Parent;
            ParentPanel.Visible = parent != null && parent.GroupType == "Employer";
            if (!ParentPanel.Visible)
                return;

            var parentLabel = (parent.GroupLabel ?? "Company");
            ParentLabel.InnerText = parentLabel;

            if (Identity.IsInGroup(parent.GroupName))
                ParentName.Text = $"<a href='/ui/portal/job/employers/profile/view?group={parent.GroupIdentifier}'>{parent.GroupName}</a>";
            else
                ParentName.Text = parent.GroupName;
        }

        private void BindSites(Guid group)
        {
            SitesPanel.Visible = false;

            var subgroups = ServiceLocator.GroupSearch.GetSubgroups(group);
            if (subgroups.Count == 0)
                return;

            SitesPanel.Visible = true;

            SiteRepeater.DataSource = subgroups;
            SiteRepeater.DataBind();
        }

        private void BindSectors(TGroupSetting[] settings)
        {
            if (settings == null)
                return;

            var interests = new List<TCollectionItem>();

            foreach (var item in settings)
                interests.Add(new TCollectionItem() { ItemName = item.SettingValue });

            // EmpSector.Text = string.Join(", ", interests.Select(x => x.ItemName));
        }

        #region Binding (Photos)

        private static readonly object _syncRoot = new object();

        private void BindPhotos(Guid groupId)
        {
            var dataItems = GetDataItems(groupId);

            GalleryItemRepeater.DataSource = dataItems.Select(x => new
            {
                IsVideo = x.Type == "video",
                HasCaption = x.CaptionText != null,
                x.ID,
                x.ResourceUrl,
                x.ThumbnailUrl,
                x.CaptionText
            });
            GalleryItemRepeater.DataBind();

            PhotoGalleryPanel.Visible = dataItems.Length > 0;
        }

        private DataItem[] GetDataItems(Guid groupId)
        {
            var DataUploadPath = $@"{ServiceLocator.FilePaths.FileStoragePath}\Tenants\{CurrentSessionState.Identity.Organization.OrganizationCode}\Contacts\Groups\{groupId}\Photos\";

            if (!Directory.Exists(DataUploadPath))
                return new DataItem[0];

            var result = new List<DataItem>();

            lock (_syncRoot)
            {
                var directory = new DirectoryInfo(DataUploadPath);

                var files = directory.GetFiles("*.json");
                for (var i = 0; i < files.Length; i++)
                    result.Add(ReadDataItem(files[i]));
            }

            return result
                .OrderBy(x => x.Sequence == 0 ? int.MaxValue : x.Sequence)
                .ThenBy(x => x.Timestamp)
                .ThenBy(x => x.ID)
                .ToArray();
        }

        private DataItem ReadDataItem(FileInfo file)
        {
            var json = File.ReadAllText(file.FullName);
            return JsonConvert.DeserializeObject<DataItem>(json);
        }

        private class DataItem
        {
            public Guid ID { get; set; }
            public string Type { get; set; }
            public int Sequence { get; set; }
            public long Timestamp { get; set; }
            public string ResourceName { get; set; }
            public string ResourceUrl { get; set; }
            public long ResourceSize { get; set; }
            public string ThumbnailUrl { get; set; }
            public string CaptionText { get; set; }

            [JsonConstructor]
            private DataItem()
            {

            }

            public DataItem(string type)
            {
                ID = Guid.NewGuid();
                Type = type;
                Timestamp = DateTime.UtcNow.Ticks;
            }
        }

        #endregion
    }
}