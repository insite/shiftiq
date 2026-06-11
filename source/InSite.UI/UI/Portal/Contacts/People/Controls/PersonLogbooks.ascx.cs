using System;
using System.Linq;
using System.Web.UI;

using InSite.Application.Records.Read;

using Shift.Constant;

namespace InSite.UI.Portal.Contacts.People.Controls
{
    public partial class PersonLogbooks : UserControl
    {
        protected bool IsHoursColumnVisible => CurrentSessionState.Identity.Organization.Toolkits.Logbooks.DisplayTotalLogbookHours;

        public int LoadData(Guid organiztionId, Guid userIdentifier)
        {
            var filter = new VJournalSetupUserFilter
            {
                OrganizationIdentifier = organiztionId,
                UserIdentifier = userIdentifier,
                Role = JournalSetupUserRole.Learner,
                OrderBy = "JournalCreated"
            };

            var data = ServiceLocator.JournalSearch
                .GetJournalSetupUsersExtended(filter)
                .Select(x => new
                {
                    JournalSetupName = x.JournalSetupName,
                    EntryCount = x.ExperienceCount,
                    HourSum = x.HourSum,
                })
                .ToList();

            NoLogbooks.Visible = data.Count == 0;

            LogbookRepeater.Visible = data.Count > 0;
            LogbookRepeater.DataSource = data;
            LogbookRepeater.DataBind();

            return data.Count;
        }
    }
}
