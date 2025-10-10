namespace Shift.Contract
{
    public class Access
    {
        public bool Execute { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Create { get; set; }
        public bool Delete { get; set; }
        public bool Admin { get; set; }
        public bool Configure { get; set; }
    }
}