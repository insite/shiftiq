using System.Collections.Generic;
using System.Text;

namespace Shift.Sdk.UI
{
    public class ScaleDto
    {
        public string Category { get; set; }
        
        public RangeDto ItemA { get; set; }
        public RangeDto ItemB { get; set; }
        public RangeDto ItemC { get; set; }
        public RangeDto ItemD { get; set; }
        public RangeDto ItemF { get; set; }

        public List<string> Rows { get; set; }
        public string RowsHtml
        {
            get
            {
                var html = new StringBuilder();
                html.Append("<ul>");
                foreach (var row in Rows)
                    html.Append($"<li>{row}</li>");
                html.Append("</ul>");
                return html.ToString();
            }
        }
        
        public ScaleDto() { Rows = new List<string>(); }
    }
}