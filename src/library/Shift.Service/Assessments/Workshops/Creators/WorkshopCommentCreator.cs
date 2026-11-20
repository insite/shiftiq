using Humanizer;

using InSite.Domain.Banks;

using Shift.Common;

using Shift.Contract;
using Shift.Service.Security;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class WorkshopCommentCreator(BankState bank, string timeZoneId, UserReader userReader)
{
    public enum SortOrder { OldestToNewest, NewestToOldest }

    public async Task<WorkshopComment[]> CreateAsync(Comment[] comments, bool showSubject, SortOrder sortOrder)
    {
        var criteria = new SearchUsers
        {
            UserIds = comments.Select(x => x.Author).Distinct().ToArray()
        };
        criteria.DisablePaging();

        var users = (await userReader.SearchAsync(criteria)).ToDictionary(x => x.UserId);

        var sortedComments = sortOrder == SortOrder.NewestToOldest
            ? comments.OrderByDescending(x => x.Posted)
            : comments.OrderBy(x => x.Posted);

        var result = sortedComments
            .Select(comment => new WorkshopComment
            {
                CommentId = comment.Identifier,
                EntityId = comment.Subject,
                AuthorName = users.TryGetValue(comment.Author, out var author) ? author.FullName : "Unknown",
                PostedOn = TimeZones.ConvertTime(comment.Posted, timeZoneId).Humanize(),
                Subject = showSubject ? comment.GetSubjectTitle(bank) : null,
                Text = Markdown.ToHtml(comment.Text),
                Category = comment.Category,
                Flag = comment.Flag,
                EventFormat = comment.EventFormat,
                IsHidden = comment.IsHidden
            }).ToArray();

        return result;
    }
}
