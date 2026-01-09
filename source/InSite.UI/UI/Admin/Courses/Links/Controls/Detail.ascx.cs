using System.Web.UI;

 using InSite.Persistence;

namespace InSite.Admin.Courses.Links.Controls
{
    public partial class Detail : UserControl
    {
        public void GetInputValues(TLtiLink entity)
        {
            entity.ResourceTitle = Title.Text;
            entity.ResourceSummary = Description.Text;
            entity.ResourceParameters = CustomParameters.Text;
            entity.ToolProviderUrl = Url.Text;
            entity.ResourceCode = Code.Text;
            entity.ToolProviderName = Publisher.Text;
            entity.ToolProviderType = Subtype.Value;
            entity.ResourceName = Location.Text;
            entity.ToolConsumerKey = Key.Text;
            entity.ToolConsumerSecret = Secret.Text;
        }

        public void SetInputValues(TLtiLink entity)
        {
            Title.Text = entity.ResourceTitle;
            Description.Text = entity.ResourceSummary;
            CustomParameters.Text = entity.ResourceParameters;
            Url.Text = entity.ToolProviderUrl;
            Code.Text = entity.ResourceCode;
            Publisher.Text = entity.ToolProviderName;
            Subtype.Value = entity.ToolProviderType;
            Location.Text = entity.ResourceName;
            Key.Text = entity.ToolConsumerKey;
            Secret.Text = entity.ToolConsumerSecret;
        }
    }
}