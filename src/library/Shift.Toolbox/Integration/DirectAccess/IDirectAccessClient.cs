using System;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public interface IDirectAccessClient
    {
        // Commands:

        // HTTP POST exam/eventnotification
        AdHocEventNotificationOutput AdHocEventNotification(Guid user, string eventId, AdHocEventNotificationInput input);

        // HTTP POST exam/event
        ExamEventOutput ExamEvent(Guid user, int eventId, ExamEventInput input);

        // HTTP POST exam/eventcandidate
        ExamEventCandidateOutput ExamEventCandidate(Guid user, string eventId, string individualId, ExamEventCandidateInput input);

        // HTTP POST exam
        ExamSubmissionResponse SubmitExamData(Guid user, ExamSubmissionRequest input);


        // Queries:

        // HTTP POST individual/search
        IndividualRequestOutput IndividualRequest(Guid user, IndividualRequestInput input);

        // HTTP GET individual/verify/{IndividualId}
        void VerifyActiveIndividual(Guid user, VerificationInputVariables inputs, VerificationDisplayVariables displays);

        // HTTP GET individual/verifyprogramreg/{IndividualId}/{ExamId}
        void VerifyCorrespondingRegistration(Guid user, VerificationInputVariables inputs, VerificationDisplayVariables displays);
    }
}
