using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Groups.Write;
using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Application.Surveys.Read
{
    /// <summary>
    /// Implements the process manager for Survey changes. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class SurveyChangeProcessor
    {
        private readonly ICommander _commander;

        private readonly ISurveySearch _surveys;
        private readonly IGroupSearch _groups;

        public SurveyChangeProcessor(ICommander commander, IChangeQueue publisher, ISurveySearch surveys, IGroupSearch groups)
        {
            _commander = commander;

            _surveys = surveys;
            _groups = groups;

            publisher.Subscribe<SurveyOptionItemSettingsChanged>(Handle);
        }

        private void Handle(SurveyOptionItemSettingsChanged e)
        {
            if (OptionHasCategory() && QuestionEnablesGroupMembership() && !GroupExists())
                AddNewGroup();

            bool OptionHasCategory()
                => string.IsNullOrEmpty(e.Category);

            bool QuestionEnablesGroupMembership()
            {
                var option = _surveys.GetSurveyOptionItem(e.Item, x => x.SurveyOptionList.SurveyQuestion);
                if (option != null)
                    return option.SurveyOptionList.SurveyQuestion.SurveyQuestionListEnableGroupMembership;
                return false;
            }

            bool GroupExists()
            {
                var filter = new QGroupFilter { OrganizationIdentifier = e.OriginOrganization, GroupName = e.Category };
                var groups = _groups.SearchGroups(filter);
                return groups.Any();
            }

            void AddNewGroup()
            {
                _commander.Send(new CreateGroup(UuidFactory.Create(), e.OriginOrganization, "List", e.Category));
            }
        }
    }
}