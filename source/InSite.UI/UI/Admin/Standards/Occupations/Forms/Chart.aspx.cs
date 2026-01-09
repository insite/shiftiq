using System;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Occupations.Utilities.Chart;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;

namespace InSite.Admin.Standards.Occupations.Forms
{
    public partial class Chart : AdminBasePage
    {
        private Shift.Sdk.UI.Chart Model { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LineRepeater.ItemDataBound += LineRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!LoadData())
                HttpResponseHelper.Redirect("/ui/admin/standards/standards/search", true);
        }

        private bool LoadData()
        {
            if (!Guid.TryParse(Request.QueryString["asset"], out var standardIdentifier))
                return false;

            var standard = StandardSearch.BindFirst(
                x => new
                {
                    x.StandardIdentifier,
                    Title = x.ContentTitle,
                    Summary = x.ContentSummary
                },
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.StandardIdentifier == standardIdentifier);

            if (standard == null)
                return false;

            PageHelper.AutoBindHeader(this, null, standard.Title);

            var model = OccupationAdapter.Load(Organization.OrganizationCode, standardIdentifier);
            Model = model.CreateChart();

            if (Model.Lines.Count > 0)
            {
                LineRepeater.DataSource = Model.Lines.Values;
                LineRepeater.DataBind();
            }
            else
            {
                ScreenStatus.AddMessage(AlertType.Information, "No data found for the chart.");
            }

            return true;
        }

        protected string GetLevelLabel(int level, bool hasLevel, string url)
        {
            var sb = new StringBuilder();

            if (hasLevel)
                sb.AppendFormat("<a href='{1}'><span class='badge bg-primary'>Level {0}</span></a>", level, url);
            else
                sb.AppendFormat("<span class='badge text-white'>Level {0}</span>", level);

            return sb.ToString();
        }

        private void LineRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (IsContentItem(e))
            {
                var line = (Shift.Sdk.UI.ChartLine)e.Item.DataItem;

                var repeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
                repeater.DataSource = line.Competencies.Values;
                repeater.DataBind();
            }
        }
    }
}