using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.CourseObjects;

namespace InSite.Application.Courses.Write
{
    public class CourseObjectCommandReceiver
    {
        private readonly IChangeQueue _publisher;

        public CourseObjectCommandReceiver(ICommandQueue commander, IChangeQueue publisher)
        {
            _publisher = publisher;

            commander.Subscribe<RestartCourseEnrollment>(Handle);
        }

        private void Commit(IChange @event, ICommand command)
        {
            @event.OriginOrganization = command.OriginOrganization;
            @event.OriginUser = command.OriginUser;
            _publisher.Publish(@event);
        }

        public void Handle(RestartCourseEnrollment command)
        {
            var @event = new CourseEnrollmentRestarted(command.Learner, command.Course);
            Commit(@event, command);
        }
    }
}
