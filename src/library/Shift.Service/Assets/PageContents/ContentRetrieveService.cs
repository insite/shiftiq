using Shift.Common;
using Shift.Contract;
using Shift.Service.Workspace;

namespace Shift.Service.Content.PageContents;

public class ContentRetrieveService(
    TInputReader inputReader,
    PageReader pageReader
) : IContentRetrieveService
{
    public async Task<PageContentModel> RetrievePageContentAsync(PageModel pageModel)
    {
        var criteria = new CollectPages { ParentPageId = pageModel.PageId };
        criteria.DisablePaging();

        var blockPages = (await pageReader.CollectAsync(criteria))
            .Where(x => x.ContentControl != null)
            .OrderBy(x => x.Sequence)
            .ToList();

        var containerIds = blockPages.Select(x => x.PageIdentifier).Union([ pageModel.PageId ]).ToArray();
        var contents = await CollectContentsAsync(containerIds);

        var (contentFields, content) = GetPageDetails(pageModel, contents);
        var blocks = GetBlocks(blockPages, contents);

        return new PageContentModel
        {
            Title = pageModel.Title,
            ContentFields = contentFields,
            Content = content,
            Blocks = blocks
        };
    }

    private async Task<Dictionary<Guid, ContentContainer>> CollectContentsAsync(Guid[] containerIds)
    {
        var criteria = new CollectInputs
        {
            ContainerIds = containerIds
        };

        criteria.DisablePaging();

        var inputs = await inputReader.CollectAsync(criteria);

        var result = new Dictionary<Guid, ContentContainer>();

        foreach (var input in inputs)
        {
            if (string.IsNullOrEmpty(input.ContentLabel))
                continue;

            if (!result.TryGetValue(input.ContainerIdentifier, out var content))
                result.Add(input.ContainerIdentifier, content = new ContentContainer());

            var item = content[input.ContentLabel];

            item.Html[input.ContentLanguage] = input.ContentHtml;
            item.Text[input.ContentLanguage] = input.ContentText;
            item.Snip[input.ContentLanguage] = input.ContentSnip;
        }

        return result;
    }

    private static (string[], ContentContainer) GetPageDetails(PageModel model, Dictionary<Guid, ContentContainer> contents)
    {
        var contentFields = model.ContentLabelsToArray();

        if (!contents.TryGetValue(model.PageId, out var content))
            content = new ContentContainer();
        else
        {
            var labels = content.GetLabels().ToList();
            foreach (var label in labels)
            {
                if (!contentFields.Any(x => x.Equals(label, StringComparison.OrdinalIgnoreCase)))
                    content.Remove(label);
            }
        }

        return (contentFields, content);
    }

    private static string[] _blockTypes = [
        "HeadingAndParagraphs",
        "HeadingAndParagraphsWithImage",
        "ImageGallery",
        "TwoColumns",
        "LinkToAchievement",
        "LinkToAssessment",
        "LinkToCourse",
        "LinkToForm",
        "CourseSummary"
    ];

    private static BlockContentModel[] GetBlocks(List<PageEntity> blockPages, Dictionary<Guid, ContentContainer> contents)
    {
        return blockPages
            .Select(x => new BlockContentModel
            {
                BlockId = x.PageIdentifier,
                Title = x.Title,
                Hook = x.Hook,
                BlockType = x.ContentControl != null ? x.ContentControl.Split(".").Last() : "Unknown",
                Content = contents.TryGetValue(x.PageIdentifier, out var content) ? content : new ContentContainer()
            })
            .Where(x => _blockTypes.Contains(x.BlockType))
            .ToArray();
    }
}