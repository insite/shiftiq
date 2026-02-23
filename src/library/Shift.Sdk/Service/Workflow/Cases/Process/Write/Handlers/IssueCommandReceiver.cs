using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Cases.Write;
using InSite.Domain.Issues;

namespace InSite.Application.Issues.Write
{
    public class IssueCommandReceiver
    {
        private readonly IChangeQueue _publisher;
        private readonly IChangeRepository _repository;

        public IssueCommandReceiver(ICommandQueue commander, IChangeQueue publisher, IChangeRepository repository)
        {
            commander.Subscribe<AddAttachment>(Handle);
            commander.Subscribe<ChangeAttachmentFile>(Handle);
            commander.Subscribe<RenameAttachmentFile>(Handle);
            commander.Subscribe<ModifyFileRequirement>(Handle);
            commander.Subscribe<CompleteFileRequirement>(Handle);
            commander.Subscribe<DeleteFileRequirement>(Handle);
            commander.Subscribe<AssignGroup>(Handle);
            commander.Subscribe<AssignUser>(Handle);
            commander.Subscribe<AuthorComment>(Handle);
            commander.Subscribe<ChangeCommentPrivacy>(Handle);
            commander.Subscribe<ChangeIssueStatus>(Handle);
            commander.Subscribe<ChangeIssueTitle>(Handle);
            commander.Subscribe<ChangeIssueType>(Handle);
            commander.Subscribe<CloseIssue>(Handle);
            commander.Subscribe<ConnectIssueToSurveyResponse>(Handle);
            commander.Subscribe<DescribeIssue>(Handle);
            commander.Subscribe<OpenIssue>(Handle);
            commander.Subscribe<DeleteAttachment>(Handle);
            commander.Subscribe<DeleteComment>(Handle);
            commander.Subscribe<ReopenIssue>(Handle);
            commander.Subscribe<ReviseComment>(Handle);
            commander.Subscribe<UnassignUser>(Handle);
            commander.Subscribe<DeleteIssue>(Handle);

            _publisher = publisher;
            _repository = repository;
        }

        private void Commit(CaseAggregate aggregate, ICommand c)
        {
            aggregate.Identify(c.OriginOrganization, c.OriginUser);
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
            {
                change.AggregateState = aggregate.State;
                _publisher.Publish(change);
            }
        }

        public void Handle(AddAttachment c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AddAttachment(c.FileName, c.FileType, c.FileIdentifier, c.Posted, c.Poster);

            Commit(aggregate, c);
        }

        public void Handle(ChangeAttachmentFile c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeAttachmentFile(c.FileName, c.FileIdentifier);

            Commit(aggregate, c);
        }

        public void Handle(RenameAttachmentFile c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RenameAttachmentFile(c.OldFileName, c.NewFileName);

            Commit(aggregate, c);
        }

        public void Handle(ModifyFileRequirement c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ModifyFileRequirement(c.RequestedFileCategory, c.RequestedFileSubcategory, c.RequestedFrom, c.RequestedFileDescription, c.RequestedFileStatus);

            Commit(aggregate, c);
        }

        public void Handle(CompleteFileRequirement c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.CompleteRequest(c.RequestedFileCategory, c.FileName, c.FileType, c.FileIdentifier, c.Posted, c.Poster);

            Commit(aggregate, c);
        }

        public void Handle(DeleteFileRequirement c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteRequest(c.RequestedFileCategory);

            Commit(aggregate, c);
        }

        public void Handle(AssignGroup c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AssignGroup(c.Group, c.Role);
            Commit(aggregate, c);
        }

        public void Handle(AssignUser c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AssignUser(c.User, c.Role);
            Commit(aggregate, c);
        }

        public void Handle(ChangeIssueStatus c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeIssueStatus(c.Status, c.Effective);
            Commit(aggregate, c);
        }

        public void Handle(ChangeIssueTitle c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeIssueTitle(c.IssueTitle);

            Commit(aggregate, c);
        }

        public void Handle(ChangeIssueType c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeIssueType(c.IssueType);

            Commit(aggregate, c);
        }

        public void Handle(CloseIssue c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.CloseIssue();
            Commit(aggregate, c);
        }

        public void Handle(ConnectIssueToSurveyResponse c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ConnectIssueToSurveyResponse(c.Response);
            Commit(aggregate, c);
        }

        public void Handle(DescribeIssue c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DescribeIssue(c.Description);
            Commit(aggregate, c);
        }

        public void Handle(OpenIssue c)
        {
            var aggregate = new CaseAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.OpenIssue(c.Tenant, c.Number, c.Title, c.Description, c.Opened, c.Source, c.Type, c.Reported);
            Commit(aggregate, c);
        }

        public void Handle(DeleteAttachment c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RemoveAttachment(c.FileName);

            Commit(aggregate, c);
        }

        public void Handle(DeleteComment c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.RemoveComment(c.Comment);

            Commit(aggregate, c);
        }

        public void Handle(ReopenIssue c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReopenIssue();
            Commit(aggregate, c);
        }

        public void Handle(AuthorComment c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.AuthorComment(c.Comment, c.Text, c.Category, c.Flag,
                c.Author, c.AuthorRole, c.AssignedTo, c.ResolvedBy, c.SubCategory, c.Tag,
                c.Posted, c.Flagged, c.Submitted, c.Resolved);

            Commit(aggregate, c);
        }

        public void Handle(ChangeCommentPrivacy c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ChangeCommentPrivacy(c.Comment, c.CommentPrivacy);

            Commit(aggregate, c);
        }

        public void Handle(ReviseComment c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.ReviseComment(c.Comment, c.Text, c.Category, c.Flag,
                c.Revisor, c.AssignedTo, c.ResolvedBy, c.SubCategory, c.Tag,
                c.Revised, c.Flagged, c.Submitted, c.Resolved);

            Commit(aggregate, c);
        }

        public void Handle(UnassignUser c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.UnassignUser(c.User, c.Role);

            Commit(aggregate, c);
        }

        public void Handle(DeleteIssue c)
        {
            var aggregate = _repository.Get<CaseAggregate>(c.AggregateIdentifier, c.ExpectedVersion);
            aggregate.DeleteIssue();
            Commit(aggregate, c);
        }
    }
}
