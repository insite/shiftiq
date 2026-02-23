using System.Linq;

using InSite.Application.Files.Read;
using InSite.Application.Pages.Write;

using Shift.Common.Timeline.Changes;
using Shift.Common;
using Shift.Common.Timeline.Commands;
using Shift.Sdk.Service;
using Shift.Service.Workspace;

using IContentModifyService = Shift.Contract.IContentModifyService;
using PageModel = Shift.Contract.PageModel;
using PageContentModifyModel = Shift.Contract.PageContentModifyModel;
using CollectPages = Shift.Contract.CollectPages;

namespace Shift.Service.Content.PageContents;

public class ContentModifyService(
    PageReader pageReader,
    FileReader fileReader,
    ICommanderAsync commander,
    IStorageServiceAsync storageService
) : IContentModifyService
{
    private static readonly HashSet<string> _blockTypes = new()
    {
        "HeadingAndParagraphs",
        "HeadingAndParagraphsWithImage",
        "ImageGallery",
        "TwoColumns",
        "LinkToAchievement",
        "LinkToAssessment",
        "LinkToCourse",
        "LinkToForm",
        "CourseSummary",  
    };

    public async Task ModifyPageContentAsync(PageModel pageModel, PageContentModifyModel modifyModel, string userFullName)
    {
        var contentFields = pageModel.ContentLabelsToArray();

        var commands = new List<ICommand>();
        var contents = new List<ContentContainer>();

        AddPageContentCommands(pageModel, modifyModel, contentFields, commands, contents);

        if (modifyModel.Blocks != null && contentFields.Any(x => x.Equals("PageBlocks", StringComparison.OrdinalIgnoreCase)))
            await AddBlockCommands(pageModel, modifyModel, userFullName, commands, contents);

        await new ContentImageProcessor(storageService, fileReader).SaveImages(pageModel.PageId, contents);

        await commander.SendCommandsAsync(commands);
    }

    private static void AddPageContentCommands(
        PageModel pageModel,
        PageContentModifyModel modifyModel,
        string[] contentFields,
        List<ICommand> commands,
        List<ContentContainer> contents
    )
    {
        if (modifyModel.Content == null)
            return;

        var src = modifyModel.Content;
        var dst = new ContentContainer();

        foreach (var name in contentFields)
        {
            var srcItem = src[name];
            var dstItem = dst[name];

            if (name == "HtmlHead")
            {
                dstItem.Html = srcItem?.Html;
            }
            else if (name != "PageBlocks")
            {
                dstItem.Html = srcItem?.Html;
                dstItem.Text = srcItem?.Text;
            }
        }

        contents.Add(dst);

        var newTitle = src.Title?.Text?.Default;
        if (!string.Equals(pageModel.Title, newTitle))
            commands.Add(new ChangePageTitle(pageModel.PageId, newTitle));

        commands.Add(new ChangePageContent(pageModel.PageId, dst));
    }

    private async Task AddBlockCommands(
        PageModel pageModel,
        PageContentModifyModel modifyModel,
        string userFullName,
        List<ICommand> commands,
        List<ContentContainer> contents
    )
    {
        var blockPages = await pageReader.CollectAsync(new CollectPages { ParentPageId = pageModel.PageId });

        await AddBlockDeleteCommands(modifyModel, blockPages, commands);

        AddBlockModifyCommands(pageModel, modifyModel, userFullName, blockPages, commands, contents);
    }

    private async Task AddBlockDeleteCommands(PageContentModifyModel modifyModel, List<PageEntity> blockPages, List<ICommand> commands)
    {
        if (modifyModel.DeletedBlockIds == null || modifyModel.DeletedBlockIds.Length == 0)
            return;

        foreach (var blockId in modifyModel.DeletedBlockIds)
        {
            if (!blockPages.Any(x => x.PageIdentifier == blockId))
                continue;

            commands.Add(new DeletePage(blockId));

            var children = await pageReader.CollectChildrenAsync(blockId);
            foreach (var child in children)
                commands.Add(new DeletePage(child.PageIdentifier));
        }
    }

    private static void AddBlockModifyCommands(
        PageModel pageModel,
        PageContentModifyModel modifyModel,
        string userFullName,
        List<PageEntity> blockPages,
        List<ICommand> commands,
        List<ContentContainer> contents
    )
    {
        foreach (var blockModel in modifyModel.Blocks)
        {
            var isNew = blockModel.BlockId == Guid.Empty;
            var blockPage = blockPages.Find(x => x.PageIdentifier == blockModel.BlockId);
            if (blockPage == null && !isNew)
                continue;

                var blockId = isNew ? UniqueIdentifier.Create() : blockModel.BlockId;

            if (isNew)
            {
                if (!_blockTypes.Contains(blockModel.BlockType))
                    throw new ArgumentException($"Invalid block type: {blockModel.BlockType}");

                var sequence = blockPages.Count > 0 ? blockPages.Max(x => x.Sequence) + 1 : 1;

                commands.Add(new CreatePage(blockId, pageModel.SiteId, pageModel.PageId, pageModel.OrganizationId, Guid.Empty, blockModel.Title, "Block", sequence, false, false));
                commands.Add(new ChangePageContentControl(blockId, "InSite.UI.Layout.Common.Controls." + blockModel.BlockType));
                commands.Add(new ChangePageContentLabels(blockId, "PageBlocks, Body, Title, Summary, ImageURL"));
                commands.Add(new ChangePageAuthorName(blockId, userFullName));
                commands.Add(new ChangePageAuthorDate(blockId, DateTimeOffset.UtcNow));
            }
            else if (!string.Equals(blockPage!.Title, blockModel.Title))
                commands.Add(new ChangePageTitle(blockId, blockModel.Title));

            var slug = StringHelper.Sanitize(blockModel.Title, '-', true, ['_']);
            if (!string.Equals(blockPage?.PageSlug, slug))
                commands.Add(new ChangePageSlug(blockId, slug));

            if (!string.Equals(blockPage?.Hook ?? "", blockModel.Hook ?? ""))
                commands.Add(new ChangePageHook(blockId, !string.IsNullOrEmpty(blockModel.Hook) ? blockModel.Hook : null));

            if (blockModel.Content != null)
            {
                var content = BlockContentProcessor.GetBlockContent(blockModel.Content);

                contents.Add(content);
                commands.Add(new ChangePageContent(blockId, content));
            }
        }
    }
}