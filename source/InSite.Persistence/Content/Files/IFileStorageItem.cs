using System;
using System.IO;

namespace InSite.Persistence
{
    public interface IFileStorageItem
    {
        Guid FileId { get; }
        Guid OrganizationIdentifier { get; }
        string Path { get; }
        int ContentSize { get; }
        string DataFingerprint { get; }

        Guid? Uploader { get; }
        DateTimeOffset Uploaded { get; }

        bool IsNew { get; }

        Stream Read();
        void Write(Stream stream, bool isCheckFileSizeLimits);
        void Write(Action<Stream> write);
        void Save();
        void Delete();
    }
}
