using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Surveys.Sessions
{
    public class ResponseState : AggregateState
    {
        public ResponseSession Session { get; set; }

        public ResponseState()
        {
            Session = new ResponseSession();
        }

        public void When(ResponseAnswerChanged e)
        {
            Session.FindAnswer(e.Question).Answer = e.Answer;
            Session.LastQuestion = e.Question;
        }

        public void When(ResponseGroupChanged e)
        {
            Session.Group = e.Group;
        }
        
        public void When(ResponsePeriodChanged e)
        {
            Session.Period = e.Period;
        }

        public void When(ResponseUserChanged e)
        {
            Session.User = e.User;
        }

        public void When(ResponseSessionCompleted e)
        {
            Session.Status = ResponseSessionStatus.Completed;
            Session.Completed = e.Completed;
            Session.IsLocked = true;
        }

        public void When(ResponseSessionConfirmed e)
        {

        }

        public void When(ResponseSessionFormConsent e)
        {

        }

        public void When(ResponseSessionCreated e)
        {
            Session.Status = ResponseSessionStatus.Created;
            Session.Created = e.ChangeTime;
            Session.Identifier = e.AggregateIdentifier;
            Session.Form = e.Form;
            Session.User = e.User;
            Session.Tenant = e.Tenant;
            Session.Source = e.Source;
        }

        public void When(ResponseSessionLocked e)
        {
            Session.IsLocked = true;
        }

        public void When(ResponseOptionSelected e)
        {
            if (Session.FindAnswerAndOption(e.Item, out var answer, out var option))
            {
                option.IsSelected = true;
                Session.LastQuestion = answer.Question;
            }
        }

        public void When(ResponseOptionUnselected e)
        {
            if (Session.FindAnswerAndOption(e.Item, out var answer, out var option))
            {
                option.IsSelected = false;
                Session.LastQuestion = answer.Question;
            }
        }

        public void When(ResponseOptionsAdded e)
        {
            var options = Session.FindAnswer(e.Question).Options;
            foreach (var item in e.Items)
                options.Add(new ResponseOption(item));
        }

        public void When(ResponseAnswerAdded e)
        {
            Session.Answers.Add(new ResponseAnswer(e.Question));
        }

        public void When(ResponseSessionReviewed e)
        {

        }

        public void When(ResponseSessionStarted e)
        {
            Session.Started = e.Started;

            if (e.NoStatusChange == true)
                Session.Status = ResponseSessionStatus.Started;
        }

        public void When(ResponseSessionUnlocked e)
        {
            Session.IsLocked = false;
        }

        public void When(ResponseSessionDeleted e)
        {

        }
    }
}