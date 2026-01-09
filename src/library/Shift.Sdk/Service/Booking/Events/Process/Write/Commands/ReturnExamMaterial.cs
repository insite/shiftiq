using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Events.Write
{
    public class ReturnExamMaterial : Command
    {
        public string ShipmentCode { get; set; }
        public string ShipmentCondition { get; set; }
        public DateTimeOffset? ShipmentReceived { get; set; }

        public ReturnExamMaterial(Guid id, string code, DateTimeOffset? date, string condition)
        {
            AggregateIdentifier = id;
            ShipmentCode = code;
            ShipmentReceived = date;
            ShipmentCondition = condition;
        }
    }
}