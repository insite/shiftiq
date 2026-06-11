namespace Shift.Contract
{
    public partial class FormWorkshop
    {
        public class Section
        {
            public WorkshopStandard[] Standards { get; set; }
            public WorkshopQuestion[] Questions { get; set; }
        }
    }
}
