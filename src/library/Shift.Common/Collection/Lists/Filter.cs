using System;
using System.Collections.ObjectModel;

namespace Shift.Common
{
    [Serializable]
    public abstract class Filter : ISearchReport
    {
        [Serializable]
        public class ColumnCollection : Collection<string> { }

        Guid? ISearchReport.Identifier
        {
            get => FilterIdentifier;
            set => FilterIdentifier = value;
        }

        string ISearchReport.Name
        {
            get => FilterName;
            set => FilterName = value;
        }

        public bool AdvancedOptionsVisible { get; set; }
        public bool IsFilterClean { get; set; }
        public bool IsSchemaOnly { get; set; }
        public Guid? FilterIdentifier { get; set; }
        public string FilterName { get; set; }

        public string OrderBy { get; set; }
        public Paging Paging { get; set; }

        public ColumnCollection ShowColumns { get; private set; }

        protected Filter()
        {
            ShowColumns = new ColumnCollection();
        }
    }
}