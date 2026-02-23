using System;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Application.Banks.Write
{
    public class BankCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public BankCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            commander.Subscribe<MemorizeBank>(Handle);
            commander.Subscribe<AddAttachment>(Handle);
            commander.Subscribe<AddAttachmentToQuestion>(Handle);
            commander.Subscribe<AddField>(Handle);
            commander.Subscribe<AddForm>(Handle);
            commander.Subscribe<AddOption>(Handle);
            commander.Subscribe<AddQuestionOrderingOption>(Handle);
            commander.Subscribe<AddQuestionOrderingSolution>(Handle);
            commander.Subscribe<AddQuestion>(Handle);
            commander.Subscribe<AddQuestion2>(Handle);
            commander.Subscribe<AddSection>(Handle);
            commander.Subscribe<AddSet>(Handle);
            commander.Subscribe<AddCriterion>(Handle);
            commander.Subscribe<AddSpecification>(Handle);
            commander.Subscribe<AddQuestionHotspotOption>(Handle);
            commander.Subscribe<AddQuestionLikertColumn>(Handle);
            commander.Subscribe<AddQuestionLikertRow>(Handle);
            commander.Subscribe<AnalyzeBank>(Handle);
            commander.Subscribe<AnalyzeForm>(Handle);
            commander.Subscribe<ArchiveForm>(Handle);
            commander.Subscribe<ChangeAssessmentHook>(Handle);
            commander.Subscribe<ChangeAttachment>(Handle);
            commander.Subscribe<ChangeAttachmentImage>(Handle);
            commander.Subscribe<ChangeBankContent>(Handle);
            commander.Subscribe<ChangeBankLevel>(Handle);
            commander.Subscribe<ChangeBankType>(Handle);
            commander.Subscribe<ChangeBankStandard>(Handle);
            commander.Subscribe<ChangeBankEdition>(Handle);
            commander.Subscribe<ChangeBankStatus>(Handle);
            commander.Subscribe<ChangeQuestionHotspotImage>(Handle);
            commander.Subscribe<ChangeQuestionHotspotOption>(Handle);
            commander.Subscribe<ChangeQuestionHotspotPinLimit>(Handle);
            commander.Subscribe<ChangeQuestionHotspotShowShapes>(Handle);
            commander.Subscribe<OpenBank>(Handle);
            commander.Subscribe<ChangeCommentAuthorRole>(Handle);
            commander.Subscribe<ChangeCommentVisibility>(Handle);
            commander.Subscribe<ChangeFormAddendum>(Handle);
            commander.Subscribe<ChangeFormAsset>(Handle);
            commander.Subscribe<ChangeFormClassification>(Handle);
            commander.Subscribe<ChangeFormCode>(Handle);
            commander.Subscribe<ChangeFormContent>(Handle);
            commander.Subscribe<ChangeFormGradebook>(Handle);
            commander.Subscribe<ChangeFormInvigilation>(Handle);
            commander.Subscribe<ChangeFormName>(Handle);
            commander.Subscribe<ChangeFormVersion>(Handle);
            commander.Subscribe<ChangeOption>(Handle);
            commander.Subscribe<ChangeQuestionClassification>(Handle);
            commander.Subscribe<ChangeQuestionComposedVoice>(Handle);
            commander.Subscribe<ChangeQuestionContent>(Handle);
            commander.Subscribe<ChangeQuestionFlag>(Handle);
            commander.Subscribe<ChangeQuestionGradeItem2>(Handle);
            commander.Subscribe<ChangeQuestionLikertRowGradeItem>(Handle);
            commander.Subscribe<ChangeQuestionLayout>(Handle);
            commander.Subscribe<ChangeQuestionLikertColumn>(Handle);
            commander.Subscribe<ChangeQuestionLikertOptions>(Handle);
            commander.Subscribe<ChangeQuestionLikertRow>(Handle);
            commander.Subscribe<ChangeQuestionMatches>(Handle);
            commander.Subscribe<ChangeQuestionPublicationStatus>(Handle);
            commander.Subscribe<ChangeQuestionOrderingLabel>(Handle);
            commander.Subscribe<ChangeQuestionOrderingOption>(Handle);
            commander.Subscribe<ChangeQuestionOrderingSolution>(Handle);
            commander.Subscribe<ChangeQuestionRandomization>(Handle);
            commander.Subscribe<ChangeQuestionScoring>(Handle);
            commander.Subscribe<ChangeQuestionSet>(Handle);
            commander.Subscribe<ChangeQuestionStandard>(Handle);
            commander.Subscribe<ChangeQuestionCondition>(Handle);
            commander.Subscribe<ChangeSetRandomization>(Handle);
            commander.Subscribe<ChangeSectionContent>(Handle);
            commander.Subscribe<ChangeSetStandard>(Handle);
            commander.Subscribe<ChangeCriterionFilter>(Handle);
            commander.Subscribe<ChangeSpecificationCalculation>(Handle);
            commander.Subscribe<ChangeSpecificationContent>(Handle);
            commander.Subscribe<ChangeSpecificationTabTimeLimit>(Handle);
            commander.Subscribe<EnableThirdPartyAssessment>(Handle);
            commander.Subscribe<DisableThirdPartyAssessment>(Handle);
            commander.Subscribe<DisconnectQuestionRubric>(Handle);
            commander.Subscribe<ImportSet>(Handle);
            commander.Subscribe<LockBank>(Handle);
            commander.Subscribe<MergeSets>(Handle);
            commander.Subscribe<ModifyFormLanguages>(Handle);
            commander.Subscribe<MoveComment>(Handle);
            commander.Subscribe<ConnectFormMessage>(Handle);
            commander.Subscribe<ConnectQuestionRubric>(Handle);
            commander.Subscribe<MoveQuestion>(Handle);
            commander.Subscribe<PostComment>(Handle);
            commander.Subscribe<PublishForm>(Handle);
            commander.Subscribe<ReconfigureSection>(Handle);
            commander.Subscribe<ReconfigureSpecification>(Handle);
            commander.Subscribe<RejectComment>(Handle);
            commander.Subscribe<DeleteBank>(Handle);
            commander.Subscribe<DeleteAttachment>(Handle);
            commander.Subscribe<DeleteAttachmentFromQuestion>(Handle);
            commander.Subscribe<DeleteField>(Handle);
            commander.Subscribe<DeleteFields>(Handle);
            commander.Subscribe<DeleteForm>(Handle);
            commander.Subscribe<DeleteOption>(Handle);
            commander.Subscribe<DeleteQuestion>(Handle);
            commander.Subscribe<DeleteQuestionLikertColumn>(Handle);
            commander.Subscribe<DeleteQuestionLikertRow>(Handle);
            commander.Subscribe<DeleteQuestionOrderingOption>(Handle);
            commander.Subscribe<DeleteQuestionOrderingSolution>(Handle);
            commander.Subscribe<DeleteSection>(Handle);
            commander.Subscribe<DeleteSet>(Handle);
            commander.Subscribe<DeleteCriterion>(Handle);
            commander.Subscribe<DeleteSpecification>(Handle);
            commander.Subscribe<DeleteQuestionHotspotOption>(Handle);
            commander.Subscribe<DisableSectionsAsTabs>(Handle);
            commander.Subscribe<EnableSectionsAsTabs>(Handle);
            commander.Subscribe<DisableTabNavigation>(Handle);
            commander.Subscribe<EnableTabNavigation>(Handle);
            commander.Subscribe<DisableSingleQuestionPerTab>(Handle);
            commander.Subscribe<EnableSingleQuestionPerTab>(Handle);
            commander.Subscribe<DuplicateQuestion>(Handle);
            commander.Subscribe<RenameBank>(Handle);
            commander.Subscribe<RenameSet>(Handle);
            commander.Subscribe<RenameSpecification>(Handle);
            commander.Subscribe<RetypeSpecification>(Handle);
            commander.Subscribe<ReorderFields>(Handle);
            commander.Subscribe<ReorderOptions>(Handle);
            commander.Subscribe<ReorderQuestionHotspotOptions>(Handle);
            commander.Subscribe<ReorderQuestionLikert>(Handle);
            commander.Subscribe<ReorderQuestionOrderingOptions>(Handle);
            commander.Subscribe<ReorderQuestionOrderingSolutions>(Handle);
            commander.Subscribe<ReorderQuestionOrderingSolutionOptions>(Handle);
            commander.Subscribe<ReorderQuestions>(Handle);
            commander.Subscribe<ReorderSections>(Handle);
            commander.Subscribe<ReorderSets>(Handle);
            commander.Subscribe<RetractComment>(Handle);
            commander.Subscribe<ReviseComment>(Handle);
            commander.Subscribe<SwapFields>(Handle);
            commander.Subscribe<UnarchiveForm>(Handle);
            commander.Subscribe<UnlockBank>(Handle);
            commander.Subscribe<UnpublishForm>(Handle);
            commander.Subscribe<UpgradeAttachment>(Handle);
            commander.Subscribe<UpgradeForm>(Handle);
            commander.Subscribe<UpgradeQuestion>(Handle);
            commander.Subscribe<VerifyAssessmentQuestionOrder>(Handle);

            _publisher = publisher;
            _repository = repository;
        }

        private void Commit(BankAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(MemorizeBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.MemorizeBank(aggregate.Data);
            Commit(aggregate, c);
        }

        public void Handle(AddAttachment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddAttachment(c.Attachment);

            Commit(aggregate, c);
        }

        public void Handle(AddAttachmentToQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddAttachmentToQuestion(c.Attachment, c.Question);

            Commit(aggregate, c);
        }

        public void Handle(AddField c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddField(c.Field, c.Section, c.Question, c.Index);
            Commit(aggregate, c);
        }

        public void Handle(AddForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddForm(c.Specification, c.Identifier, c.Name, c.Asset, c.TimeLimit);

            Commit(aggregate, c);
        }

        public void Handle(AddOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddOption(c.Question, c.Content, c.Points, c.IsTrue, c.CutScore, c.Standard);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestionOrderingOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddQuestionOrderingOption(c.Question, c.Option, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestionOrderingSolution c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddQuestionOrderingSolution(c.Question, c.Solution, c.Points, c.CutScore);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.AddQuestion(c.Set, c.Question);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestion2 c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.AddQuestion2
                    (c.Set,
                    c.Question,
                    c.Type,
                    c.Condition,
                    c.Asset,
                    c.Standard,
                    c.Source,
                    c.Points,
                    c.CalculationMethod,
                    c.Content
                );

            Commit(aggregate, c);
        }

        public void Handle(AddSection c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddSection(c.Form, c.Section, c.Criterion);

            Commit(aggregate, c);
        }

        public void Handle(AddSet c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddSet(c.Set, c.Name, c.Standard);

            Commit(aggregate, c);
        }

        public void Handle(AddCriterion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddCriterion(c.Specification, c.Sets, c.Identifier, c.Name, c.Weight, c.QuestionLimit, c.BasicFilter, c.AdvancedFilter);

            Commit(aggregate, c);
        }

        public void Handle(AddSpecification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddSpecification(c.Type, c.Consequence, c.Identifier, c.Name, c.Asset, c.FormLimit, c.QuestionLimit, c.Calculation);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestionHotspotOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddQuestionHotspotOption(c.Question, c.Option, c.Shape, c.Content, c.Points);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestionLikertColumn c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddQuestionLikertColumn(c.Question, c.Column, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(AddQuestionLikertRow c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddQuestionLikertRow(c.Question, c.Row, c.Standard, c.SubStandards, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(AnalyzeBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AnalyzeBank();
            Commit(aggregate, c);
        }

        public void Handle(AnalyzeForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AnalyzeForm(c.FormIdentifier);
            Commit(aggregate, c);
        }

        public void Handle(DeleteBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteBank();
            Commit(aggregate, c);
        }

        public void Handle(ConnectFormMessage c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ConnectFormMessage(c.Form, c.MessageType, c.MessageIdentifier);
            Commit(aggregate, c);
        }

        public void Handle(ConnectQuestionRubric c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ConnectQuestionRubric(c.Question, c.Rubric);
            Commit(aggregate, c);
        }

        public void Handle(ArchiveForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ArchiveForm(c.Form, c.Questions, c.Attachments);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAssessmentHook c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeAssessmentHook(c.Form, c.Hook);
            Commit(aggregate, c);
        }

        public void Handle(ChangeAttachment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeAttachment(c.Attachment, c.Condition, c.Content, c.Image);

            Commit(aggregate, c);
        }

        public void Handle(ChangeAttachmentImage c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeAttachmentImage(c.Attachment, c.Upload, c.Author, c.ActualDimension);

            Commit(aggregate, c);
        }

        public void Handle(ChangeBankContent c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankContent(c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeBankLevel c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankLevel(c.Level);

            Commit(aggregate, c);
        }

        public void Handle(ChangeBankType c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankType(c.Type);
            Commit(aggregate, c);
        }

        public void Handle(ChangeBankStandard c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankStandard(c.Standard);

            Commit(aggregate, c);
        }

        public void Handle(ChangeBankEdition c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankEdition(c.Major, c.Minor);

            Commit(aggregate, c);
        }

        public void Handle(ChangeBankStatus c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeBankStatus(c.IsActive);

            Commit(aggregate, c);
        }

        public void Handle(ChangeCommentAuthorRole c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            if (aggregate.Data.FindComment(c.Comment) == null)
                return;

            aggregate.ChangeCommentAuthorRole(c.Comment, c.AuthorRole);

            Commit(aggregate, c);
        }

        public void Handle(ChangeCommentVisibility c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            if (aggregate.Data.FindComment(c.Comment) == null)
                return;

            aggregate.ChangeCommentVisibility(c.Comment, c.IsHidden);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormAddendum c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormAddendum(c.Form, c.Acronyms, c.Formulas, c.Figures, c.RemoveObsolete);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormAsset c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormAsset(c.Form, c.Asset);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormClassification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormClassification(c.Form, c.Instrument, c.Theme);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormCode c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormCode(c.Form, c.Code, c.Source, c.Origin);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormContent c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormContent(c.Form, c.Content, c.HasDiagrams, c.HasReferenceMaterials);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormGradebook c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormGradebook(c.Form, c.Gradebook);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormInvigilation c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormInvigilation(c.Form, c.Invigilation);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormName c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormName(c.Form, c.Name);

            Commit(aggregate, c);
        }

        public void Handle(ChangeFormVersion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeFormVersion(c.Form, c.Major, c.Minor);

            Commit(aggregate, c);
        }

        public void Handle(ChangeOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeOption(c.Question, c.Number, c.Content, c.Points, c.IsTrue, c.CutScore, c.Standard);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionClassification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionClassification(c.Question, c.Classification);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionComposedVoice c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionComposedVoice(c.Question, c.ComposedVoice);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionContent c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionContent(c.Question, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionFlag c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionFlag(c.Question, c.Flag);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionGradeItem2 c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionGradeItem2(c.Form, c.Question, c.GradeItem);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionLikertRowGradeItem c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionLikertRowGradeItem(c.Form, c.Question, c.LikertRow, c.GradeItem);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionHotspotImage c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionHotspotImage(c.Question, c.Image);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionHotspotOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionHotspotOption(c.Question, c.Option, c.Shape, c.Content, c.Points);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionHotspotPinLimit c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionHotspotPinLimit(c.Question, c.PinLimit);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionHotspotShowShapes c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionHotspotShowShapes(c.Question, c.ShowShapes);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionLayout c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionLayout(c.Question, c.Layout);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionLikertColumn c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionLikertColumn(c.Question, c.Column, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionLikertOptions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionLikertOptions(c.Question, c.Options);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionLikertRow c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionLikertRow(c.Question, c.Row, c.Standard, c.SubStandards, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionMatches c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionMatches(c.Question, c.Matches);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionPublicationStatus c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionPublicationStatus(c.Question, c.Status);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionOrderingLabel c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionOrderingLabel(c.Question, c.Label);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionOrderingOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionOrderingOption(c.Question, c.Option, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionOrderingSolution c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionOrderingSolution(c.Question, c.Solution, c.Points, c.CutScore);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionRandomization c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionRandomization(c.Question, c.Randomization);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionScoring c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionScoring(c.Question, c.Points, c.CutScore, c.CalculationMethod);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionSet c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionSet(c.Question, c.Set);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionStandard c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionStandard(c.Question, c.Standard, c.SubStandards);

            Commit(aggregate, c);
        }

        public void Handle(ChangeQuestionCondition c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeQuestionCondition(c.Question, c.Condition);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSetRandomization c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSetRandomization(c.Set, c.Randomization);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSectionContent c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSectionContent(c.Set, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSetStandard c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSetStandard(c.Set, c.Standard);

            Commit(aggregate, c);
        }

        public void Handle(ChangeCriterionFilter c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeCriterionFilter(c.Criterion, c.SetWeight, c.QuestionLimit, c.TagFilter, c.PivotFilter);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSpecificationCalculation c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSpecificationCalculation(c.Specification, c.Calculation);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSpecificationContent c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSpecificationContent(c.Specification, c.Content);

            Commit(aggregate, c);
        }

        public void Handle(ChangeSpecificationTabTimeLimit c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeSpecificationTabTimeLimit(c.Specification, c.TabTimeLimit);

            Commit(aggregate, c);
        }

        public void Handle(EnableThirdPartyAssessment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableThirdPartyAssessment(c.Form);
            Commit(aggregate, c);
        }

        public void Handle(DisableThirdPartyAssessment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableThirdPartyAssessment(c.Form);
            Commit(aggregate, c);
        }

        public void Handle(DisconnectQuestionRubric c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisconnectQuestionRubric(c.Question);
            Commit(aggregate, c);
        }

        public void Handle(OpenBank c)
        {
            var aggregate = new BankAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.OpenBank(c.Bank);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestionLikertColumn c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestionLikertColumn(c.Question, c.Column);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestionLikertRow c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestionLikertRow(c.Question, c.Row);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestionOrderingOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestionOrderingOption(c.Question, c.Option);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestionOrderingSolution c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestionOrderingSolution(c.Question, c.Solution);

            Commit(aggregate, c);
        }

        public void Handle(DuplicateQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DuplicateQuestion(c.SourceQuestion, c.DestinationQuestion, c.DestinationAsset);

            Commit(aggregate, c);
        }

        public void Handle(MergeSets c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.MergeSets(c.Set);

            Commit(aggregate, c);
        }

        public void Handle(ModifyFormLanguages c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ModifyFormLanguages(c.Form, c.Languages.NullIfEmpty());

            Commit(aggregate, c);
        }

        public void Handle(MoveComment c)
        {
            var bank = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            var comment = bank.Data.FindComment(c.Comment);
            if (comment == null || comment.Type == c.Type && comment.Subject == c.Subject || !IsCommentSubjectValid(bank.Data, c.Type, c.Subject))
                return;

            bank.MoveComment(c.Comment, c.Type, c.Subject);

            Commit(bank, c);
        }

        public void Handle(MoveQuestion c)
        {
            if (c.AggregateIdentifier != c.Bank)
            {
                var sourceAggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
                if (!sourceAggregate.CanMoveQuestionOut(c.Question))
                    return;

                var destinationAggregate = _repository.Get<BankAggregate>(c.Bank);
                if (!destinationAggregate.CanMoveQuestionIn(c.Set, c.Question))
                    return;

                var data = sourceAggregate.MoveQuestionOut(c.Bank, c.Set, c.Competency, c.Question);

                Commit(sourceAggregate, c);

                destinationAggregate.MoveQuestionIn(c.AggregateIdentifier, c.Set, c.Competency, c.Asset, data.Item1, data.Item2);

                Commit(destinationAggregate, c);
            }
            else
            {
                var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
                aggregate.MoveQuestion(c.Set, c.Competency, c.Asset, c.Question);
                Commit(aggregate, c);
            }
        }

        public void Handle(ImportSet c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ImportSet(c.Set);
            Commit(aggregate, c);
        }

        public void Handle(LockBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.LockBank();
            Commit(aggregate, c);
        }

        public void Handle(PostComment c)
        {
            var bank = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            var comment = bank.Data.FindComment(c.Comment);
            if (comment != null || !IsCommentSubjectValid(bank.Data, c.Type, c.Subject))
                return;

            bank.PostComment(
                c.Comment,
                c.Flag,
                c.Type,
                c.Subject,
                c.Author,
                c.AuthorRole,
                c.Category,
                c.Text,
                c.Instructor,
                c.EventDate,
                c.EventFormat,
                c.Posted);

            Commit(bank, c);
        }

        public void Handle(PublishForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.PublishForm(c.Form, c.Publication);

            Commit(aggregate, c);
        }

        public void Handle(ReconfigureSection c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReconfigureSection(c.Section, c.WarningOnNextTabEnabled, c.BreakTimerEnabled, c.TimeLimit, c.TimerType);

            Commit(aggregate, c);
        }

        public void Handle(ReconfigureSpecification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReconfigureSpecification(c.Specification, c.Consequence, c.FormLimit, c.QuestionLimit);

            Commit(aggregate, c);
        }

        public void Handle(DeleteAttachment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteAttachment(c.Attachment);

            Commit(aggregate, c);
        }

        public void Handle(DeleteAttachmentFromQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteAttachmentFromQuestion(c.Attachment, c.Question);

            Commit(aggregate, c);
        }

        public void Handle(RejectComment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            if (aggregate.Data.FindComment(c.Comment) == null)
                return;

            aggregate.RejectComment(c.Comment);

            Commit(aggregate, c);
        }

        public void Handle(RetractComment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            if (aggregate.Data.FindComment(c.Comment) == null)
                return;

            aggregate.RetractComment(c.Comment);

            Commit(aggregate, c);
        }

        public void Handle(DeleteField c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteField(c.Field, c.Form, c.Question);
            Commit(aggregate, c);
        }

        public void Handle(DeleteFields c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteFields(c.Form, c.Question);
            Commit(aggregate, c);
        }

        public void Handle(DeleteForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteForm(c.Form);

            Commit(aggregate, c);
        }

        public void Handle(DeleteOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteOption(c.Question, c.Option);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestion(c.Question, c.RemoveAllQuestions);

            Commit(aggregate, c);
        }

        public void Handle(DeleteSection c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteSection(c.Section);

            Commit(aggregate, c);
        }

        public void Handle(DeleteSet c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteSet(c.Set);

            Commit(aggregate, c);
        }

        public void Handle(DeleteCriterion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteCriterion(c.Criterion);

            Commit(aggregate, c);
        }

        public void Handle(DeleteSpecification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteSpecification(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(DeleteQuestionHotspotOption c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteQuestionHotspotOption(c.Question, c.Option);

            Commit(aggregate, c);
        }

        public void Handle(DisableSectionsAsTabs c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableSectionsAsTabs(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(EnableSectionsAsTabs c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableSectionsAsTabs(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(DisableTabNavigation c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableTabNavigation(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(EnableTabNavigation c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableTabNavigation(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(DisableSingleQuestionPerTab c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DisableSingleQuestionPerTab(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(EnableSingleQuestionPerTab c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.EnableSingleQuestionPerTab(c.Specification);

            Commit(aggregate, c);
        }

        public void Handle(RenameBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RenameBank(c.Name);

            Commit(aggregate, c);
        }

        public void Handle(RenameSet c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RenameSet(c.Set, c.Name);

            Commit(aggregate, c);
        }

        public void Handle(RenameSpecification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RenameSpecification(c.Specification, c.Name);
            Commit(aggregate, c);
        }

        public void Handle(RetypeSpecification c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RetypeSpecification(c.Specification, c.Type);
            Commit(aggregate, c);
        }

        public void Handle(ReorderFields c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderFields(c.Section, c.Sequences);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestionHotspotOptions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestionHotspotOptions(c.Question, c.OptionsOrder);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestionLikert c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestionLikert(c.Question, c.Rows, c.Columns);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestionOrderingOptions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestionOrderingOptions(c.Question, c.OptionsOrder);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestionOrderingSolutions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestionOrderingSolutions(c.Question, c.SolutionsOrder);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestionOrderingSolutionOptions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestionOrderingSolutionOptions(c.Question, c.Solution, c.OptionsOrder);

            Commit(aggregate, c);
        }

        public void Handle(ReorderOptions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderOptions(c.Question, c.Sequences);

            Commit(aggregate, c);
        }

        public void Handle(ReorderQuestions c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderQuestions(c.Set, c.Sequences);

            Commit(aggregate, c);
        }

        public void Handle(ReorderSections c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderSections(c.Form, c.Sequences);
            Commit(aggregate, c);
        }

        public void Handle(ReorderSets c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReorderSets(c.Sequences);
            Commit(aggregate, c);
        }

        public void Handle(ReviseComment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            if (aggregate.Data.FindComment(c.Comment) == null)
                return;

            aggregate.ReviseComment(c.Comment, c.Author, c.Flag, c.Category, c.Text, c.Instructor, c.EventDate, c.EventFormat, c.Revised);

            Commit(aggregate, c);
        }

        public void Handle(SwapFields c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.SwapFields(c.A, c.B);

            Commit(aggregate, c);
        }

        public void Handle(UnarchiveForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UnarchiveForm(c.Form, c.Questions, c.Attachments);
            Commit(aggregate, c);
        }

        public void Handle(UnlockBank c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UnlockBank();
            Commit(aggregate, c);
        }

        public void Handle(UnpublishForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);

            aggregate.UnpublishForm(c.Form);

            Commit(aggregate, c);
        }

        public void Handle(UpgradeAttachment c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UpgradeAttachment(c.CurrentAttachment, c.UpgradedAttachment);
            Commit(aggregate, c);
        }

        public void Handle(UpgradeForm c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UpgradeForm(c.Source, c.Destination, c.NewName);
            Commit(aggregate, c);
        }

        public void Handle(UpgradeQuestion c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UpgradeQuestion(c.CurrentQuestion, c.UpgradedQuestion);
            Commit(aggregate, c);
        }

        public void Handle(VerifyAssessmentQuestionOrder c)
        {
            var aggregate = _repository.Get<BankAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.VerifyAssessmentFormFields(c.Form, c.Questions);
            Commit(aggregate, c);
        }

        #region Methods (helpers)

        private static bool IsCommentSubjectValid(BankState bank, CommentType type, Guid subject)
        {
            if (type == CommentType.Bank && subject != bank.Identifier)
                return false;

            if (type == CommentType.Set && bank.FindSet(subject) == null)
                return false;

            if (type == CommentType.Specification && bank.FindSpecification(subject) == null)
                return false;

            if (type == CommentType.Criterion && bank.FindCriterion(subject) == null)
                return false;

            if (type == CommentType.Form && bank.FindForm(subject) == null)
                return false;

            if (type == CommentType.Field && bank.FindField(subject) == null)
                return false;

            if (type == CommentType.Question && bank.FindQuestion(subject) == null)
                return false;

            return true;
        }

        #endregion
    }
}