namespace Shift.Contract.Responses
{
    public class ComplianceSummaryResponse
    {
        public ObjectResponse Organization { get; set; }

        public ObjectResponse Department { get; set; }

        public ObjectResponse Learner { get; set; }

        public ObjectResponse PrimaryProfile { get; set; }

        public ComplianceSummaryMeasurementResponse Measurement { get; set; }
    }
}