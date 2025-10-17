using System;

using Shift.Common;
using Shift.Constant;

namespace Shift.Sdk.Contract.Booking
{
    public class ClassEventValidator
    {
        private EventValidationModel _model;

        public string Validate(EventValidationModel model)
        {
            _model = model;

            var error = ValidateRegistrationStatus()
                ?? ValidateEventSchedule()
                ?? ValidateExamForm();

            return error;
        }

        private string ValidateRegistrationStatus()
        {
            if (!StringHelper.Equals(_model.ApprovalStatus, "Registered"))
                return "You are not registered in this class.";

            if (!_model.HasEvent)
                return "This class is not scheduled.";

            var status = _model.ClassStatus;
            if (status != EventClassStatus.InProgress && status != EventClassStatus.Published)
                return "This class event is not approved.";

            return null;
        }

        private string ValidateEventSchedule()
        {
            if (_model.AllowLoginAnyTime)
                return null;

            var now = DateTimeOffset.UtcNow;
            var start = _model.EventScheduledStart;
            var startText = start.Format(_model.UserTimeZone);

            if (now < start.AddMinutes(-15))
            {
                var earlyText = start.AddMinutes(-15).Format(_model.UserTimeZone);
                return $"The start time for your class is {startText}. You can not sign in before {earlyText}.";
            }

            if (now > start.AddMinutes(60))
            {
                var lateText = start.AddMinutes(60).Format(_model.UserTimeZone);
                return $"The start time for your class is {startText}. You can not sign in after {lateText}.";
            }

            return null;
        }

        private string ValidateExamForm()
        {
            if (!_model.HasForm)
                return "There is no exam form assigned to this class event registration.";

            if (_model.AssessmentCount == 0)
                return $"Exam Form {_model.ExamFormIdentifier.Value} must be contained in an Assessment asset.";

            if (_model.AssessmentCount > 1)
                return $"Exam Form {_model.ExamFormIdentifier.Value} is contained in {_model.AssessmentCount} Assessment assets.";

            return null;
        }
    }
}