using System;

namespace Shift.Sdk.UI
{
    public class BlockContentControlTypeInfo
    {
        public int Sequence { get; }
        public string Name { get; }
        public string Title { get; }
        public string Path { get; }
        public string[] Labels { get; }

        public BlockContentControlTypeInfo(int sequence, Type type, string title, string path, string[] labels)
        {
            Sequence = sequence;
            Name = type.FullName;
            Title = title;
            Path = path;
            Labels = labels;
        }
    }
}