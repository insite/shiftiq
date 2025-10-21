using System;
using System.Web.UI;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class BarChart : Control
    {
        #region Properties

        public String Color
        {
            get { return (string)ViewState[nameof(Color)] ?? "#333333"; }
            set { ViewState[nameof(Color)] = value; }
        }

        public Int32 Width
        {
            get { return (Int32)(ViewState[nameof(Width)] ?? 100); }
            set { ViewState[nameof(Width)] = value; }
        }

        public String Text
        {
            get { return (String)ViewState[nameof(Text)]; }
            set { ViewState[nameof(Text)] = value; }
        }

        #endregion

        #region Overriden methods

        protected override void Render(HtmlTextWriter writer)
        {
            var space = Width > 0 ? "&nbsp;" : "";
            var tag = string.Format("<div class='barchart' style='background-color:{0}; width: {1}px'>{3}</div><span class='barchart'>{2}</span>", Color, Width, Text, space);

            writer.Write(tag);
        }

        #endregion
    }
}
