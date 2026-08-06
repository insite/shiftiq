using System.ComponentModel;

namespace InSite.Application.Issues.Read
{
    public enum DocumentFilterType
    {
        [Description("All")]
        All,

        [Description("Only with requested documents")]
        RequestedOnly,

        [Description("Only with uploaded documents")]
        UploadedOnly,
    }
}
