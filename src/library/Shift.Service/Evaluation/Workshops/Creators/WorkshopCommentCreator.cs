using Humanizer;

using InSite.Domain.Banks;

using Shift.Common;

using Shift.Contract;
using Shift.Service.Security;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class WorkshopCommentCreator(BankState bank, TimeZoneInfo timeZone, UserReader userReader)
{
    public async Task<WorkshopComment[]> CreateAsync(Comment[] comments, bool showSubject)
    {
        var criteria = new SearchUsers
        {
            UserIds = comments.Select(x => x.Author).Distinct().ToArray()
        };
        criteria.Filter.Page = 0;

        var users = (await userReader.SearchAsync(criteria)).ToDictionary(x => x.UserId);

        var result = comments
            .OrderByDescending(x => x.Posted)
            .Select(comment => new WorkshopComment
            {
                CommentId = comment.Identifier,
                EntityId = comment.Subject,
                AuthorName = users.TryGetValue(comment.Author, out var author) ? author.FullName : "Unknown",
                PostedOn = TimeZoneInfo.ConvertTime(comment.Posted, timeZone).Humanize(),
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
