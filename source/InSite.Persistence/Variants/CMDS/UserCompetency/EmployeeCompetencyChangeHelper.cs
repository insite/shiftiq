using System;
using System.Collections.Generic;
using System.Data;

using Shift.Common.Timeline.Commands;

using InSite.Application.StandardValidations.Write;

using Shift.Common;

using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class EmployeeCompetencyChangeHelper
    {
        private const string ExpiredComment = "This is a time-sensitive competency that has expired";

        public static int CreateChanges(OrganizationExpirationType expirationType, Action<ICommand> sendCommand)
        {
            var table = UserCompetencyRepository.SelectCompetenciesThatNeedToBeExpired(expirationType, DateTimeOffset.UtcNow);
            var processed = new HashSet<(Guid, Guid)>();

            foreach (DataRow row in table.Rows)
            {
                var item = (StandardId: (Guid)row["CompetencyStandardIdentifier"], UserId: (Guid)row["UserIdentifier"]);
                if (processed.Contains(item))
                    continue;

                processed.Add(item);

                var competency = StandardValidationSearch.SelectFirst(x => x.UserIdentifier == item.UserId && x.StandardIdentifier == item.StandardId);
                if (competency != null)
                    sendCommand(new ExpireStandardValidation(competency.ValidationIdentifier, UniqueIdentifier.Create(), ExpiredComment));
            }

            return processed.Count;
        }
    }
}