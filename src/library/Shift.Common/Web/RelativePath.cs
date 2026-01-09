using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class RelativePath
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public char Separator { get; set; }

        public bool IsCurrentRelative
            => Value.IsNotEmpty() && !IsRootRelative;

        public bool IsRootRelative
            => Value.IsNotEmpty() && Value.StartsWith(Separator.ToString());

        public RelativePath(string name, string value, char separator = '/')
            : this(value, separator)
        {
            Name = name;
        }

        public RelativePath(string value, char separator = '/')
        {
            Value = value;
            Separator = separator;
        }

        public int CountSegments()
            => Value.IsEmpty() ? 0 : 1 + Value.Count(c => c == Separator);

        public bool HasSegments()
            => CountSegments() > 0;

        public void RemoveLastSegment()
        {
            if (!HasSegments())
                return;

            var trimmedUrl = Value.TrimEnd(Separator);

            var lastSlashIndex = trimmedUrl.LastIndexOf(Separator);

            if (lastSlashIndex > -1)
                Value = trimmedUrl.Substring(0, lastSlashIndex);
            else
                Value = null;
        }

        public RelativePath Clone()
        {
            return new RelativePath(Name, Value, Separator);
        }
    }

    public class RelativePathCollection
    {
        private readonly List<RelativePath> _paths = new List<RelativePath>();

        private readonly List<RelativePath> _subpaths = new List<RelativePath>();

        public int Count() => _paths.Count + _subpaths.Count;

        public List<RelativePath> Items => _paths.Union(_subpaths).ToList();

        public RelativePathCollection(List<RelativePath> list)
        {
            foreach (var item in list)
            {
                var path = item.Clone();

                _paths.Add(path);

                while (path.HasSegments())
                {
                    path.RemoveLastSegment();

                    if (path.Value.IsNotEmpty() && !_subpaths.Any(x => x.Value == path.Value))
                    {
                        _subpaths.Add(new RelativePath(path.Value, path.Value, '.'));
                    }
                }
            }
        }
    }
}