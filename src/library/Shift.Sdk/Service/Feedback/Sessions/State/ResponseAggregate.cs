using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new ResponseState();

        public ResponseState Data => (ResponseState)State;

        public ResponseSession Session => Data.Session;

        public void CreateResponse(string source, Guid organization, Guid form, Guid user)
        {
            Apply(new ResponseSessionCreated(source, organization, form, user));
        }

        public void AddResponseAnswer(Guid question)
        {
            if (Session.ContainsAnswer(question))
                return;

            Apply(new ResponseAnswerAdded(question));
        }

        public void AddResponseOptions(Guid question, Guid[] items)
        {
            var answer = Session.FindAnswer(question)
                ?? throw new AnswerNotFoundException(question);

            var changeItems = items.Where(x => !answer.Options.Any(y => y.Item == x)).ToArray();
            if (changeItems.IsEmpty())
                return;

            Apply(new ResponseOptionsAdded(question, changeItems));
        }

        public void StartResponse(DateTimeOffset? started, bool noStatusChange)
        {
            Apply(new ResponseSessionStarted(started, noStatusChange));
        }

        public void ChangeResponseAnswer(Guid question, string answer)
        {
            if (!Session.ContainsAnswer(question))
                Apply(new ResponseAnswerAdded(question));

            Apply(new ResponseAnswerChanged(question, answer));
        }

        public void ChangeResponseGroup(Guid? group)
            => Apply(new ResponseGroupChanged(group));

        public void ChangeResponsePeriod(Guid? period)
            => Apply(new ResponsePeriodChanged(period));

        public void ChangeResponseUser(Guid user)
            => Apply(new ResponseUserChanged(user));

        public void SelectResponseOption(Guid option)
        {
            if (Session.ContainsOption(option))
                Apply(new ResponseOptionSelected(option));
        }

        public void UnselectResponseOption(Guid option)
        {
            if (Session.ContainsOption(option))
                Apply(new ResponseOptionUnselected(option));
        }

        public void CompleteResponse(DateTimeOffset? completed)
        {
            Apply(new ResponseSessionCompleted(completed));
        }

        public void ConfirmResponse()
        {
            Apply(new ResponseSessionConfirmed());
        }

        public void ReviewResponse()
        {
            Apply(new ResponseSessionReviewed());
        }

        public void LockResponse()
        {
            Apply(new ResponseSessionLocked());
        }

        public void TermsConsent(Guid session, Guid organization, Guid question, Guid user)
        {
            Apply(new ResponseSessionFormConsent(question, organization, session, user));
        }

        public void UnlockResponse()
        {
            Apply(new ResponseSessionUnlocked());
        }

        public void DeleteResponse()
        {
            Apply(new ResponseSessionDeleted());
        }
    }
}
