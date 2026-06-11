using Shift.Common;
using Shift.Constant;
using Shift.Sdk.Contract.Booking;

namespace Shift.Test.Booking
{
    public class ValidationTests
    {
        private static readonly EventValidationModel _examSuccess = new EventValidationModel
        {
            ApprovalStatus = "Eligible",
            HasEvent = true,
            EventFormat = EventExamFormat.Online.Value,
            EventSchedulingStatus = "Approved",
            EventScheduledStart = DateTimeOffset.UtcNow,
            ExamFormIdentifier = Guid.NewGuid(),
            HasForm = true,
            AssessmentCount = 1,
            HasAccommodations = false,
            HasResumeInterruptedOnlineExam = false,
            UserTimeZone = "Mountain Standard Time"
        };

        private static readonly EventValidationModel _classSuccess = new EventValidationModel
        {
            ApprovalStatus = "Registered",
            HasEvent = true,
            EventScheduledStart = DateTimeOffset.UtcNow,
            ExamFormIdentifier = Guid.NewGuid(),
            HasForm = true,
            AssessmentCount = 1,
            UserTimeZone = "Mountain Standard Time",
            ClassStatus = EventClassStatus.Published
        };

        [Fact]
        public void ValidateExam_Success()
        {
            var error = new ExamEventValidator().Validate(_examSuccess);
            Assert.True(string.IsNullOrEmpty(error));
        }

        [Fact]
        public void ValidateExam_Fail_NotEligible()
        {
            var clone = _examSuccess.Clone();
            clone.ApprovalStatus = "Not Eligible";

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not eligible"));
        }

        [Fact]
        public void ValidateExam_Fail_NoEvent()
        {
            var clone = _examSuccess.Clone();
            clone.HasEvent = false;

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not scheduled"));
        }

        [Fact]
        public void ValidateExam_Fail_WrongFormat()
        {
            var clone = _examSuccess.Clone();
            clone.EventFormat = null;

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not available"));
        }

        [Fact]
        public void ValidateExam_Fail_WrongSchedulingStatus()
        {
            var clone = _examSuccess.Clone();
            clone.EventSchedulingStatus = null;

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not approved"));
        }

        [Fact]
        public void ValidateExam_Fail_WrongStart()
        {
            var clone = _examSuccess.Clone();
            clone.EventScheduledStart = DateTimeOffset.UtcNow.AddMinutes(30);

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "can not sign in before"));
        }

        [Fact]
        public void ValidateExam_Fail_NoForm()
        {
            var clone = _examSuccess.Clone();
            clone.HasForm = false;

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "no exam form"));
        }

        [Fact]
        public void ValidateExam_Fail_WrongAssessmentCount()
        {
            var clone = _examSuccess.Clone();
            clone.AssessmentCount = 0;

            var error = new ExamEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "must be contained"));
        }

        [Fact]
        public void ValidateClass_Success()
        {
            var error = new ClassEventValidator().Validate(_classSuccess);
            Assert.True(string.IsNullOrEmpty(error));
        }

        [Fact]
        public void ValidateClass_Fail_WrongClassStatus()
        {
            var clone = _classSuccess.Clone();
            clone.ClassStatus = EventClassStatus.Completed;

            var error = new ClassEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not approved"));
        }

        [Fact]
        public void ValidateClass_Fail_NoEvent()
        {
            var clone = _classSuccess.Clone();
            clone.HasEvent = false;

            var error = new ClassEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "not scheduled"));
        }

        [Fact]
        public void ValidateClass_Fail_WrongStart()
        {
            var clone = _classSuccess.Clone();
            clone.EventScheduledStart = DateTimeOffset.UtcNow.AddMinutes(30);

            var error = new ClassEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "can not sign in before"));
        }

        [Fact]
        public void ValidateClass_Fail_NoForm()
        {
            var clone = _classSuccess.Clone();
            clone.HasForm = false;

            var error = new ClassEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "no exam form"));
        }

        [Fact]
        public void ValidateClass_Fail_WrongAssessmentCount()
        {
            var clone = _classSuccess.Clone();
            clone.AssessmentCount = 0;

            var error = new ClassEventValidator().Validate(clone);
            Assert.True(ErrorContains(error, "must be contained"));
        }

        private static bool ErrorContains(string error, string keyword)
            => !string.IsNullOrEmpty(error) && error.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
