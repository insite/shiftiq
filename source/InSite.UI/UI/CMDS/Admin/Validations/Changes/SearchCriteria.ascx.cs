using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardValidationChangeFilter>
    {
        public override StandardValidationChangeFilter Filter
        {
            get
            {
                var filter = new StandardValidationChangeFilter
                {
                    DepartmentIdentifier = DepartmentIdentifier.Value,
                    SelfAssessmentStatus = SelfAssessmentStatus.Value,
                    StandardIdentifier = StandardIdentifier.Value,
                    StandardType = StandardType.Value,
                    OrganizationIdentifier = Organization.Identifier,
                    UserIdentifier = UserIdentifier.Value,
                    ValidationStatus = ValidationStatus.Value,

                    ChangePostedSince = ChangePostedSince.Value,
                    ChangePostedBefore = ChangePostedBefore.Value,
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

                ChangePostedSince.Value = filter.ChangePostedSince;
                ChangePostedBefore.Value = filter.ChangePostedBefore;
                ValidatorUserIdentifier.Value = filter.ValidatorUserIdentifier;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;
            }
        }

        public override void Clear()
        {
            DepartmentIdentifier.Value = null;
            SelfAssessmentStatus.ClearSelection();
            StandardType.ClearSelection();
            StandardIdentifier.Value = null;
            UserIdentifier.Value = null;
            ValidationStatus.ClearSelection();

            ValidatorUserIdentifier.Value = null;
            ChangePostedSince.Value = null;
            ChangePostedBefore.Value = null;
        }
    }
}