using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Reports.Criteria
{
    public partial class MultipleDepartmentSelector : UserControl
    {
        #region DepartmentItem class

        private class DepartmentItem
        {
            public Guid DepartmentIdentifier { get; set; }
            public String DepartmentName { get; set; }
        }

        #endregion

        #region DepartmentDistrictItem class

        private class DepartmentDivisionItem
        {
            private List<DepartmentItem> _departments = new List<DepartmentItem>();

            public Guid? DivisionIdentifier { get; set; }
            public String DivisionName { get; set; }
            public List<DepartmentItem> Departments { get { return _departments; } }
        }

        #endregion

        #region Properties

        public String OnClientItemsChanged
        {
            get { return OnClientItemsChangedField.Value; }
            set { OnClientItemsChangedField.Value = value; }
        }

        protected Boolean EnableDistricts { get; set; }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentDistricts.ItemDataBound += DepartmentDistricts_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            InitDepartmentItems();
        }

        #endregion

        #region Event handlers

        private void DepartmentDistricts_ItemDataBound(Object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.AlternatingItem && e.Item.ItemType != ListItemType.Item)
                return;

            DepartmentDivisionItem district = (DepartmentDivisionItem)e.Item.DataItem;

            Repeater Departments = (Repeater)e.Item.FindControl("Departments");
            Departments.DataSource = district.Departments;
            Departments.DataBind();
        }

        #endregion

        #region Public methods

        public void LoadData(Guid organizationId, string departmentLabel)
        {
            var organization = OrganizationSearch.Select(organizationId);

            EnableDistricts = OrganizationHelper.EnableDivisions(organization.CompanyDescription.CompanySize);

            var filter = new DepartmentFilter();
            filter.OrganizationIdentifier = organizationId;
            filter.DepartmentLabel = departmentLabel;

            if (!CurrentSessionState.Identity.HasAccessToAllCompanies)
                filter.UserIdentifier = CurrentSessionState.Identity.User.UserIdentifier;

            var sortExpression = EnableDistricts
                ? "CASE WHEN DivisionIdentifier IS NULL THEN 1 ELSE 0 END, DivisionName, DepartmentName"
                : "DepartmentName";

            var departments = ContactRepository3.SelectDepartments(filter, sortExpression);

            DepartmentDistricts.DataSource = EnableDistricts ? GetDepartmentDivisions(departments) : GetDepartments(departments);
            DepartmentDistricts.DataBind();
        }

        public Guid GetDepartmentID(Int32 districtIndex, Int32 departmentIndex)
        {
            Repeater departmentsRepeater = (Repeater)DepartmentDistricts.Items[districtIndex].FindControl("Departments");
            RepeaterItem departmentItem = departmentsRepeater.Items[departmentIndex];
            Literal departmentIdLabel = (Literal)departmentItem.FindControl("DepartmentIdentifier");

            return Guid.Parse(departmentIdLabel.Text);
        }

        public string GetSelectedDepartmentsText()
        {
            var departments = GetSelectedDepartments();

            if (departments == null)
                return null;

            var list = new StringBuilder();

            foreach (Guid department in departments)
            {
                if (list.Length > 0)
                    list.Append(",");

                list.Append(department.ToString());
            }

            return list.Length > 0 ? list.ToString() : null;
        }

        public Guid[] GetSelectedDepartments()
        {
            var list = new List<Guid>();

            foreach (RepeaterItem districtItem in DepartmentDistricts.Items)
            {
                var departments = (Repeater)districtItem.FindControl("Departments");

                foreach (RepeaterItem departmentItem in departments.Items)
                {
                    var chk = (CheckBox)departmentItem.FindControl("IsSelected");

                    if (!chk.Checked)
                        continue;

                    var departmentIdCtrl = (Literal)departmentItem.FindControl("DepartmentIdentifier");

                    list.Add(Guid.Parse(departmentIdCtrl.Text));
                }
            }

            return list.Count > 0 ? list.ToArray() : null;
        }

        public string GetAllDepartmentsText()
        {
            var departments = GetAllDepartments();

            if (departments == null)
                return null;

            var list = new StringBuilder();

            foreach (Guid department in departments)
            {
                if (list.Length > 0)
                    list.Append(",");

                list.Append(department.ToString());
            }

            return list.Length > 0 ? list.ToString() : null;
        }

        public Guid[] GetAllDepartments()
        {
            var list = new List<Guid>();

            foreach (RepeaterItem districtItem in DepartmentDistricts.Items)
            {
                var departments = (Repeater)districtItem.FindControl("Departments");

                foreach (RepeaterItem departmentItem in departments.Items)
                {
                    var departmentIdCtrl = (Literal)departmentItem.FindControl("DepartmentIdentifier");

                    list.Add(Guid.Parse(departmentIdCtrl.Text));
                }
            }

            return list.Count > 0 ? list.ToArray() : null;
        }

        #endregion

        #region Load data

        private DepartmentDivisionItem[] GetDepartments(DataTable departments)
        {
            var district = new DepartmentDivisionItem();

            foreach (DataRow row in departments.Rows)
            {
                district.Departments.Add(new DepartmentItem
                {
                    DepartmentIdentifier = (Guid)row["DepartmentIdentifier"],
                    DepartmentName = (string)row["DepartmentName"]
                });
            }

            return new DepartmentDivisionItem[] { district };
        }

        private DepartmentDivisionItem[] GetDepartmentDivisions(DataTable departments)
        {
            List<DepartmentDivisionItem> divisions = new List<DepartmentDivisionItem>();

            foreach (DataRow row in departments.Rows)
            {
                var division = divisions.Count > 0 ? divisions[divisions.Count - 1] : null;
                var divisionKey = row["DivisionIdentifier"] as Guid?;

                if (division == null || division.DivisionIdentifier != divisionKey)
                {
                    divisions.Add(division = new DepartmentDivisionItem
                    {
                        DivisionIdentifier = divisionKey,
                        DivisionName = (row["DivisionName"] as string) ?? "Departments Not Assigned to a Division",
                    });
                }

                division.Departments.Add(new DepartmentItem
                {
                    DepartmentIdentifier = (Guid)row["DepartmentIdentifier"],
                    DepartmentName = (string)row["DepartmentName"],
                });
            }

            return divisions.ToArray();
        }

        private void InitDepartmentItems()
        {
            for (Int32 districtIndex = 0; districtIndex < DepartmentDistricts.Items.Count; districtIndex++)
            {
                CheckBox chkDistrict = (CheckBox)DepartmentDistricts.Items[districtIndex].FindControl("IsSelected");
                chkDistrict.Attributes["onclick"] = "MultipleDepartmentSelector_Districts_onclick(this)";

                Repeater departments = (Repeater)DepartmentDistricts.Items[districtIndex].FindControl("Departments");

                for (Int32 departmentIndex = 0; departmentIndex < departments.Items.Count; departmentIndex++)
                {
                    CheckBox chk = (CheckBox)departments.Items[departmentIndex].FindControl("IsSelected");
                    chk.Attributes["index"] = districtIndex + "|" + departmentIndex;
                    chk.Attributes["onclick"] = "MultipleDepartmentSelector_Departments_onclick()";
                }
            }
        }

        #endregion
    }
}