using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;


namespace InSite.Admin.Events.Seats.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<QSeatFilter>
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var seat = (QSeat)e.Row.DataItem;
            var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

            var freePriceLiteral = e.Row.FindControl("FreePrice");
            freePriceLiteral.Visible = false;

            if (configuration.Prices == null)
            {
                freePriceLiteral.Visible = true;
            }
            else if (configuration.Prices.Count == 1 && !configuration.Prices[0].Name.HasValue())
            {
                var singlePriceLiteral = (ITextControl)e.Row.FindControl("SinglePrice");
                singlePriceLiteral.Text = $"{configuration.Prices[0].Amount:c2}";
            }
            else
            {
                var multiplePriceRepeater = (Repeater)e.Row.FindControl("MultiplePrice");

                multiplePriceRepeater.DataSource = configuration.Prices;
                multiplePriceRepeater.DataBind();
            }
        }

        protected override int SelectCount(QSeatFilter filter)
        {
            return ServiceLocator.EventSearch.CountSeats(filter);
        }

        protected override IListSource SelectData(QSeatFilter filter)
        {
            return ServiceLocator.EventSearch
                .GetSeats(filter)
                .ToSearchResult();
        }

        protected static string GetLocalTime(object item)
        {
            var when = (DateTimeOffset?)item;
            return when.Format(null, true, nullValue: string.Empty);
        }

        protected static string GetDescription(object item)
        {
            var seat = (QSeat)item;
            return ContentSeat.Deserialize(seat.Content).Description.Default;
        }
    }
}