using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Events
{
    public class ExamMaterialReturned : Change
    {
        public string ReturnShipmentCode { get; set; }
        public string ReturnShipmentCondition { get; set; }
        public DateTimeOffset? ReturnShipmentDate { get; set; }

        public ExamMaterialReturned(string code, DateTimeOffset? date, string condition)
        {
            ReturnShipmentCode = code;
            ReturnShipmentDate = date;
            ReturnShipmentCondition = condition;
        }
    }
}
