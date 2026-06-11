using System;

using Shift.Common;

namespace Shift.Sdk.Contract.Booking
{
    public class ExamEventValidator
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
            if (_model.ApprovalStatus == "Not Eligible")
                return "You are not eligible to write this exam.";

            if (!_model.HasEvent)
                return "This exam is not scheduled.";

            if (_model.EventFormat != EventExamFormat.Online.Value)
                return "This exam is not available online. It is available on paper only.";

            if (_model.EventSchedulingStatus == null || !_model.EventSchedulingStatus.StartsWith("Approved"))
                return "This exam event is not approved.";

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
                return $"The start time for your exam is {startText}. You can not sign in before {earlyText}.";
            }

            if (!IsTimeLimitDisabled() && now > start.AddMinutes(60))
            {
                var lateText = start.AddMinutes(60).Format(_model.UserTimeZone);
                return $"The start time for your exam is {startText}. You can not sign in after {lateText}.";
            }

            return null;
        }

        private string ValidateExamForm()
        {
            if (!_model.HasForm)
                return "There is no exam form assigned to this exam event registration.";

            if (_model.AssessmentCount == 0)
                return $"Exam Form {_model.ExamFormIdentifier.Value} must be contained in an Assessment asset.";

            if (_model.AssessmentCount > 1)
                return $"Exam Form {_model.ExamFormIdentifier.Value} is contained in {_model.AssessmentCount} Assessment assets.";

            return null;
        }

        private bool IsTimeLimitDisabled()
        {
            if (!_model.HasAccommodations)
                return false;

            return _model.HasResumeInterruptedOnlineExam;
        }
    }
}