using Shift.Sdk.UI;

namespace InSite.Api.Models.Records
{
    public class Achievement
    {
        public Template Template { get; set; }
        public Document Document { get; set; }

        public Achievement()
        {
            Template = new Template();
            Document = new Document();
        }
    }
}