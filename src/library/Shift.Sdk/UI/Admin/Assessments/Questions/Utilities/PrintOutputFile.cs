namespace Shift.Sdk.UI
{
    public class PrintOutputFile
    {
        public string Name { get; }
        public string Ext { get; }
        public byte[] Data { get; }

        public PrintOutputFile(string name, byte[] data)
            : this(name, "pdf", data) { }

        public PrintOutputFile(string name, string ext, byte[] data)
        {
            Name = name;
            Ext = ext;
            Data = data;
        }
    }
}