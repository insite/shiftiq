namespace InSite.Persistence.Plugin.CMDS
{
    public class UserStatusHome
    {
        public int Critical { get; set; }
        public int CriticalValidated { get; set; }
        public int CriticalSubmitted { get; set; }
        public int NonCritical { get; set; }
        public int NonCriticalValidated { get; set; }
        public int NonCriticalSubmitted { get; set; }
    }
}
