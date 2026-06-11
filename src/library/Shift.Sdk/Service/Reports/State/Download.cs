using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ShiftHumanizer = Shift.Common.Humanizer;

namespace InSite.Domain.Reports
{
    public interface IDownload
    {
        string Format { get; set; }
        bool ShowHidden { get; set; }
        bool RemoveSpaces { get; set; }
    }

    [Serializable]
    public class Download : IDownload, ISearchReport
    {
        #region Properties

        public Guid? Identifier { get; set; }
        public string Name { get; set; }
        public string Format { get; set; }
        public bool ShowHidden { get; set; }
        public bool RemoveSpaces { get; set; }
        public DownloadColumnState[] Columns { get; set; }

        #endregion

        public Download()
        {
            Reset();
        }

        public virtual void Reset()
        {
            Format = "csv";
            ShowHidden = true;
            RemoveSpaces = false;
        }

        public void SortColumns()
        {
            Array.Sort(Columns);

            for (var i = 0; i < Columns.Length; i++)
                Columns[i].Sequence = i + 1;
        }
    }

    [Serializable]
    public class DownloadColumnState : IComparable, IComparable<DownloadColumnState>
    {
        public int Sequence { get; set; }

        public bool Hidden { get; set; }

        [JsonProperty]
        public int GroupKey { get; private set; }

        [JsonIgnore]
        public DownloadColumnGroup Group
        {
            get => _group;
            set
            {
                if (value != null && (_group == null || _group.GroupKey == DownloadColumnGroup.Empty.GroupKey))
                    SetGroup(value);
            }
        }

        [JsonProperty]
        public DownloadColumn Info { get; private set; }

        [NonSerialized]
        private DownloadColumnGroup _group;

        [JsonConstructor]
        private DownloadColumnState()
        {

        }

        public DownloadColumnState(DownloadColumn info)
            : this()
        {
            Info = info;

            SetGroup(DownloadColumnGroup.Empty);
        }

        private void SetGroup(DownloadColumnGroup value)
        {
            _group = value;
            GroupKey = _group.GroupKey;
        }

        public int CompareTo(object obj)
        {
            if (obj is DownloadColumnState other)
                return CompareTo(other);

            throw new ArgumentException("Invalid type of object: " + obj.GetType().FullName);
        }

        public int CompareTo(DownloadColumnState other)
        {
            var result = Group.CompareTo(other.Group);
            if (result != 0)
                return result;

            result = Sequence.CompareTo(other.Sequence);
            if (result != 0)
                return result;

            return Info.Name.CompareTo(other.Info.Name);
        }

        public DownloadColumnState Clone()
        {
            var result = new DownloadColumnState();

            this.ShallowCopyTo(result);

            result.Info = Info.Clone();
            result.SetGroup(_group);

            return result;
        }
    }

    public sealed class DownloadColumnGroup : IComparable, IComparable<DownloadColumnGroup>
    {
        public static readonly DownloadColumnGroup Empty = new DownloadColumnGroup(-1, string.Empty, int.MinValue, -1, string.Empty);

        public int TypeSequence { get; }
        public string TypeName { get; }

        public int GroupKey { get; }
        public int GroupSequence { get; }
        public string GroupName { get; }

        public DownloadColumnGroup(int typeSequence, string typeName, int groupKey, int groupSequence, string groupName)
        {
            TypeSequence = typeSequence;
            TypeName = typeName;

            GroupKey = groupKey;
            GroupSequence = groupSequence;
            GroupName = groupName;
        }

        public int CompareTo(object obj)
        {
            if (obj is DownloadColumnGroup other)
                return CompareTo(other);

            throw new ArgumentException("Invalid type of object: " + obj.GetType().FullName);
        }

        public int CompareTo(DownloadColumnGroup other)
        {
            var result = TypeSequence.CompareTo(other.TypeSequence);
            if (result != 0)
                return result;

            result = TypeName.CompareTo(other.TypeName);
            if (result != 0)
                return result;

            result = GroupSequence.CompareTo(other.GroupSequence);
            if (result != 0)
                return result;

            return GroupName.CompareTo(other.GroupName);
        }
    }

    [Serializable]
    public sealed class DownloadColumn
    {
        public string Name { get; private set; }
        public string Title { get; set; }
        public string Format { get; set; }
        public double? Width { get; set; }
        public HorizontalAlignment? Align { get; set; }
        public bool Visible { get; set; }

        private DownloadColumn()
        {

        }

        public DownloadColumn(string name, string title = null, string format = null, double? width = null, HorizontalAlignment? align = null)
        {
            Name = name;
            Title = string.IsNullOrEmpty(title) ? ShiftHumanizer.TitleCase(name) : title;
            Format = format;
            Width = width;
            Align = align;
            Visible = true;
        }

        public DownloadColumn Clone()
        {
            var result = new DownloadColumn { Name = Name };

            this.ShallowCopyTo(result);

            return result;
        }
    }
}
