using System.Text.RegularExpressions;

using InSite.Application.Files.Read;
using InSite.Domain.Banks;

using Shift.Common;

using Shift.Constant;

using Shift.Contract;
using Shift.Sdk.Service;

using Shift.Service.Content;

namespace Shift.Service.Evaluation.Workshops;

public class WorkshopImageListService(
    AppSettings appSettings,
    ITimelineQuery timelineQuery,
    UploadReader uploadReader,
    IFileSearchAsync fileSearch,
    IStorageServiceAsync storageService
) : IWorkshopImageListService
{
    private class FileInfo
    {
        public Guid Identifier { get; set; }
        public string? FileName { get; set; }
        public string? NavigateUrl { get; set; }
    }

    private static readonly Regex MarkdownImagePattern = new Regex(
        @"!\[(?<Alt>.*?)]\((?<Url>.*?)(?:\s""(?<Title>.*)"")?\)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex UrlPattern = new Regex(
        @"https://(?:(?<Environment>dev|local|sandbox)-)?(?<Organization>.*)\.(?<Host>(?:insite|keyeracmds|shiftiq)\.com)/files(?<Path>/[^\?]+)(?<Query>\?.*)?",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex EnvironmentPattern = new Regex(
        @"https://(?:(?<Environment>dev|local|sandbox)-)?(?<Organization>.*)\.",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public async Task<WorkshopImage[]> CollectImagesAsync(IPrincipal principal, Guid bankId)
    {
        var bank = await timelineQuery.GetAggregateStateAsync<BankAggregate, BankState>(bankId);

        var attachments = bank.EnumerateAllAttachments().GroupBy(x => x.FileIdentifier ?? x.Upload).ToDictionary(x => x.Key, x => x.First());
        var images = new Dictionary<string, WorkshopImage>(StringComparer.OrdinalIgnoreCase);

        await AddImagesFromAttachmentsAsync(principal, bank, attachments, images);

        AddImagesFromContent(principal, bank, attachments, images);

        var result = images
            .Select(x => x.Value)
            .OrderBy(x => x.Attachment != null ? 0 : 1)
            .ThenBy(x => (x.Attachment?.Title).IfNullOrEmpty(x.FileName))
            .ToArray();

        return result;
    }

    private async Task AddImagesFromAttachmentsAsync(IPrincipal principal, BankState bank, Dictionary<Guid, Attachment> attachments, Dictionary<string, WorkshopImage> images)
    {
        var files = await GetFilesAsync(principal, bank);

        foreach (var file in files)
        {
            if (!attachments.TryGetValue(file.Identifier, out var attachment))
                throw new ArgumentNullException("attachment");

            var image = CreateUploadImage(principal, file, attachment);

            images.Add(image.Url, image);
        }        
    }

    private void AddImagesFromContent(IPrincipal principal, BankState bank, Dictionary<Guid, Attachment> attachments, Dictionary<string, WorkshopImage> images)
    {
        foreach (var question in bank.Sets.SelectMany(x => x.EnumerateAllQuestions()))
        {
            var title = question.Content.Title?.Default;
            if (string.IsNullOrEmpty(title))
                continue;

            var matches = MarkdownImagePattern.Matches(title);
            if (matches.Count == 0)
                continue;

            foreach (Match match in matches)
            {
                var url = match.Groups["Url"].Value;
                var files = storageService.ExtractAndParseFileUrls(url);
                
                if (files.Count > 0 && attachments.ContainsKey(files[0].FileIdentifier))
                    continue;

                var image = CreateUploadImage(principal, new FileInfo { NavigateUrl = url }, null);

                images.TryAdd(image.Url, image);
            }
        }
    }

    private async Task<List<FileInfo>> GetFilesAsync(IPrincipal principal, BankState bank)
    {
        var attachments = bank.EnumerateAllAttachments().Where(x => x.Type == AttachmentType.Image).ToList();

        var uploadIds = attachments.Where(x => x.Upload != Guid.Empty).Select(x => x.Upload).ToArray();
        var uploads = (await uploadReader.CollectAsync(bank.Tenant, uploadIds))
            .Select(x => new FileInfo
            {
                Identifier = x.UploadIdentifier,
                NavigateUrl = GetAbsoluteUrl(principal, "/files" + x.NavigateUrl)
            })
            .ToList();

        var fileIds = attachments.Where(x => x.FileIdentifier.HasValue).Select(x => x.FileIdentifier!.Value).ToArray();

        var files = (await fileSearch.GetModelsAsync(fileIds, false))
            .Select(x => new FileInfo
            {
                Identifier = x.FileIdentifier,
                FileName = x.FileName,
            })
            .ToList();

        files.AddRange(uploads);

        return files;
    }

    private WorkshopImage CreateUploadImage(IPrincipal principal, FileInfo fileInfo, Attachment? attachment)
    {
        var attachmentInfo = attachment != null ? CreateAttachmentInfo(attachment) : null;

        if (!string.IsNullOrEmpty(fileInfo.FileName))
        {
            var fileUrl = storageService.GetFileUrl(fileInfo.Identifier, fileInfo.FileName, false, true);

            return new WorkshopImage
            {
                FileName = fileInfo.FileName,
                Url = GetAbsoluteUrl(principal, fileUrl),
                Environment = appSettings.Environment.Name,
                Attachment = attachmentInfo
            };
        }

        if (string.IsNullOrEmpty(fileInfo.NavigateUrl))
            throw new ArgumentNullException("fileInfo.NavigateUrl");

        var image = UrlPattern.Match(fileInfo.NavigateUrl);
        EnvironmentName environment = EnvironmentName.External;

        if (image.Success)
        {
            var environmentResult = image.Groups["Environment"];

            if (!environmentResult.Success)
                environment = EnvironmentName.Production;
            else if (environmentResult.Value == "dev")
                environment = EnvironmentName.Development;
            else
                environment = environmentResult.Value.ToEnum<EnvironmentName>(true);
        }

        var query = image.Groups["Query"];

        var url = query.Success
            ? fileInfo.NavigateUrl.Substring(0, fileInfo.NavigateUrl.Length - query.Length)
            : fileInfo.NavigateUrl;

        var path = image.Groups["Path"].Value;
        string name = "";

        if (path.IndexOfAny(System.IO.Path.GetInvalidPathChars()) < 0)
            name = System.IO.Path.GetFileName(path);

        if (string.IsNullOrEmpty(name))
            name = url;

        return new WorkshopImage
        {
            FileName = name,
            Url = url,
            Environment = environment,
            Attachment = attachmentInfo
        };
    }

    private string GetAbsoluteUrl(IPrincipal principal, string relativeUrl)
    {
        return UrlHelper.GetAbsoluteUrl(appSettings.Partition.Domain, appSettings.Environment.Name, relativeUrl, principal.Organization.Slug);
    }

    private static WorkshopImage.AttachmentInfo CreateAttachmentInfo(Attachment attachment)
    {
        return new WorkshopImage.AttachmentInfo
        {
            Title = (attachment.Content?.Title.Default).IfNullOrEmpty("(Untitled)"),
            Number = $"{attachment.Asset}.{attachment.AssetVersion}",
            Condition = attachment.Condition,
            PublicationStatus = attachment.PublicationStatus.GetDescription(),
            Dimension = attachment.Image != null ? $"{attachment.Image.Actual.Width} x {attachment.Image.Actual.Height}" : "0 x 0"
        };
    }
}
