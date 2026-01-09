using InSite.Application.Records.Read;
using InSite.Persistence;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class ProgramDetails : System.Web.UI.UserControl
    {
        public void BindProgram(TProgram program)
        {
            ProgramLink.HRef = Outline.GetNavigateUrl(program.ProgramIdentifier);
            ProgramCode.Text = program.ProgramCode;
            ProgramName.Text = program.ProgramName;
        }
    }
}