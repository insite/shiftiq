namespace Shift.Common
{
    public class RabbitMq
    {
        public bool Disabled { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string[] Environments { get; set; }
    }
}
