namespace Shift.Toolbox.Integration.DirectAccess
{
    public class DirectAccessApiResponse
    {
        public bool IsOK => Status == 200;

        public int Status { get; set; }
        
        public string Content { get; set; }
        
        public DirectAccessApiError Error { get; set; }
    }
}