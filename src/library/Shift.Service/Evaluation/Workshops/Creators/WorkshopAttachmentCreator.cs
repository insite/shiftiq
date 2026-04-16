using Humanizer;

using InSite.Application.Files.Read;

using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Contract;
using Shift.Service.Content;
using Shift.Service.Security;
using Shift.Service.Timeline;

namespace Shift.Service.Evaluation.Workshops.Creators;

internal class WorkshopAttachmentCreator(UserReader userReader, UploadReader uploadReader, FileReader fileReader, ChangeReader changeReader, IStorageServiceAsync storageService)
{
    private class FileInfo
    {
        public Guid Identifier { get; set; }
        public string FileName { get; set; } = default!;
        public string NavigateUrl { get; set; } = default!;
        public int? FileSize { get; set; }
    }

    public async Task<WorkshopAttachment[]> CreateAsync(BankState bank)
    {
        var users = await ReadUsersAsync(bank);
        var uploads = await ReadUploadsAsync(bank);
        var changeCounts = await ReadChangeCounts(bank);

        var result = new List<WorkshopAttachment>();

        foreach (var attachment in bank.Attachments)
        {
            users.TryGetValue(attachment.Author, out var author);
            uploads.TryGetValue(attachment.FileIdentifier ?? attachment.Upload, out var upload);
            changeCounts.TryGetValue(attachment.Identifier, out var changeCount);

            var model = CreateModel(attachment, author, upload, changeCount);

            result.Add(model);
        }

        result.Sort((a, b) =>
        {
            var cmp = (a.FileUrl == null).CompareTo(b.FileUrl == null);
            return cmp != 0 ? cmp : (a.Title ?? "").CompareTo(b.Title ?? "");
        });

        return result.ToArray();
    }

    private static WorkshopAttachment CreateModel(Attachment attachment, UserMatch? author, FileInfo? upload, int changeCount)
    {
        var (resolution, dimensions, color) = GetImageDetails(attachment);

        return new WorkshopAttachment
        {
            AttachmentId = attachment.Identifier,
            AttachmentType = attachment.Type,
            AssetNumber = attachment.Asset,
            AssetVersion = attachment.AssetVersion,
            Title = attachment.Content?.Title.Default,
            Condition = attachment.Condition,
            PublicationStatus = attachment.PublicationStatus.GetDescription(),
            QuestionCount = attachment.EnumerateAllVersions().Sum(x => x.QuestionIdentifiers.Count),
            PostedOn = attachment.Uploaded,
            FileName = upload?.FileName,
            FileUrl = upload?.NavigateUrl,
            FileSize = upload != null ? (upload.FileSize ?? 0).Bytes().Humanize("0.##") : null,
            AuthorName = author?.FullName,
            ChangeCount = changeCount,
            ImageResolution = resolution,
            ImageDimensions = dimensions,
            Color = color,
        };
    }

    private static (string? resolution, string[]? dimensions, string? color) GetImageDetails(Attachment attachment)
    {
        if (attachment.Type != AttachmentType.Image)
            return (null, null, null);

        var hasImageResolution = attachment.Image.Resolution > 0;
        var dimensions = new List<string>();
        
        dimensions.Add($"{attachment.Image.Actual.Width:n0} x {attachment.Image.Actual.Height:n0} pixels");

        if (attachment.Image.TargetOnline?.HasValue == true)
            dimensions.Add($"{attachment.Image.TargetOnline.Width:n0} x {attachment.Image.TargetOnline.Height:n0} pixels (online)");

        if (attachment.Image.TargetPaper?.HasValue == true)
            dimensions.Add($"{attachment.Image.TargetPaper.Width:n0} x {attachment.Image.TargetPaper.Height:n0} pixels (paper)");

        if (attachment.Image.Resolution > 0)
            dimensions.Add($"{Math.Round((decimal)attachment.Image.Actual.Width / attachment.Image.Resolution):n0} x {Math.Round((decimal)attachment.Image.Actual.Height / attachment.Image.Resolution):n0} inches");

        var resolution = hasImageResolution ? attachment.Image.Resolution + " DPI" : null;
        var color = attachment.Image.IsColor ? "Color" : "Black and White";

        return (resolution, dimensions.ToArray(), color);
    }

