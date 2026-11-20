namespace Shift.Contract
{
    public partial class SpecWorkshopPartial
    {
        public SpecWorkshop.SpecDetails Details { get; set; }
        public WorkshopProblemQuestion[] ProblemQuestions { get; set; }
    }
}