namespace Shift.Constant
{
    public static class AlertTypeExtensions
    {
        public static string GetCssPostfix(this AlertType indicator)
        {
            switch (indicator)
            {
                case AlertType.Error: 
                    return "danger";
                case AlertType.Information: 
                    return "info";
                case AlertType.Success: 
                    return "success";
                case AlertType.Warning: 
                    return "warning";
                default: 
                    return "light";
            }
        }
    }
}
