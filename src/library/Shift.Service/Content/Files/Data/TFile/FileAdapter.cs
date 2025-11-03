using Shift.Common;
using Shift.Contract;

namespace Shift.Service.Content;

public class FileAdapter : IEntityAdapter
{
    public void Copy(ModifyFile modify, FileEntity entity)
    {
        entity.UserIdentifier = modify.UserIdentifier;
        entity.OrganizationIdentifier = modify.OrganizationIdentifier;
        entity.ObjectType = modify.ObjectType;
        entity.ObjectIdentifier = modify.ObjectIdentifier;
        entity.FileName = modify.FileName;
        entity.FileSize = modify.FileSize;
        entity.FileLocation = modify.FileLocation;
        entity.FileUrl = modify.FileUrl;
        entity.FilePath = modify.FilePath;
        entity.FileContentType = modify.FileContentType;
        entity.FileUploaded = modify.FileUploaded;
        entity.DocumentName = modify.DocumentName;
        entity.FileDescription = modify.FileDescription;
        entity.FileCategory = modify.FileCategory;
        entity.FileSubcategory = modify.FileSubcategory;
        entity.FileStatus = modify.FileStatus;
        entity.FileExpiry = modify.FileExpiry;
        entity.FileReceived = modify.FileReceived;
        entity.FileAlternated = modify.FileAlternated;
        entity.LastActivityTime = modify.LastActivityTime;
        entity.LastActivityUserIdentifier = modify.LastActivityUserIdentifier;
        entity.ReviewedTime = modify.ReviewedTime;
        entity.ReviewedUserIdentifier = modify.ReviewedUserIdentifier;
        entity.ApprovedTime = modify.ApprovedTime;
        entity.ApprovedUserIdentifier = modify.ApprovedUserIdentifier;
    }

    public string Serialize<T>(IEnumerable<T> models, string format, string includes)
    {
        return format.ToLower() == "csv"
            ? CsvHelper.SerializeCsv(models, includes)
            : JsonHelper.SerializeJson(models, includes);
    }

    public FileEntity ToEntity(CreateFile create)
    {
        var entity = new FileEntity
        {
            UserIdentifier = create.UserIdentifier,
            OrganizationIdentifier = create.OrganizationIdentifier,
            ObjectType = create.ObjectType,
            ObjectIdentifier = create.ObjectIdentifier,
            FileIdentifier = create.FileIdentifier,
            FileName = create.FileName,
            FileSize = create.FileSize,
            FileLocation = create.FileLocation,
            FileUrl = create.FileUrl,
            FilePath = create.FilePath,
            FileContentType = create.FileContentType,
            FileUploaded = create.FileUploaded,
            DocumentName = create.DocumentName,
            FileDescription = create.FileDescription,
            FileCategory = create.FileCategory,
            FileSubcategory = create.FileSubcategory,
            FileStatus = create.FileStatus,
            FileExpiry = create.FileExpiry,
            FileReceived = create.FileReceived,
            FileAlternated = create.FileAlternated,
            LastActivityTime = create.LastActivityTime,
            LastActivityUserIdentifier = create.LastActivityUserIdentifier,
            ReviewedTime = create.ReviewedTime,
            ReviewedUserIdentifier = create.ReviewedUserIdentifier,
            ApprovedTime = create.ApprovedTime,
            ApprovedUserIdentifier = create.ApprovedUserIdentifier
        };
        return entity;
    }

    public IEnumerable<FileModel> ToModel(IEnumerable<FileEntity> entities)
    {
        return entities.Select(ToModel);
    }

    public FileModel ToModel(FileEntity entity)
    {
        var model = new FileModel
        {
            UserIdentifier = entity.UserIdentifier,
            ObjectType = entity.ObjectType,
            ObjectIdentifier = entity.ObjectIdentifier,
            FileIdentifier = entity.FileIdentifier,
            FileName = entity.FileName,
            FileSize = entity.FileSize,
            FileLocation = entity.FileLocation,
            FileUrl = entity.FileUrl,
            FilePath = entity.FilePath,
            FileContentType = entity.FileContentType,
            FileUploaded = entity.FileUploaded,
            DocumentName = entity.DocumentName,
            FileDescription = entity.FileDescription,
            FileCategory = entity.FileCategory,
            FileSubcategory = entity.FileSubcategory,
            FileStatus = entity.FileStatus,
            FileExpiry = entity.FileExpiry,
            FileReceived = entity.FileReceived,
            FileAlternated = entity.FileAlternated,
            LastActivityTime = entity.LastActivityTime,
            LastActivityUserIdentifier = entity.LastActivityUserIdentifier,
            ReviewedTime = entity.ReviewedTime,
            ReviewedUserIdentifier = entity.ReviewedUserIdentifier,
            ApprovedTime = entity.ApprovedTime,
            ApprovedUserIdentifier = entity.ApprovedUserIdentifier
        };

        return model;
    }

    public IEnumerable<FileMatch> ToMatch(IEnumerable<FileEntity> entities)
    {
        return entities.Select(ToMatch);
    }

    public FileMatch ToMatch(FileEntity entity)
    {
        var match = new FileMatch
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            OrganizationCode = entity.Organization.OrganizationCode,
            ObjectType = entity.ObjectType,
            ObjectIdentifier = entity.ObjectIdentifier,
            FileIdentifier = entity.FileIdentifier,
            FileLocation = entity.FileLocation,
            FileName = entity.FileName,
            DocumentName = entity.DocumentName,
            FileSize = entity.FileSize,
            FileUploaded = entity.FileUploaded,
            UserIdentifier = entity.UserIdentifier,
            UserFullName = entity.User.FullName,
            HasClaims = entity.Claims.Any()
        };

        return match;
    }

    public IEnumerable<FileDownload> ToDownload(IEnumerable<FileEntity> entities)
    {
        return entities.Select(ToDownload);
    }

    public FileDownload ToDownload(FileEntity entity)
    {
        var match = new FileDownload
        {
            OrganizationIdentifier = entity.OrganizationIdentifier,
            OrganizationCode = entity.Organization.OrganizationCode,
            ObjectType = entity.ObjectType,
            ObjectIdentifier = entity.ObjectIdentifier,
            FileIdentifier = entity.FileIdentifier,
            FileLocation = entity.FileLocation,
            FileName = entity.FileName,
            DocumentName = entity.DocumentName,
            FileSize = entity.FileSize,
            FileUploaded = entity.FileUploaded,
            UserIdentifier = entity.UserIdentifier,
            UserFullName = entity.User.FullName,
            Visibility = entity.Claims.Any() ? "Private" : "Public"
        };

        return match;
    }
}