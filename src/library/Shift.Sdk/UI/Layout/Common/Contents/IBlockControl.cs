using Shift.Common;
namespace Shift.Sdk.UI
{
    public interface IBlockControl
    {
        void BindContent(ContentContainer block, string hook = null);
        string[] GetContentLabels();
    }
}