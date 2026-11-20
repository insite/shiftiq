using System;
using System.Web.UI;

using Shift.Common;

namespace InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields
{
    public partial class User : UserControl, IExperienceUserField
    {
        public string Title
        {
            get => FieldTitle.Text;
            set
            {
                FieldTitle.Text = value;
                RequiredValidator.FieldName = value;
            }
        }

        public string Help
        {
            get => HelpText.Text;
            set => HelpText.Text = value;
        }

        public bool IsRequired
        {
            get => RequiredValidator.Enabled;
            set
            {
                RequiredValidator.Visible = value;
                RequiredValidator.Enabled = value;
            }
        }

        public string ValidationGroup
        {
            get => RequiredValidator.ValidationGroup;
            set => RequiredValidator.ValidationGroup = value;
        }

        public Guid? Value
        {
            get => UserValue.Value;
            set => UserValue.Value = value;
        }

        public void RestrictToGroup(Guid? groupId)
        {
            UserValue.Filter.MembershipGroupIdentifier = groupId;
        }

        public void RestrictToUsers(Guid[] userIdentifiers)
        {
            UserValue.Filter.UserIdentifiers = userIdentifiers;
        }
    }
}
