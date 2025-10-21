using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Contacts.Groups.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QGroupFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;

            MergeButton.Click += MergeButton_Click;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var parents = ((IEnumerable<GroupSearchResult.ParentInfo>)DataBinder.Eval(e.Row.DataItem, "FunctionalParents")).ToArray();

            var repeater = (Repeater)e.Row.FindControl("FunctionalParentsRepeater");
            repeater.DataSource = parents;
            repeater.DataBind();
            repeater.Visible = parents.Length != 0;

            var row = e.Row.DataItem;

            if (Filter.ShowColumns.Count == 0 || Filter.ShowColumns.Any(c => StringHelper.Equals(c, "Shipping Address")))
            {
                var shippingAddressLiteral = (ITextControl)e.Row.FindControl("ShippingAddressLiteral");
                shippingAddressLiteral.Text = LocationHelper.ToHtml(
                    DataBinder.Eval(row, "ShippingAddress.Street1") as string,
                    DataBinder.Eval(row, "ShippingAddress.Street2") as string,
                    DataBinder.Eval(row, "ShippingAddress.City") as string,
                    DataBinder.Eval(row, "ShippingAddress.Province") as string,
                    DataBinder.Eval(row, "ShippingAddress.PostalCode") as string,
                    null, null, null
                );
            }

            if (Filter.ShowColumns.Count == 0 || Filter.ShowColumns.Any(c => StringHelper.Equals(c, "Billing Address")))
            {
                var billingAddressLiteral = (ITextControl)e.Row.FindControl("BillingAddressLiteral");
                billingAddressLiteral.Text = LocationHelper.ToHtml(
                    DataBinder.Eval(row, "BillingAddress.Street1") as string,
                    DataBinder.Eval(row, "BillingAddress.Street2") as string,
                    DataBinder.Eval(row, "BillingAddress.City") as string,
                    DataBinder.Eval(row, "BillingAddress.Province") as string,
                    DataBinder.Eval(row, "BillingAddress.PostalCode") as string,
                    null, null, null
                );
            }

            if (Filter.ShowColumns.Count == 0 || Filter.ShowColumns.Any(c => StringHelper.Equals(c, "Physical Address")))
            {
                var physicalAddressLiteral = (ITextControl)e.Row.FindControl("PhysicalAddressLiteral");
                physicalAddressLiteral.Text = LocationHelper.ToHtml(
                    DataBinder.Eval(row, "PhysicalAddress.Street1") as string,
                    DataBinder.Eval(row, "PhysicalAddress.Street2") as string,
                    DataBinder.Eval(row, "PhysicalAddress.City") as string,
                    DataBinder.Eval(row, "PhysicalAddress.Province") as string,
                    DataBinder.Eval(row, "PhysicalAddress.PostalCode") as string,
                    null, null, null
                );
            }
        }

        private void MergeButton_Click(object sender, EventArgs e)
        {
            var groups = new List<Guid>();

            foreach (GridViewRow row in Grid.Rows)
            {
                var mergeCheckBox = (ICheckBoxControl)row.FindControl("MergeCheckBox");
                if (!mergeCheckBox.Checked)
                    continue;

                groups.Add(Grid.GetDataKey<Guid>(row));

                if (groups.Count == 2)
                    break;
            }

            if (groups.Count == 2)
            {
                var url = $"/ui/admin/contacts/groups/merge?group1={groups[0]}&group2={groups[1]}";
                HttpResponseHelper.Redirect(url);
            }
        }

        protected override int SelectCount(QGroupFilter filter)
        {
            var count = ServiceLocator.GroupSearch.CountGroups(filter);

            ButtonPanel.Visible = count > 0;

            return count;
        }

        protected override IListSource SelectData(QGroupFilter filter)
        {
            return ServiceLocator.GroupSearch
                .SearchGroups(filter)
                .ToSearchResult();
        }

        #region Export

        public class ExportDataItem
        {
            public Guid GroupIdentifier { get; set; }
            public string GroupName { get; set; }
            public string GroupType { get; set; }
            public string GroupTag { get; set; }
            public string GroupCode { get; set; }
            public string GroupCategory { get; set; }
            public string GroupStatus { get; set; }
            public string GroupOffice { get; set; }
            public string GroupPhone { get; set; }
            public DateTimeOffset? GroupExpiryDate { get; set; }
            public string GroupRegion { get; set; }
            public string NumberOfEmployees { get; set; }
            public int? GroupCapacity { get; set; }
            public int GroupSize { get; set; }
            public int Subgroups { get; set; }
            public int MembershipStatusSize { get; set; }

            public string ShippingAddressCity { get; set; }
            public string ShippingAddressCountry { get; set; }
            public string ShippingAddressDescription { get; set; }
            public string ShippingAddressPostalCode { get; set; }
            public string ShippingAddressProvince { get; set; }
            public string ShippingAddressStreet1 { get; set; }
            public string ShippingAddressStreet2 { get; set; }

            public string BillingAddressCity { get; set; }
            public string BillingAddressCountry { get; set; }
            public string BillingAddressDescription { get; set; }
            public string BillingAddressPostalCode { get; set; }
            public string BillingAddressProvince { get; set; }
            public string BillingAddressStreet1 { get; set; }
            public string BillingAddressStreet2 { get; set; }

            public string PhysicalAddressCity { get; set; }
            public string PhysicalAddressCountry { get; set; }
            public string PhysicalAddressDescription { get; set; }
            public string PhysicalAddressPostalCode { get; set; }
            public string PhysicalAddressProvince { get; set; }
            public string PhysicalAddressStreet1 { get; set; }
            public string PhysicalAddressStreet2 { get; set; }

            public string ParentGroupName { get; set; }
            public string AncestorsParentGroupName { get; set; }

            public string MandatorySurvey { get; set; }
            public string InvoiceProduct { get; set; }
        }

        public override IListSource GetExportData(QGroupFilter filter, bool empty)
        {
            var data = ServiceLocator.GroupSearch.ExportGroups(filter, empty);

            var result = new List<ExportDataItem>(data.Count);

            foreach (var dataItem in data)
            {
                var exportItem = new ExportDataItem
                {
                    GroupIdentifier = dataItem.Group.GroupIdentifier,
                    GroupName = dataItem.Group.GroupName,
                    GroupType = dataItem.Group.GroupType,
                    GroupTag = dataItem.Group.GroupLabel,
                    GroupCode = dataItem.Group.GroupCode,
                    GroupCategory = dataItem.Group.GroupCategory,
                    GroupStatus = TCollectionItemCache.GetName(dataItem.Group.GroupStatusItemIdentifier),
                    GroupOffice = dataItem.Group.GroupOffice,
                    GroupPhone = dataItem.Group.GroupPhone,
                    GroupExpiryDate = dataItem.Group.GroupExpiry,
                    GroupRegion = dataItem.Group.GroupRegion,
                    GroupCapacity = dataItem.Group.GroupCapacity,
                    GroupSize = dataItem.GroupSize,
                    NumberOfEmployees = dataItem.Group.GroupSize,
                    Subgroups = dataItem.Subgroups,
                    MembershipStatusSize = dataItem.MembershipStatusSize,
                    MandatorySurvey = dataItem.SurveyFormName,
                    InvoiceProduct = dataItem.MembershipProductName
                };

                if (dataItem.ShippingAddress != null)
                {
                    exportItem.ShippingAddressCity = dataItem.ShippingAddress.City;
                    exportItem.ShippingAddressCountry = dataItem.ShippingAddress.Country;
                    exportItem.ShippingAddressDescription = dataItem.ShippingAddress.Description;
                    exportItem.ShippingAddressPostalCode = dataItem.ShippingAddress.PostalCode;
                    exportItem.ShippingAddressProvince = dataItem.ShippingAddress.Province;
                    exportItem.ShippingAddressStreet1 = dataItem.ShippingAddress.Street1;
                    exportItem.ShippingAddressStreet2 = dataItem.ShippingAddress.Street2;
                }

                if (dataItem.BillingAddress != null)
                {
                    exportItem.BillingAddressCity = dataItem.BillingAddress.City;
                    exportItem.BillingAddressCountry = dataItem.BillingAddress.Country;
                    exportItem.BillingAddressDescription = dataItem.BillingAddress.Description;
                    exportItem.BillingAddressPostalCode = dataItem.BillingAddress.PostalCode;
                    exportItem.BillingAddressProvince = dataItem.BillingAddress.Province;
                    exportItem.BillingAddressStreet1 = dataItem.BillingAddress.Street1;
                    exportItem.BillingAddressStreet2 = dataItem.BillingAddress.Street2;
                }

                if (dataItem.PhysicalAddress != null)
                {
                    exportItem.PhysicalAddressCity = dataItem.PhysicalAddress.City;
                    exportItem.PhysicalAddressCountry = dataItem.PhysicalAddress.Country;
                    exportItem.PhysicalAddressDescription = dataItem.PhysicalAddress.Description;
                    exportItem.PhysicalAddressPostalCode = dataItem.PhysicalAddress.PostalCode;
                    exportItem.PhysicalAddressProvince = dataItem.PhysicalAddress.Province;
                    exportItem.PhysicalAddressStreet1 = dataItem.PhysicalAddress.Street1;
                    exportItem.PhysicalAddressStreet2 = dataItem.PhysicalAddress.Street2;
                }

                if (dataItem.Parent != null)
                {
                    exportItem.ParentGroupName = dataItem.Parent.GroupName;

                    if (dataItem.Ancestors.IsNotEmpty())
                    {
                        var ancestorsParentGroupName = new List<string>();

                        foreach (var ancestor in dataItem.Ancestors)
                        {
                            var group = ServiceLocator.GroupSearch.GetGroup(ancestor.ParentGroupIdentifier);

                            ancestorsParentGroupName.Add(group.GroupName);
                        }

                        if (ancestorsParentGroupName.Count > 0)
                            exportItem.AncestorsParentGroupName = string.Join(", ", ancestorsParentGroupName.OrderBy(y => y));
                    }
                }

                result.Add(exportItem);
            }

            return result.ToSearchResult();
        }

        #endregion

        protected bool CanWrite => CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Contacts, PermissionOperation.Write);

        protected string Humanize(DateTimeOffset value) => $"{value.Humanize()}";

        protected string Reminder(DateTime value)
        {
            return (DateTime.UtcNow - value).Days > 365
                ? "<span class='badge bg-danger'>Please Archive</span>"
                : string.Empty;
        }

        protected string TrimText(object obj)
        {
            const int maxLength = 100;

            var text = (string)obj;

            return string.IsNullOrEmpty(text) || text.Length <= maxLength
                ? text
                : text.Substring(0, 97) + "...";
        }

        protected string Localize(DateTimeOffset? date)
        {
            string expiredBadge = string.Empty;
            if (date.HasValue && date.Value < DateTimeOffset.UtcNow)
                expiredBadge = "<span class=\"badge bg-danger\">Expired</span>";

            return date.HasValue
                ? $"{ControlHelper.LocalizeDate(date, true)} {expiredBadge}"
                : string.Empty;
        }
    }
}