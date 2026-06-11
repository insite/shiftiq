namespace InSite.Domain.Banks
{
    public interface IQuestionAnswer
    {
        decimal Points { get; set; }
        bool? IsTrue { get; }
    }
}
