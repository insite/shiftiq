using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<TProgramFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!IsPostBack)
            {
                DepartmentIdentifier.Filter.OrganizationIdentifier = Organization.Identifier;
                DepartmentIdentifier.Value = null;
            }
        }

        public override TProgramFilter Filter
        {
            get
            {
                var filter = new TProgramFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    ProgramCode = ProgramCode.Text,
                    ProgramName = ProgramName.Text,
                    ProgramDescription = ProgramDescription.Text,
                    GroupIdentifier = DepartmentIdentifier.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ProgramCode.Text = value.ProgramCode;
                ProgramName.Text = value.ProgramName;
                ProgramDescription.Text = value.ProgramDescription;
                DepartmentIdentifier.Value = value.GroupIdentifier;
            }
        }

        public override void Clear()
        {
            ProgramCode.Text = null;
            ProgramName.Text = null;
            ProgramDescription.Text = null;
            DepartmentIdentifier.Value = null;
        }
    }
}