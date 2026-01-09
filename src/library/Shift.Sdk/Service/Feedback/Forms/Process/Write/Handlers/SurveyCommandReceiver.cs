using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Surveys.Forms;

namespace InSite.Application.Surveys.Write.Handlers
{
    public class SurveyCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public SurveyCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            _publisher = publisher;
            _repository = repository;

            commander.Subscribe<AddSurveyBranch>(Handle);
            commander.Subscribe<AddSurveyCondition>(Handle);
            commander.Subscribe<AddSurveyFormMessage>(Handle);
            commander.Subscribe<AddSurveyOptionItem>(Handle);
            commander.Subscribe<AddSurveyOptionList>(Handle);
            commander.Subscribe<AddSurveyQuestion>(Handle);
            commander.Subscribe<AttributeSurveyQuestion>(Handle);
            commander.Subscribe<ChangeSurveyDisplaySummaryChart>(Handle);
            commander.Subscribe<ChangeSurveyFormContent>(Handle);
            commander.Subscribe<ChangeSurveyFormAsset>(Handle);
            commander.Subscribe<ChangeSurveyFormLanguages>(Handle);
            commander.Subscribe<ChangeSurveyFormMessages>(Handle);
            commander.Subscribe<ChangeSurveyFormSchedule>(Handle);
            commander.Subscribe<ChangeSurveyFormSettings>(Handle);
            commander.Subscribe<ChangeSurveyFormStatus>(Handle);
            commander.Subscribe<ChangeSurveyHook>(Handle);
            commander.Subscribe<ChangeSurveyOptionItemContent>(Handle);
            commander.Subscribe<ChangeSurveyOptionItemSettings>(Handle);
            commander.Subscribe<ChangeSurveyOptionListContent>(Handle);
            commander.Subscribe<ChangeSurveyQuestionContent>(Handle);
            commander.Subscribe<ChangeSurveyQuestionSettings>(Handle);
            commander.Subscribe<ChangeSurveyScale>(Handle);
            commander.Subscribe<ConfigureSurveyWorkflow>(Handle);
            commander.Subscribe<CreateSurveyForm>(Handle);
            commander.Subscribe<LockSurveyForm>(Handle);
            commander.Subscribe<ModifySurveyComment>(Handle);
            commander.Subscribe<PostSurveyComment>(Handle);
            commander.Subscribe<RecodeSurveyQuestion>(Handle);
            commander.Subscribe<DeleteSurveyBranch>(Handle);
            commander.Subscribe<DeleteSurveyComment>(Handle);
            commander.Subscribe<DeleteSurveyCondition>(Handle);
            commander.Subscribe<DeleteSurveyOptionItem>(Handle);
            commander.Subscribe<DeleteSurveyOptionList>(Handle);
            commander.Subscribe<DeleteSurveyQuestion>(Handle);
            commander.Subscribe<RenameSurveyForm>(Handle);
            commander.Subscribe<ReorderSurveyOptionItems>(Handle);
            commander.Subscribe<ReorderSurveyOptionLists>(Handle);
            commander.Subscribe<ReorderSurveyQuestions>(Handle);
            commander.Subscribe<UnlockSurveyForm>(Handle);
            commander.Subscribe<DeleteSurveyForm>(Handle);
        }

        private void Commit(SurveyAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddSurveyBranch c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyBranch(c.FromItem, c.ToQuestion);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSurveyCondition c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyCondition(c.MaskingItem, c.MaskedQuestions);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSurveyFormMessage c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyFormMessage(c.Message);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSurveyOptionItem c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyOptionItem(c.List, c.Item);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSurveyOptionList c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyOptionList(c.Question, c.List);
                Commit(aggregate, c);
            });
        }

        public void Handle(AddSurveyQuestion c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AddSurveyQuestion(c.Question, c.Type, c.Code, c.Indicator, c.Source);
                Commit(aggregate, c);
            });
        }

        public void Handle(AttributeSurveyQuestion c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.AttributeSurveyQuestion(c.Question, c.Attribute);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyDisplaySummaryChart c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyDisplaySummaryChart(c.DisplaySummaryChart);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormContent c)
        {
            c.Content?.CreateSnips();

            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormContent(c.Content);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormAsset c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormAsset(c.Asset);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormLanguages c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormLanguages(c.Language, c.Translations);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormMessages c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormMessages(c.Messages);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormSchedule c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormSchedule(c.Opened, c.Closed);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormSettings c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormSettings(c.UserFeedback, c.RequireUserIdentification, c.RequireUserAuthentication, c.ResponseLimitPerUser, c.DurationMinutes, c.EnableUserConfidentiality);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyFormStatus c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyFormStatus(c.Status);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyHook c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyHook(c.Hook);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyOptionItemContent c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyOptionItemContent(c.Item, c.Content);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyOptionItemSettings c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyOptionItemSettings(c.Item, c.Category, c.Points);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyOptionListContent c)
        {
            c.Content?.CreateSnips();

            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyOptionListContent(c.List, c.Content, c.Category);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyQuestionContent c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyQuestionContent(c.Question, c.Content);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyQuestionSettings c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyQuestionSettings(c.Question, c.IsHidden, c.IsRequired, c.IsNested, c.LikertAnalysis, c.ListEnableRandomization, c.ListEnableOtherText, c.ListEnableBranch, c.ListEnableGroupMembership, c.ListDisableColumnHeadingWrap, c.TextLineCount, c.TextCharacterLimit, c.NumberEnableStatistics, c.NumberEnableAutoCalc, c.NumberAutoCalcQuestions, c.NumberEnableNotApplicable, c.ListSelectionRange, c.EnableCreateCase);
                Commit(aggregate, c);
            });
        }

        public void Handle(ChangeSurveyScale c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ChangeSurveyScale(c.Question, c.Scale);
                Commit(aggregate, c);
            });
        }

        public void Handle(ConfigureSurveyWorkflow c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ConfigureSurveyWorkflow(c.Configuration);
                Commit(aggregate, c);
            });
        }

        public void Handle(CreateSurveyForm c)
        {
            var aggregate = new SurveyAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.CreateSurveyForm(c.Source, c.Tenant, c.Asset, c.Name, c.Status, c.Language);
            Commit(aggregate, c);
        }

        public void Handle(LockSurveyForm c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.LockSurveyForm(c.Locked);
                Commit(aggregate, c);
            });
        }

        public void Handle(ModifySurveyComment c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ModifySurveyComment(c.Comment, c.Text, c.Flag, c.Resolved);
                Commit(aggregate, c);
            });
        }

        public void Handle(PostSurveyComment c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.PostSurveyComment(c.Comment, c.Text, c.Flag, c.Resolved);
                Commit(aggregate, c);
            });
        }

        public void Handle(RecodeSurveyQuestion c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RecodeSurveyQuestion(c.Question, c.Code, c.Indicator);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyBranch c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyBranch(c.FromItem, c.ToQuestion);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyComment c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyComment(c.Comment);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyCondition c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyCondition(c.MaskingItem, c.MaskedQuestions);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyOptionItem c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyOptionItem(c.Item);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyOptionList c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyOptionList(c.List);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyQuestion c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyQuestion(c.Question);
                Commit(aggregate, c);
            });
        }

        public void Handle(RenameSurveyForm c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.RenameSurveyForm(c.Name);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReorderSurveyOptionItems c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReorderSurveyOptionItems(c.List, c.Sequences);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReorderSurveyOptionLists c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReorderSurveyOptionLists(c.Question, c.Sequences);
                Commit(aggregate, c);
            });
        }

        public void Handle(ReorderSurveyQuestions c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.ReorderSurveyQuestions(c.Sequences);
                Commit(aggregate, c);
            });
        }

        public void Handle(UnlockSurveyForm c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.UnlockSurveyForm(c.Unlocked);
                Commit(aggregate, c);
            });
        }

        public void Handle(DeleteSurveyForm c)
        {
            _repository.LockAndRun<SurveyAggregate>(c.AggregateIdentifier, aggregate =>
            {
                aggregate.DeleteSurveyForm();
                Commit(aggregate, c);
            });
        }
    }
}