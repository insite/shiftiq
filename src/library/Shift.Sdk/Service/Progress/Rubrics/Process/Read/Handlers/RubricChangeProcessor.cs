using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public class RubricChangeProcessor
    {
        private readonly ICommander _commander;
        private readonly IBankSearch _bankSearch;

        public RubricChangeProcessor(ICommander commander, IChangeQueue publisher, IBankSearch bankSearch)
        {
            _commander = commander;
            _bankSearch = bankSearch;

            publisher.Subscribe<RubricDeleted>(Handle);
        }

        private void Handle(RubricDeleted e)
        {
            var questions = _bankSearch.GetQuestions(new QBankQuestionFilter
            {
                OrganizationIdentifier = e.OriginOrganization,
                RubricIdentifier = e.AggregateIdentifier
            });

            foreach (var question in questions)
                Send(e, new DisconnectQuestionRubric(question.BankIdentifier, question.QuestionIdentifier));
        }

        private void Send(IChange cause, ICommand effect)
        {
            Identify(cause, effect);
            _commander.Send(effect);
        }

        private void Identify(IChange cause, ICommand effect)
        {
            effect.OriginOrganization = cause.OriginOrganization;
            effect.OriginUser = cause.OriginUser;
        }
    }
}
