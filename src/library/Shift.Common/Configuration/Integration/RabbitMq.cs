namespace Shift.Common
{
    public class RabbitMq
    {
        public RabbitMqMode Mode { get; set; } = RabbitMqMode.Off;
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Environments { get; set; }
        public int[] Partitions { get; set; }
        public string[] IncludeQueue { get; set; }
        public string[] ExcludeQueue { get; set; }
    }
}
