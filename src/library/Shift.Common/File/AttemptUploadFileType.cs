namespace Shift.Common.File
{
    public class AttemptUploadFileType
    {
        public int ID { get; }
        public string Title { get; }

        public AttemptUploadFileType(int id, string title)
        {
            ID = id;
            Title = title;
        }
    }
}
