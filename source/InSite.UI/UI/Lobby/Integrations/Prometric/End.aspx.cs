using System;
using System.Web.UI;

namespace InSite.UI.Lobby.Integrations.Prometric
{
    /// <summary>
    /// When the END indicator page is reached, the exam is considered complete, and the browser will be closed. The 
    /// END indicator page does not support query string parameters.
    /// 
    /// Example of an END Indicator Page URL
    /// https://example.com/examend.aspx
    ///
    /// </summary>
    public partial class End : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}