using InSite.Application.Banks.Write;
using InSite.Application.Files.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Integration.ImageMagick;
using Shift.Common.Timeline.Commands;
using Shift.Constant;
using Shift.Contract;

using Shift.Service.Content;
using Shift.Toolbox;

namespace Shift.Service.Evaluation.Workshops;

internal class QuestionFileProcessor(IStorageServiceAsync storageService, FileReader fileReader, ISequence sequence)
{
    public async Task SaveFiles(Guid bankId, Guid organizationId, MultilingualString title, List<ICommand> commands)
    {
        var ids = ParseFiles(title);
        if (ids.Count == 0)
            return;

        var criteria = new SearchFiles
        {
            FileIds = ids.ToArray(),
            ObjectId = ObjectIdentifiers.Temporary
        };
        criteria.Filter.Page = 0;

        var files = await fileReader.CollectAsync(criteria);

        foreach (var file in files)
            commands.Add(await CreateAttachmentCommandAsync(bankId, organizationId, file));

        foreach (var file in files)
            await storageService.ChangeObjectAsync(file.FileIdentifier, bankId, FileObjectType.Bank);
    }

    private HashSet<Guid> ParseFiles(MultilingualString title)
    {
        var ids = new HashSet<Guid>();

        foreach (var language in title.Languages)
        {
            var subList = storageService.ExtractAndParseFileUrls(title[language]);
            foreach (var (fileId, _) in subList)
                ids.Add(fileId);
        }

        return ids;
    }

    private async Task<AddAttachment> CreateAttachmentCommandAsync(Guid bankId, Guid organizationId, FileEntity file)
    {
        var attachment = new Attachment
        {
            Identifier = UniqueIdentifier.Create(),
            Asset = await sequence.IncrementAsync(organizationId, SequenceType.Asset),
            Author = file.UserIdentifier,
            Type = Attachment.GetAttachmentType(Path.GetExtension(file.FileName)),
            FileIdentifier = file.FileIdentifier,
            Upload = Guid.Empty
        };

        attachment.Content.Title.Default = file.DocumentName;

        if (attachment.Type == AttachmentType.Image)
        {
            var (_, stream) = await storageService.GetFileStreamAsync(file.FileIdentifier);
            using (stream)
                attachment.Image = ReadImageProps(stream);
        }

        return new AddAttachment(bankId, attachment);
    }

    private static AttachmentImage ReadImageProps(Stream stream)
    {
        try
        {
            var imgInfo = ImageHelper.ReadInfo(stream);

            return new AttachmentImage
            {
                Actual = new ImageDimension
                {
                    Height = (int)imgInfo.Height,
                    Width = (int)imgInfo.Width,
                },
                IsColor = imgInfo.ColorSpace != ColorSpace.Gray,
                Resolution = (int)Math.Round(imgInfo.PixelsPerInch, MidpointRounding.AwayFromZero)
            };
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("The image is corrupted", ex);
        }
    }
}
