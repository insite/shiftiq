namespace InSite.Application.Files.Read
{
    public interface IFileChangeFactory
    {
        FileChange[] CreateChanges(FileStorageModel oldModel, FileStorageModel newModel);
    }
}
