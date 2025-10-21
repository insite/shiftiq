using System;
using System.Web.UI;

using InSite.Application.Standards.Read;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Custom.CMDS.Admin.Standards.ValidationChanges.Controls
{
    public partial class Detail : UserControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                UserIdentifier.Filter.EnableIsArchived = false;
                ValidatorUserIdentifier.Filter.EnableIsArchived = false;
            }
        }

        public bool SetInputValues(StandardValidationChange entity)
        {
            ChangePosted.Value = entity.ChangePosted;
            StandardIdentifier.Value = entity.StandardIdentifier;
            UserIdentifier.Value = entity.UserIdentifier;
            ValidatorUserIdentifier.Value = entity.AuthorUserIdentifier == UserIdentifiers.Someone
                ? null
                : entity.AuthorUserIdentifier;
            ChangeStatus.Value = entity.ChangeStatus;
            ChangeComment.Text = entity.ChangeComment;

            return true;
        }

        public void GetInputValues(QStandardValidationLog entity)
        {
            entity.LogPosted = ChangePosted.Value.Value;
            entity.StandardIdentifier = StandardIdentifier.Value.Value;
            entity.UserIdentifier = UserIdentifier.Value.Value;
            entity.AuthorUserIdentifier = ValidatorUserIdentifier.Value ?? UserIdentifiers.Someone;
            entity.LogStatus = ChangeStatus.Value;
            entity.LogComment = ChangeComment.Text;
        }
    }
}