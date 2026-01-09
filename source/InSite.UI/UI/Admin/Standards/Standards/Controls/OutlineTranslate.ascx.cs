using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Standards.Write;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class OutlineTranslate : BaseUserControl
    {
        #region Events

        public event EventHandler Updated;

        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid? StandardID
        {
            get => (Guid?)ViewState[nameof(StandardID)];
            set => ViewState[nameof(StandardID)] = value;
        }

        private string[] Fields
        {
            get => (string[])ViewState[nameof(Fields)];
            set => ViewState[nameof(Fields)] = value;
        }

        #endregion

        #region Fields

        private string _defaultLanguage = null;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var options = (AssetContentSection)e.Item.DataItem;
            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(options.ControlPath);

            section.SetValidationGroup("OutlineTranslate");
            section.SetOptions(options);
            section.SetLanguage(_defaultLanguage);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (StandardID.HasValue)
            {
                var data = new Shift.Common.ContentContainer();

                for (var i = 0; i < Fields.Length; i++)
                {
                    var name = Fields[i];
                    var item = data[name];
                    var container = (DynamicControl)FieldRepeater.Items[i].FindControl("Container");
                    var section = (SectionBase)container.GetControl();

                    if (name == "Title")
                    {
                        item.Text = section.GetValue();
                    }
                    else
                    {
                        item.Html = section.GetValue(ContentSectionDefault.BodyHtml.GetName());
                        item.Text = section.GetValue(ContentSectionDefault.BodyText.GetName());
                    }
                }

                ServiceLocator.SendCommand(new ModifyStandardContent(StandardID.Value, data));
            }

            OnUpdated();
        }

        public bool LoadData(int asset)
        {
            Fields = Organization.GetStandardContentLabels();
            if (Fields.Length == 0)
                return false;

            var standard = StandardSearch.Select(Organization.Key, asset);
            if (standard == null)
                return false;

            StandardID = standard.StandardIdentifier;

            var content = ServiceLocator.ContentSearch.GetBlock(standard.StandardIdentifier, labels: Fields);

            Header.InnerHtml = $"{HttpUtility.HtmlEncode(content.Title.GetText())} " +
                $"<small class='text-body-secondary'>{standard.StandardType} Asset #{standard.AssetNumber}</small>";

            _defaultLanguage = standard.Language ?? CurrentSessionState.Identity.Language;

            FieldRepeater.DataSource = Fields.Select(name =>
            {
                var item = content[name];
                AssetContentSection result;

                if (name == "Title")
                {
                    result = new AssetContentSection.SingleLine(name)
                    {
                        Title = name,
                        Value = item?.Text
                    };
                }
                else
                {
                    result = new AssetContentSection.MarkdownAndHtml(name)
                    {
                        Title = name,
                        HtmlValue = item?.Html,
                        MarkdownValue = item?.Text,
                        IsMultiValue = true
                    };
                }

                return result;
            });
            FieldRepeater.DataBind();

            return true;
        }
    }
}