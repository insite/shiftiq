using System.Collections.Generic;

using InSite.Application.Files.Read;
using InSite.Domain.Reports;

using Shift.Common;
using Shift.Contract;
using Shift.Service.Content;

namespace Shift.Service.Content.PageContents;

internal class ContentImageProcessor(IStorageServiceAsync storageService, FileReader fileReader)
{
    public async Task SaveImages(Guid pageId, List<ContentContainer> contents)
    {
        var ids = new HashSet<Guid>();
        foreach (var content in contents)
            AddNewImages(content, ids);

        if (ids.Count == 0)
            return;

        var files = await fileReader.CollectAsync(new SearchFiles
        {
            FileIds = ids.ToArray(),
            ObjectId = ObjectIdentifiers.Temporary
        });

        foreach (var file in files)
            await storageService.ChangeObjectAsync(file.FileIdentifier, pageId, FileObjectType.Page);
    }

    private void AddNewImages(ContentContainer content, HashSet<Guid> ids)
    {
        foreach (var label in content.GetLabels())
        {
            var item = content[label];

            foreach (var language in item.Languages)
            {
                AddNewImages(item.GetHtml(label), ids);
                AddNewImages(item.GetText(label), ids);
            }
        }
    }

    private void AddNewImages(string s, HashSet<Guid> ids)
    {
        var subList = storageService.ExtractAndParseFileUrls(s);
        foreach (var (fileId, _) in subList)
            ids.Add(fileId);
    }

    private async Task<List<FileEntity>> CollectTempFiles(List<(Guid FileId, string FileName)> list)
    {
        var ids = list.Select(x => x.FileId).ToArray();

        return await fileReader.CollectAsync(new SearchFiles
        {
            FileIds = ids,
            ObjectId = ObjectIdentifiers.Temporary
        });
    }
}
