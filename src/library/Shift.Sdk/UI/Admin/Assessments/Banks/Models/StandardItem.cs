using Shift.Common;
namespace Shift.Sdk.UI
{
    public class StandardItem
    {
        public System.Guid Identifier { get; set; }
        public string Code { get; set; }
        public string Content { get; set; }

        public string Title => Code + ". " + ContentTitle.Deserialize(Content).Title.Default;
    }
}