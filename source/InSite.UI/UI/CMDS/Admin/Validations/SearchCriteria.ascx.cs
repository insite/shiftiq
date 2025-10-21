using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardValidationFilter>
    {
        public override StandardValidationFilter Filter
        {
            get
            {
                var filter = new StandardValidationFilter
                {
                    DepartmentIdentifier = DepartmentIdentifier.Value,
                    SelfAssessmentStatus = SelfAssessmentStatus.Value,
                    StandardIdentifier = StandardIdentifier.Value,
                    StandardType = StandardType.Value,
                    OrganizationIdentifier = Organization.Identifier,
                    UserIdentifier = UserIdentifier.Value,
                    ValidationStatus = ValidationStatus.Value,
                    ValidatorUserIdentifier = ValidatorUserIdentifier.Value
                };

                return filter;
            }
            set
            {
                var filter = value;

                DepartmentIdentifier.Value = filter.DepartmentIdentifier;

                SelfAssessmentStatus.Value = filter.SelfAssessmentStatus;

                StandardType.Value = filter.StandardType;
                StandardIdentifier.Value = filter.StandardIdentifier;

                UserIdentifier.Value = filter.UserIdentifier;

                ValidationStatus.Value = filter.ValidationStatus;

                ValidatorUserIdentifier.Value = filter.ValidatorUserIdentifier;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;

            StandardType.AutoPostBack = true;
            StandardType.ValueChanged += StandardType_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                BindComboBoxes();
        }

        private void StandardType_ValueChanged(object sender, EventArgs e)
        {
            BindComboBoxes();
        }

        private void BindComboBoxes()
        {
            StandardIdentifier.Filter.OrganizationIdentifier = OrganizationIdentifiers.CMDS;
            if (StandardType.Value.IsNotEmpty())
                StandardIdentifier.Filter.StandardTypes = new string[] { StandardType.Value };
            else
                StandardIdentifier.Filter.StandardTypes = null;
        }

        public override void Clear()
        {
            StandardType.ClearSelection();
            StandardIdentifier.Value = null;
            DepartmentIdentifier.Value = null;
            UserIdentifier.Value = null;
            ValidatorUserIdentifier.Value = null;

            SelfAssessmentStatus.ClearSelection();

            ValidationStatus.ClearSelection();
        }
    }
}