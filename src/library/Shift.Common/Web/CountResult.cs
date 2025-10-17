namespace Shift.Common
{
    public class CountResult
    {
        public int Count { get; set; }

        public string Summary { get; }

        public CountResult(int count, string summary = null)
        {
            Count = count;

            Summary = summary;
        }
    }
}