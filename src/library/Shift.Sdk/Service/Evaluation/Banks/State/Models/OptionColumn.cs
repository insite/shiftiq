using System;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// The options contained by a question can be displayed in a multi-column table. This class represents a column in
    /// such a table.
    /// </summary>
    [Serializable]
    public class OptionColumn
    {
        /// <summary>
        /// Are the cells in this column (including the header and the rows) justified left, right, or center?
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public HorizontalAlignment Alignment { get; set; }

        /// <summary>
        /// The CSS class used to control layout and style for the column.
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// The column needs support for a multilingual title (i.e. column header).
        /// </summary>
        public ContentTitle Content { get; set; }

        /// <summary>
        /// Constructs an empty column.
        /// </summary>
        public OptionColumn()
        {
            Content = new ContentTitle();
        }

        /// <summary>
        /// Constructs an initialized column.
        /// </summary>
        public OptionColumn(string title, string alignment, string cssClass)
            : this(title, default(HorizontalAlignment), cssClass)
        {
            Alignment = alignment.ToEnum(ParseMarkdownAlignment);
        }

        /// <summary>
        /// Constructs an initialized column.
        /// </summary>
        public OptionColumn(string title, HorizontalAlignment alignment, string cssClass)
            : this()
        {
            if (title != null)
                title = title.Trim();

            if (cssClass != null)
                cssClass = cssClass.Trim();

            if (!string.IsNullOrEmpty(title))
                Content.Title.Default = title;

            Alignment = alignment;

            if (!string.IsNullOrEmpty(cssClass))
                CssClass = cssClass;
        }

        /// <summary>
        /// Returns the enumeration value that corresponds with the Markdown syntax for horizontal alignment.
        /// </summary>
        private HorizontalAlignment ParseMarkdownAlignment(string markdown)
        {
            if (markdown.StartsWith(":-") && markdown.EndsWith("-:"))
                return HorizontalAlignment.Center;

            if (markdown.EndsWith("-:"))
                return HorizontalAlignment.Right;

            return HorizontalAlignment.Left;
        }

        public bool Equals(OptionColumn other)
        {
            return this.Alignment == other.Alignment
                && this.CssClass == other.CssClass
                && this.Content.IsEqual(other.Content);
        }

        public OptionColumn Clone()
        {
            var clone = new OptionColumn
            {
                Alignment = Alignment,
                CssClass = CssClass,
            };

            clone.Content.Title.Set(Content.Title);

            return clone;
        }
    }
}
