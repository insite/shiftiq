using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Jobs.Employers.MyProfile
{
    public partial class Edit : PortalBasePage
    {
        private static readonly Guid FastDepartmentId = Guid.Parse("9FFC8191-C34D-4A9F-A834-34E3AE317292");

        private Guid? GroupIdentifier
        {
            get
            {
                if (Guid.TryParse(Request.QueryString["group"], out Guid id))
                    return id;
                return null;
            }
        }

        private List<Guid> OccupationListDataKeys
        {
            get => (List<Guid>)ViewState[nameof(OccupationListDataKeys)];
            set => ViewState[nameof(OccupationListDataKeys)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OccupationList.DataBinding += OccupationList_DataBinding;
            OccupationList.ItemDataBound += OccupationList_ItemDataBound;

            EmpIndustry.AutoPostBack = true;
            EmpIndustry.ValueChanged += EmpIndustry_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);
            BindModelToControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            EmpIndustryDetails.Attributes["style"] = string.Equals(EmpIndustry.Value, "Other", StringComparison.OrdinalIgnoreCase)
                ? null
                : "display:none";
        }

        private void OccupationList_DataBinding(object sender, EventArgs e)
        {
            OccupationListDataKeys = new List<Guid>();
        }

        private void OccupationList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var standardId = (Guid)DataBinder.Eval(e.Item.DataItem, "Value");

            OccupationListDataKeys.Add(standardId);
        }

        private void EmpIndustry_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            SetControlsVisibility();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var person = PersonSearch.Select(Organization.Identifier, User.Identifier);

            if (!ValidatePerson(person))
                return;

            var group = GroupIdentifier ?? person.EmployerGroupIdentifier;
            if (group == null)
                return;

            var companyAddress = new GroupAddress
            {
                Street1 = EmpAddressLine.Text,
                City = EmpAddressCity.Text,
                Province = EmpAddressProvince.Value,
                PostalCode = EmpAddressPostalCode.Text,
                Country = ServiceLocator.CountrySearch.SelectById(CountrySelector.Value)?.Name ?? string.Empty
            };

            var employer = ServiceLocator.GroupSearch.GetGroup(group.Value);
            var id = employer.GroupIdentifier;
            var industryComment = EmpIndustry.Value == "Other" ? EmpIndustryDetails.Text : null;

            var commands = new List<Command>
            {
                new ChangeGroupAddress(id, AddressType.Physical, companyAddress),
                new RenameGroup(id, employer.GroupType, EmpCompanyName.Text),
                new ChangeGroupIndustry(id, EmpIndustry.Value, industryComment),
                new ChangeGroupSize(id, EmpNumber.Value),
                new ChangeGroupPhone(id, EmpPhone.Text),
                new ChangeGroupWebSiteUrl(id, EmpWebsite.Text),
                new DescribeGroup(id, employer.GroupCategory, employer.GroupCode, GroupDescription.Text, employer.GroupLabel),
                new ChangeGroupEmail(id, GroupEmail.Text)
            };

            commands.Add(new ChangeGroupSocialMediaUrl(id, "Facebook", EmpSocialFacebook.Text));
            commands.Add(new ChangeGroupSocialMediaUrl(id, "Instagram", EmpSocialInstagram.Text));
            commands.Add(new ChangeGroupSocialMediaUrl(id, "Twitter", EmpSocialTwitter.Text));
            commands.Add(new ChangeGroupSocialMediaUrl(id, "YouTube", EmpSocialYouTube.Text));

            SaveTagList(id, commands);

            ServiceLocator.SendCommands(commands);

            SetOccupations(employer.GroupIdentifier);

            BindModelToControls();

            var toast = ToastItem.UrlEncode(AlertType.Success, "fas fa-check-circle", "Success", "Your changes are saved.");
            HttpResponseHelper.Redirect($"/ui/portal/job/employers/profile/view?group={employer.GroupIdentifier}&toast={toast}");
        }

        private void BindModelToControls()
        {
            var person = PersonSearch.Select(Organization.Identifier, User.Identifier);

            if (!ValidatePerson(person))
                return;

            EmpIndustry.RefreshData();

            var group = GroupIdentifier ?? person.EmployerGroupIdentifier;
            if (group == null)
                return;

            var employer = ServiceLocator.GroupSearch.GetGroup(group.Value);
            var address = ServiceLocator.GroupSearch.GetAddress(employer.GroupIdentifier, AddressType.Physical);

            var companyHeading = (employer.GroupLabel ?? "Site") + " Information";
            var employerHeading = (employer.GroupLabel ?? "Site") + " Name";

            CompanyHeading.InnerText = companyHeading;
            EmployerLabel.InnerText = employerHeading;

            EmpCompanyName.Text = employer.GroupName;
            EmpIndustry.Value = employer.GroupIndustry;
            EmpIndustryDetails.Text = employer.GroupIndustryComment;
            GroupDescription.Text = employer.GroupDescription;
            EmpNumber.Value = employer.GroupSize;

            EmpPhone.Text = employer.GroupPhone;

            EmpAddressLine.Text = address?.Street1;
            EmpAddressCity.Text = address?.City;
            EmpAddressProvince.Value = address?.Province;
            EmpAddressPostalCode.Text = address?.PostalCode;

            var countryName = (address.Country != null ? address.Country : (CurrentSessionState.Identity.Organization.PlatformCustomization.TenantLocation.Country ?? "Canada"));
            CountrySelector.Value = ServiceLocator.CountrySearch.SelectByName(countryName)?.Identifier;

            EmpWebsite.Text = employer.GroupWebSiteUrl;

            BindSocialMediaLinks(employer);

            GroupEmail.Text = employer.GroupEmail;

            BindTagList(employer.GroupIdentifier);

            OccupationList.DataSource = GetOccupations(employer.GroupIdentifier);
            OccupationList.DataBind();

            SetControlsVisibility();

            CancelButton.NavigateUrl = $"/ui/portal/job/employers/profile/view?group={employer.GroupIdentifier}";
        }

        private void BindSocialMediaLinks(QGroup group)
        {
            var socialMediaUrls = QGroupStore.GetSocialMediaUrls(group);

            if (socialMediaUrls.TryGetValue("Facebook", out var facebookUrl))
                EmpSocialFacebook.Text = facebookUrl;

            if (socialMediaUrls.TryGetValue("Instagram", out var instagramUrl))
                EmpSocialInstagram.Text = instagramUrl;

            if (socialMediaUrls.TryGetValue("Twitter", out var twitterUrl))
                EmpSocialTwitter.Text = twitterUrl;

            if (socialMediaUrls.TryGetValue("YouTube", out var youtubeUrl))
                EmpSocialYouTube.Text = youtubeUrl;
        }

        private void SetControlsVisibility()
        {
            var isSeniorsCare = string.Equals(EmpIndustry.Value, "Seniors Care", StringComparison.OrdinalIgnoreCase);

            GroupTagsField.Visible = isSeniorsCare;
            OccupationsField.Visible = isSeniorsCare && OccupationList.Items.Count > 0;
        }

        private bool ValidatePerson(Person person)
        {
            if (person.EmployerGroupIdentifier == null)
            {
                StatusAlert.AddMessage(AlertType.Error, "No employer is selected for your account. Please contact your administrator to resolve this.");
                EditDetail.Visible = false;
                return false;
            }

            if (person.JobsApproved == null)
            {
                StatusAlert.AddMessage(AlertType.Warning, "This account is still <strong>pending approval</strong> by Admin.<br />" +
                    "Once approved you can perform more detailed employee searches and more.");
            }

            return true;
        }

        private void BindTagList(Guid groupId)
        {
            var selectedTags = ServiceLocator.GroupSearch.GetGroupTags(groupId);
            var allTags = TCollectionItemCache
                .Query(new TCollectionItemFilter 
                { 
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    CollectionName = CollectionName.Contacts_Groups_Classification_Tag
                })
                .Select(x => new
                {
                    x.ItemName,
                    IsSelected = selectedTags.Any(y => string.Equals(y, x.ItemName, StringComparison.OrdinalIgnoreCase))
                })
                .ToList();

            GroupTagsField.Visible = allTags.Count > 0;

            GroupTagList.DataSource = allTags;
            GroupTagList.DataBind();
        }

        private void SaveTagList(Guid groupId, List<Command> commands)
        {
            foreach (RepeaterItem item in GroupTagList.Items)
            {
                var isSelected = (ICheckBox)item.FindControl("IsSelected");

                if (isSelected.Checked)
                    commands.Add(new AddGroupTag(groupId, isSelected.Text));
                else
                    commands.Add(new RemoveGroupTag(groupId, isSelected.Text));
            }
        }

        public static List<OccupationItem> GetOccupations(Guid employerGroupId)
        {
            var standards = StandardSearch.Bind(
                x => new
                {
                    x.StandardIdentifier,
                    x.ContentTitle,
                    x.ContentName,
                    IsSelected = x.Departments.Any(y => y.DepartmentIdentifier == employerGroupId)
                },
                x => x.OrganizationIdentifier == Organization.Identifier
                  && x.StandardType == StandardType.Profile
                  && x.StandardLabel == "Occupation"
                  && x.DepartmentGroupIdentifier == FastDepartmentId
            );

            var list = standards
                .Select(x => new OccupationItem
                {
                    Value = x.StandardIdentifier,
                    Text = x.ContentTitle.IfNullOrEmpty(x.ContentName),
                    IsSelected = x.IsSelected
                })
                .OrderBy(x => x.Text)
                .ToList();

            foreach (var item in list)
            {
                var popover = GetPopover(item.Value);
                item.Html = popover != null ? $"{item.Text} {popover.Html}" : item.Text;
            }

            return list;
        }

        private static Popover GetPopover(Guid value)
        {
            var standard = StandardSearch.Bind(value, x => new { x.ContentName, x.ContentTitle, x.StandardAlias, x.ContentSummary });
            if (string.IsNullOrWhiteSpace(standard.StandardAlias) && string.IsNullOrWhiteSpace(standard.ContentSummary))
                return null;

            var body = string.Empty;
            if (standard.ContentSummary != null)
                body = standard.ContentSummary.Replace("'", "");
            else
                body = "-";

            var title = standard.ContentTitle ?? standard.ContentName;
            if (!string.IsNullOrWhiteSpace(standard.StandardAlias))
                title = standard.StandardAlias;

            return new Popover
            {
                Title = title,
                Body = body.ToString()
            };
        }

        public class OccupationItem
        {
            public Guid Value { get; set; }
            public string Text { get; set; }
            public string Html { get; set; }
            public bool IsSelected { get; set; }
        }

        private class Popover
        {
            public string Title { get; set; }
            public string Body { get; set; }
            public string Html
                => $"<i class='far fa-question-circle' data-bs-container='body' data-bs-toggle='popover' data-bs-placement='bottom' data-bs-trigger='hover' title='{Title}' data-bs-content='{Body}'></i>";
        }

        private void SetOccupations(Guid employerGroupId)
        {
            var insert = new List<TDepartmentStandard>();
            var delete = new List<TDepartmentStandard>();
            var connections = TDepartmentStandardSearch
                .Bind(x => x, x => x.DepartmentIdentifier == employerGroupId && OccupationListDataKeys.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.StandardIdentifier);

            for (var i = 0; i < OccupationList.Items.Count; i++)
            {
                var item = OccupationList.Items[i];
                var standardId = OccupationListDataKeys[i];
                var hasConnection = connections.TryGetValue(standardId, out var entity);

                var isSelected = (ICheckBox)item.FindControl("IsSelected");

                if (isSelected.Checked && !hasConnection)
                    insert.Add(new TDepartmentStandard
                    {
                        DepartmentIdentifier = employerGroupId,
                        StandardIdentifier = standardId
                    });
                else if (!isSelected.Checked && hasConnection)
                    delete.Add(entity);
            }

            TDepartmentStandardStore.Insert(insert);
            TDepartmentStandardStore.Delete(delete);
        }
    }
}