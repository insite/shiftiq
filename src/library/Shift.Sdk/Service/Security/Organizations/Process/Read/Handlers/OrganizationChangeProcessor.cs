using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Application.Organizations.Read
{
    public class OrganizationChangeProcessor
    {
        private ICommander _commander;
        private IApiRequestLogger _apiRequestLogger;

        public OrganizationChangeProcessor(ICommander commander, IChangeQueue queue, IApiRequestLogger apiRequestLogger)
        {
            _commander = commander;
            _apiRequestLogger = apiRequestLogger;
        }

        private void Send(IChange cause, ICommand effect)
        {
            effect.OriginOrganization = cause.OriginOrganization;
            effect.OriginUser = cause.OriginUser;
            _commander.Send(effect);
        }
    }
}