using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.UI.Admin.Contacts.Groups.Controls
{
    public partial class WorkflowSection : UserControl
    {
        public void SetInputValues(QGroup group)
        {
            MessageToAdminWhenEventVenueChanged.Text = GetMessageTitle(group.MessageToAdminWhenEventVenueChanged);
            MessageToAdminWhenMembershipEnded.Text = GetMessageTitle(group.MessageToAdminWhenMembershipEnded);
            MessageToAdminWhenMembershipStarted.Text = GetMessageTitle(group.MessageToAdminWhenMembershipStarted);

            MessageToUserWhenMembershipEnded.Text = GetMessageTitle(group.MessageToUserWhenMembershipEnded);
            MessageToUserWhenMembershipStarted.Text = GetMessageTitle(group.MessageToUserWhenMembershipStarted);

            MandatorySurveyFormIdentifier.Text = GetMessageTitle(group.SurveyFormIdentifier);
        }

        private static string GetMessageTitle(Guid? id)
        {
            var message = id.HasValue ? ServiceLocator.MessageSearch.GetMessage(id.Value) : null;
            return (message?.MessageTitle).IfNullOrEmpty("<i>None</i>");
        }
    }
}