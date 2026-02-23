using Shift.Common;

namespace Shift.Service.Content.PageContents;

internal static class BlockContentProcessor
{
    private enum BlockFieldType { Html, Image, ImageList, Text };

    private static readonly Dictionary<string, BlockFieldType> _blockFields = new()
    {
        { "Body", BlockFieldType.Html },
        { "Column 1", BlockFieldType.Html },
        { "Column 2", BlockFieldType.Html },
        { "Description", BlockFieldType.Html },
        { "Paragraphs", BlockFieldType.Html },
        { "Image URL", BlockFieldType.Image },
        { "Image List", BlockFieldType.ImageList },
        { "Heading", BlockFieldType.Text },
        { "Link Target", BlockFieldType.Text },
        { "Title", BlockFieldType.Text },
        { "Time Required", BlockFieldType.Text },
        { "Start URL", BlockFieldType.Text },
    };

    public static ContentContainer GetBlockContent(ContentContainer src)
    {
        var dst = new ContentContainer();
        var processedFields = new HashSet<string>();

        foreach (var label in dst.GetLabels())
        {
            var fieldName = label.Split(':')[0];
            if (processedFields.Contains(fieldName))
                continue;

            if (!_blockFields.TryGetValue(fieldName, out var blockFieldType))
                throw new ArgumentException($"Non-supported field: {label}");

            switch (blockFieldType)
            {
                case BlockFieldType.Html:
                    dst[fieldName] = src[fieldName];
                    break;
                case BlockFieldType.Image:
                    dst[fieldName + ":Alt"] = src[fieldName + ":Alt"];
                    dst[fieldName + ":Url"] = src[fieldName + ":Url"];
                    break;
                case BlockFieldType.ImageList:
                    CopyImageList(src, dst, fieldName);
                    break;
                case BlockFieldType.Text:
                    dst[fieldName].Text.Default = src[fieldName].Text.Default;
                    break;
                default:
                    throw new ArgumentException($"BlockFieldType: {blockFieldType}");
            }

            processedFields.Add(fieldName);
        }

        return dst;
    }

    private static void CopyImageList(ContentContainer src, ContentContainer dst, string fieldName)
    {
        for (int i = 0; i < 10000; i++)
        {
            var alt = src[$"{fieldName}:{i}.Alt"];
            var url = src[$"{fieldName}:{i}.Url"];

            if (alt.IsEmpty && url.IsEmpty)
                return;

            dst[$"{fieldName}:{i}.Alt"] = alt;
            dst[$"{fieldName}:{i}.Url"] = url;
        }

        throw new ArgumentException("Number of images in the list is greater than 10000");
    }
}
