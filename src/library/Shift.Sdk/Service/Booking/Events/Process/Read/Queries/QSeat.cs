using System;
using System.Collections.Generic;

using InSite.Application.Registrations.Read;

namespace InSite.Application.Events.Read
{
    public class QSeat
    {
        public Guid SeatIdentifier { get; set; }
        public Guid EventIdentifier { get; set; }
        public string Configuration { get; set; }
        public string Content { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsTaxable { get; set; }
        public int? OrderSequence { get; set; }
        public string SeatTitle { get; set; }

        public QEvent Event { get; set; }

        public virtual ICollection<QRegistration> Registrations { get; set; } = new HashSet<QRegistration>();
    }
}
