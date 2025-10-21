using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Contacts.Reports.Employees.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<EmployeeFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SetContentLabelHeaders(e);

            if (!IsContentItem(e.Row))
                return;

            var row = (EmployeeRepository.EmployeeRow)e.Row.DataItem;

            var employerParentFunctional = (Repeater)e.Row.FindControl("EmployerParentFunctional");
            employerParentFunctional.DataSource = row.EmployerParentFunctionalList;
            employerParentFunctional.DataBind();
        }

        protected override int SelectCount(EmployeeFilter filter)
        {
            return EmployeeRepository.CountByEmployeeFilter(filter);
        }

        protected override IListSource SelectData(EmployeeFilter filter)
        {
            return EmployeeRepository.SelectByEmployeeFilter(filter);
        }

        protected string GetDateString(DateTime? date)
        {
            if (date.HasValue)
                return TimeZones.Format(date.Value, User.TimeZone, true);

            return string.Empty;
        }

        protected string GetAddressString(string street, string city, string province, string postcode, string country)
        {
            var list = new List<string>();

            if (street.HasValue()) list.Add(street);
            if (city.HasValue()) list.Add(city);
            if (province.HasValue()) list.Add(province);
            if (postcode.HasValue()) list.Add(postcode);
            if (country.HasValue()) list.Add(country);

            if (list.Count > 0) return string.Join(", ", list);
            
            return null;
        }

        private void SetContentLabelHeaders(GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
        }
    }
}