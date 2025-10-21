using System;
using System.Web.UI;

using InSite.Application.Standards.Read;
using InSite.Persistence;

namespace InSite.Custom.CMDS.Admin.Standards.Validations.Controls
{
    public partial class Detail : UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                UserIdentifier.Filter.EnableIsArchived = false;
        }

        public void SetDefaultInputValues()
        {
            StandardIdentifier.Filter.OrganizationIdentifier = null;
        }

        public bool SetInputValues(StandardValidation entity)
        {
            if (!entity.User.AccessGrantedToCmds)
                return false;

            StandardIdentifier.Filter.OrganizationIdentifier = null;

            StandardIdentifier.Value = entity.StandardIdentifier;
            UserIdentifier.Value = entity.UserIdentifier;
            Expired.Value = entity.Expired;
            Notified.Value = entity.Notified;
            SelfAssessmentDate.Value = entity.SelfAssessmentDate;
            SelfAssessmentStatus.Value = entity.SelfAssessmentStatus;
            ValidationComment.Text = entity.ValidationComment;
            ValidatorUserIdentifier.Value = entity.ValidatorUserIdentifier;
            ValidationDate.Value = entity.ValidationDate;
            ValidationStatus.Value = entity.ValidationStatus;

            IsValidated.Checked = entity.IsValidated;

            return true;
        }

        public void GetInputValues(QStandardValidation entity)
        {
            entity.StandardIdentifier = StandardIdentifier.Value.Value;
            entity.UserIdentifier = UserIdentifier.Value.Value;
            entity.Expired = Expired.Value;
            entity.Notified = Notified.Value;
            entity.SelfAssessmentDate = SelfAssessmentDate.Value;
            entity.SelfAssessmentStatus = SelfAssessmentStatus.Value;
            entity.ValidationComment = ValidationComment.Text;
            entity.ValidatorUserIdentifier = ValidatorUserIdentifier.Value;
            entity.ValidationDate = ValidationDate.Value;
            entity.ValidationStatus = ValidationStatus.Value;

            entity.IsValidated = IsValidated.Checked;
        }
    }
}