    private async Task<Dictionary<Guid, UserMatch>> ReadUsersAsync(BankState bank)
    {
        var criteria = new SearchUsers
        {
            UserIds = bank.Attachments.Select(x => x.Author).Distinct().ToArray()
        };
        criteria.Filter.Page = 0;

        return (await userReader.SearchAsync(criteria)).ToDictionary(x => x.UserId);
    }

    private async Task<Dictionary<Guid, FileInfo>> ReadUploadsAsync(BankState bank)
    {
        var uploadIds = bank.Attachments.Where(x => x.Upload != Guid.Empty).Select(x => x.Upload).ToArray();
        var uploads = uploadIds.Length > 0
            ? (await uploadReader.CollectAsync(bank.Tenant, uploadIds))
                .Select(x => new FileInfo
                {
                    Identifier = x.UploadIdentifier,
                    FileName = x.Name,
                    NavigateUrl = "/files" + x.NavigateUrl,
                    FileSize = x.ContentSize
                })
                .ToList()
            : new List<FileInfo>();

        var fileIds = bank.Attachments.Where(x => x.FileIdentifier.HasValue).Select(x => x.FileIdentifier!.Value).ToArray();
        var criteria = new SearchFiles { FileIds = fileIds };
        criteria.Filter.Page = 0;
        
        var files = fileIds.Length > 0
            ? (await fileReader.CollectAsync(criteria))
                .Select(x => new FileInfo
                {
                    Identifier = x.FileIdentifier,
                    FileName = x.FileName,
                    NavigateUrl = storageService.GetFileUrl(x.FileIdentifier, x.FileName, false),
                    FileSize = x.FileSize
                })
                .ToList()
            : new List<FileInfo>();

        files.AddRange(uploads);
        
        return files.ToDictionary(x => x.Identifier);
    }

    private async Task<Dictionary<Guid, int>> ReadChangeCounts(BankState bank)
    {
        var changes = await changeReader.CollectAsync(bank.Identifier, BankHelper.AttachmentChangeTypes);
        var changeCounts = new Dictionary<Guid, int>();

        foreach (var change in changes)
        {
            var attachmentIdNames = GetAttachmentIdNames(change.ChangeType);
            var changeData = JsonConvert.DeserializeObject<Dictionary<string, object>>(change.ChangeData)!;

            foreach (var attachmentIdName in attachmentIdNames)
            {
                if (!changeData.TryGetValue(attachmentIdName, out var attachmentIdText) || attachmentIdText == null)
                    continue;

                var attachmentId = Guid.Parse(attachmentIdText.ToString()!);

                changeCounts.TryGetValue(attachmentId, out var count);

                changeCounts[attachmentId] = count + 1;
            }
        }

        return changeCounts;
    }

    private static string[] GetAttachmentIdNames(string changeType)
    {
        switch (changeType)
        {
            case nameof(AttachmentAdded):
                return [nameof(AttachmentAdded.Attachment)];
            case nameof(AttachmentAddedToQuestion):
                return [nameof(AttachmentAddedToQuestion.Attachment)];
            case nameof(AttachmentChanged):
                return [nameof(AttachmentChanged.Attachment)];
            case nameof(BankAttachmentDeleted):
                return [nameof(BankAttachmentDeleted.Attachment)];
            case nameof(AttachmentDeletedFromQuestion):
                return [nameof(AttachmentDeletedFromQuestion.Attachment)];
            case nameof(AttachmentImageChanged):
                return [nameof(AttachmentImageChanged.Attachment)];
            case nameof(AttachmentUpgraded):
                return [nameof(AttachmentUpgraded.CurrentAttachment), nameof(AttachmentUpgraded.UpgradedAttachment)];
            default:
                throw new ArgumentException($"Unsupported changeType: {changeType}");
        }
    }
}
