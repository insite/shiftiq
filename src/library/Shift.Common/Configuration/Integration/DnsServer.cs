namespace Shift.Common
{
    public class DnsServer
    {
        public string Host { get; set; }
        public int Priority { get; set; }
        public int UdpRetryCount { get; set; }
        public int UdpTimeout { get; set; }
    }
}
