using System;

using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

namespace InSite.UI.Admin.Events.Seats
{
    public partial class Search : SearchPage<QSeatFilter>
    {
        public override string EntityName => "Seat";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this);
        }
    }
}