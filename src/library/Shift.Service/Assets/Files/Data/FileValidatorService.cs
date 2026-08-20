using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Exceptions;
using Shift.Common.Integration.ImageMagick;
using Shift.Service.Security;
using Shift.Toolbox;

namespace Shift.Service.Content;

public class FileValidatorService(
    OrganizationService organizationService,
    OrganizationAdapter organizationAdapter
)
{
    public record AdjustResult(string? NewFileExtension, string[] Messages, Stream Stream);

    public async Task ValidateAsync(Guid organizationId, IEnumerable<FileEntity> files)
    {
        var uploadSettings = await GetUploadSettingsAsync(organizationId);

        foreach (var file in files)
        {
            var imageType = GetImageType(file.FileName);

            if (imageType != ImageType.Null)
            {
                var limit = uploadSettings.Images.MaximumFileSize;
                if (file.FileSize > limit)
                    throw MaxFileSizeExceeded("image", limit);
            }
            else if (file.FileSize > uploadSettings.Documents.MaximumFileSize)
            {
                var limit = uploadSettings.Documents.MaximumFileSize;
                throw MaxFileSizeExceeded("document", limit);
            }
        }        
    }

    public async Task<AdjustResult> ValidateAndAdjustAsync(Guid organizationId, string filePath, Stream stream)
    {
        var uploadSettings = await GetUploadSettingsAsync(organizationId);
        var imageType = GetImageType(filePath);

        if (imageType != ImageType.Null)
        {
            var limit = uploadSettings.Images.MaximumFileSize;
            if (stream.Length > limit)
                throw MaxFileSizeExceeded("image", limit);

            return WriteImage(stream, imageType, uploadSettings.Images.MaximumWidth, uploadSettings.Images.MaximumHeight);
        }

        if (stream.Length > uploadSettings.Documents.MaximumFileSize)
        {
            var limit = uploadSettings.Documents.MaximumFileSize;
            throw MaxFileSizeExceeded("document", limit);
        }

        return new AdjustResult(null, [], stream);
    }

    private static AdjustResult WriteImage(Stream inputStream, ImageType fileImageType, int maxImageWidth, int maxImageHeight)
    {
        try
        {
            var imageInfo = ImageHelper.ReadInfo(inputStream);
            var imageType = imageInfo.ImageType;
            var isTypeChanged = false;

            if (imageType != ImageType.Png && imageType != ImageType.Gif && imageType != ImageType.Jpeg)
            {
                imageType = imageInfo.ColorSpace == ColorSpace.Gray
                    ? ImageType.Png
                    : ImageType.Jpeg;

                isTypeChanged = true;
            }
            else if (fileImageType != imageType)
            {
                isTypeChanged = true;
            }

            var newFileExtension = isTypeChanged ? FileExtension.GetImageExtension(imageType) : null;

            var messages = new List<string>();
            var outputStream = new MemoryStream();
            
            ImageHelper.AdjustImage(inputStream, outputStream, imageType, false, messages, maxImageWidth, maxImageHeight);

            outputStream.Position = 0;

            return new AdjustResult(null, ConvertMessages(messages), outputStream);
        }
        catch
        {
            return new AdjustResult(null, [], inputStream);
        }
    }

    private static string[] ConvertMessages(List<string> messages)
    {
        if (messages.Count == 0)
            return [];

        var result = new List<string>();

        foreach (var message in messages)
        {
            if (!message.StartsWith("Warning: image resized"))
                continue;

            var sizeStartIndex = message.IndexOf('(') + 1;
            var sizeEndIndex = message.IndexOf(')', sizeStartIndex);
            var sizeStringParts = message.Substring(sizeStartIndex, sizeEndIndex - sizeStartIndex).Split([" -> "], StringSplitOptions.None);

            result.Add(
                $"The recommended maximum size for an upload image is {sizeStringParts[1]} pixels." +
                $"\r\nThe size of your image is {sizeStringParts[0]} pixels." +
                $"\r\nThe system has automatically scaled the image for you, but you may want to resize your own images in the future before you upload them.");
        }

        return result.ToArray();
    }

    private static ClientErrorException MaxFileSizeExceeded(string fileType, int fileSizeLimit)
    {
        var message = $"Your account is configured with a file size limit of {fileSizeLimit} per {fileType}. " +
            $"Please decrease the size of your file before uploading it, or contact your administrator to upgrade the settings for your account.";

        return new ClientErrorException(message);
    }

    private async Task<UploadSettings> GetUploadSettingsAsync(Guid organizationId)
    {
        var organization = await organizationService.RetrieveAsync(organizationId)
            ?? throw new ArgumentException($"Organization not found: {organizationId}");

        var organizationData = organizationAdapter.ToData(organization);

        return organizationData.PlatformCustomization?.UploadSettings ?? new UploadSettings();
    }

    private static ImageType GetImageType(string filePath)
    {
        var index = filePath.LastIndexOf('.');

        if (index < 0 || index == filePath.Length - 1)
            return ImageType.Null;

        var ext = filePath.Substring(index);

        return FileExtension.GetImageType(ext);
    }
}