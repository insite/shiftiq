using System;
using System.Web.UI;

using Shift.Sdk.UI;

namespace InSite.Common.Web.UI
{
    public class BarChart : Control, IHasText
    {
        #region Properties

        public string Color
        {
            get { return ViewState[nameof(Color)] == null ? "#333333" : (string)ViewState[nameof(Color)]; }
            set { ViewState[nameof(Color)] = value; }
        }

        public int Width
        {
            get { return ViewState[nameof(Width)] == null ? 100 : (int)ViewState[nameof(Width)]; }
            set { ViewState[nameof(Width)] = value; }
        }

        public string Text
        {
            get { return (string)ViewState[nameof(Text)]; }
            set { ViewState[nameof(Text)] = value; }
        }

        #endregion

        #region Overriden methods

        protected override void Render(HtmlTextWriter writer)
        {
            var space = Width > 0 ? "&nbsp;" : "";
            var tag = String.Format("<div class='barchart' style='background-color:{0}; width: {1}px'>{3}</div><span class='barchart'>{2}</span>", Color, Width, Text, space);

            writer.Write(tag);
        }

        #endregion
    }
}