using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Write;

namespace InSite.Persistence
{
    public static class StandardOrganizationStore
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        #region INSERT

        public static void Insert(StandardOrganization entity)
        {
            _sendCommand(new AddStandardOrganization(entity.StandardIdentifier, entity.OrganizationIdentifier));
        }

        public static void Insert(Guid organizationId, IEnumerable<Guid> standardIds)
        {
            foreach (var standardId in standardIds)
                _sendCommand(new AddStandardOrganization(standardId, organizationId));
        }

        #endregion

        #region DELETE

        public static void Delete(Guid organizationId, IEnumerable<Guid> standardIds)
        {
            foreach (var standardId in standardIds)
                _sendCommand(new RemoveStandardOrganization(standardId, organizationId));
        }

        public static void DeleteByAssetId(Guid standardId)
        {
            Guid[] organizationIds;
            using (var db = new InternalDbContext())
                organizationIds = db.QStandardOrganizations
                    .Where(x => x.StandardIdentifier == standardId)
                    .Select(x => x.OrganizationIdentifier)
                    .ToArray();

            if (organizationIds.Length > 0)
                _sendCommand(new RemoveStandardOrganization(standardId, organizationIds));
        }

        #endregion
    }
}
