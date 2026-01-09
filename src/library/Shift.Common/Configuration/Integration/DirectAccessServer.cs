namespace Shift.Common
{
    public class DirectAccessServer
    {
        public string ApiUrl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsValid()
            => ApiUrl.IsNotEmpty() && UserName.IsNotEmpty() && Password.IsNotEmpty();
    }
}
