using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Admin.Jobs.Employers.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VGroupEmployerFilter>
    {
        #region Classes

        public class ExportDataItem
        {
            public Guid GroupIdentifier { get; set; }
            public string GroupName { get; set; }
            public string GroupCode { get; set; }
            public string GroupIndustry { get; set; }
            public string GroupPhone { get; set; }
            public string GroupSize { get; set; }
            public string GroupWebsite { get; set; }
            public DateTimeOffset? GroupCreated { get; set; }
            public Guid? EmployerContactIdentifier { get; set; }
            public string EmployerContactEmail { get; set; }
            public string EmployerContactFullName { get; set; }
            public string EmployerContactPhone { get; set; }
            public string PhysicalAddressCity { get; set; }
            public string PhysicalAddressCountry { get; set; }
            public string PhysicalAddressLine { get; set; }
            public string PhysicalAddressPostalCode { get; set; }
            public string PhysicalAddressProvince { get; set; }
            public DateTimeOffset? Approved { get; set; }
            public DateTimeOffset? LastChangeTime { get; set; }
            public string LastChangeType { get; set; }
            public string LastChangeUser { get; set; }

        }


        #endregion

        #region Public

        public List<int> SelectedItems
        {
            get
            {
                if (ViewState[nameof(SelectedItems)] == null) ViewState[nameof(SelectedItems)] = new List<int>();

                return ViewState[nameof(SelectedItems)] as List<int>;
            }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        #endregion

        #region Search results

        protected override int SelectCount(VGroupEmployerFilter filter)
        {
            return VGroupEmployerSearch.Count(filter);
        }

        protected override IListSource SelectData(VGroupEmployerFilter filter)
        {
            return VGroupEmployerSearch.Select(filter);
        }

        #endregion

        #region Export

        public override IListSource GetExportData(VGroupEmployerFilter filter, bool empty)
        {
            return SelectData(filter).GetList().Cast<VGroupEmployer>().Select(x => new ExportDataItem
            {
                GroupIdentifier = x.GroupIdentifier,
                GroupName = x.GroupName,
                EmployerContactIdentifier = x.UserIdentifier,
                EmployerContactFullName = x.ContactFullName,
                Approved = x.Approved,
                EmployerContactEmail = x.Email,
                EmployerContactPhone = x.Phone1,
                GroupCode = x.GroupCode,
                GroupCreated = x.GroupCreated,
                GroupIndustry = x.GroupIndustry,
                GroupPhone = x.GroupPhone,
                GroupSize = x.GroupSize,
                LastChangeTime = x.LastChangeTime,
                LastChangeType = x.LastChangeType,
                LastChangeUser = x.LastChangeUser,
                PhysicalAddressCity = x.AddressCity,
                PhysicalAddressCountry = x.AddressCountry,
                PhysicalAddressLine = x.AddressLine,
                PhysicalAddressPostalCode = x.AddressPostalCode,
                PhysicalAddressProvince = x.AddressProvince,
                GroupWebsite = x.Url

            }).ToList().ToSearchResult();
        }

        #endregion

        #region Helper methods

        protected string CreateMailToLink(object email) =>
            !ValueConverter.IsNull(email) ? $"mailto:{email}" : null;

        protected string GetString(DateTimeOffset? dateTime)
        {
            return dateTime.Format(User.TimeZone);
        }

        protected string BitToHTML(object item)
        {
            var result = (bool)item;

            if (result)
                return "Yes";
            return "No";
        }

        protected string DateToBitToHTML(object item)
        {
            var result = (DateTimeOffset?)item;

            if (result == null)
                return "No";
            return "Yes";
        }

        protected string GetEditLink(object groupId)
            => new ReturnUrl("").GetRedirectUrl($"/ui/admin/contacts/groups/edit?contact={groupId}");

        #endregion
    }
}