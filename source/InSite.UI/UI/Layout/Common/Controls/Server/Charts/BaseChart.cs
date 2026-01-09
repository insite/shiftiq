using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Common.Web.UI.Chart
{
    public abstract class BaseChart : Control
    {
        #region Properties

        public abstract ChartType ChartType { get; }

        public virtual string CssClass
        {
            get => (string)ViewState[nameof(CssClass)];
            set => ViewState[nameof(CssClass)] = value;
        }

        public virtual Unit Width
        {
            get => (Unit)(ViewState[nameof(Width)] ?? Unit.Empty);
            set => ViewState[nameof(Width)] = value;
        }

        public virtual Unit Height
        {
            get => (Unit)(ViewState[nameof(Height)] ?? Unit.Empty);
            set => ViewState[nameof(Height)] = value;
        }

        protected ChartConfiguration Configuration => 
            (_config ?? (_config = (ChartConfiguration)(ViewState[nameof(Configuration)] ?? CreateChartConfiguration())));

        public IChartData Data => Configuration.Data;

        #endregion

        #region Fields

        private ChartConfiguration _config;

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && _config != null && ViewState[nameof(Configuration)] != null)
                _config = (ChartConfiguration)ViewState[nameof(Configuration)];
        }

        protected abstract ChartConfiguration CreateChartConfiguration();

        #endregion

        #region PreRender

        protected override void OnPreRender(EventArgs e)
        {
            ViewState[nameof(Configuration)] = _config;

            var script = new StringBuilder();

            BuildInitializationScript(script);

            if (script.Length > 0)
            {
                ScriptManager.RegisterStartupScript(
                    Page,
                    GetType(),
                    "init_" + ClientID,
                    script.ToString(),
                    true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Render

        public abstract void BuildInitializationScript(StringBuilder builder);

        protected override void Render(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(CssClass))
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);

            var style = GetStyle();
            if (style != null)
                writer.AddAttribute(HtmlTextWriterAttribute.Style, style);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.RenderBeginTag("canvas");
            writer.RenderEndTag();
        }

        protected string GetStyle()
        {
            var sb = new StringBuilder();

            if (!Width.IsEmpty)
                sb.Append("width:").Append(Width.ToString()).Append(";");

            if (!Height.IsEmpty)
                sb.Append("height:").Append(Height.ToString()).Append(";");

            return sb.Length == 0 ? null : sb.ToString();
        }

        #endregion
    }
}
