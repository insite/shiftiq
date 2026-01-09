namespace InSite.Application.Attempts.Read
{
    public interface IAttemptAnswer
    {
        bool? IsSelected { get; }
        bool? IsTrue { get; }
        decimal Points { get; }
    }
}
