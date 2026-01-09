using System;
using System.Linq;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class SeatComboBox : ComboBox
    {
        public Guid EventIdentifier
        {
            get => ViewState[nameof(EventIdentifier)] as Guid? ?? Guid.Empty;
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var seats = ServiceLocator.EventSearch
                .GetSeats(EventIdentifier, false);

            var data = seats.Select(x => new ListItem
            {
                Value = x.SeatIdentifier.ToString(),
                Text = x.SeatTitle
            });

            return new ListItemArray(data);
        }
    }
}