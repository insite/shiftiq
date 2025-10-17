namespace Shift.Sdk.UI
{
    public class PrintOutputFile
    {
        public string Name { get; }
        public byte[] Data { get; }

        public PrintOutputFile(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }
}