using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Common.Contents;

using Shift.Common;
using Shift.Constant;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentBlocksEditor : ContentBlocksEditorBase
    {
        #region Enums

        private enum FieldInputControlType
        {
            Text, Html, Image, ImageArray
        }

        #endregion

        #region Classes

        private class FieldInfo
        {
            public Guid ID { get; set; }
            public string Name { get; set; }
            public Shift.Common.ContentContainer Content { get; set; }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ContentFieldRepeater.ItemCreated += ContentFieldRepeater_ItemCreated;
            ContentFieldRepeater.ItemDataBound += ContentFieldRepeater_ItemDataBound;
        }

        protected override void OnPreRender(EventArgs e)
        {
            ControlRequiredValidator.ValidationGroup = ValidationGroup;

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private FieldInputControlType GetFieldInputControlType(string name)
        {
            if (name == "Body" || name == "Column 1" || name == "Column 2" || name == "Description" || name == "Paragraphs")
                return FieldInputControlType.Html;
            else if (name == "Image URL")
                return FieldInputControlType.Image;
            else if (name == "Image List")
                return FieldInputControlType.ImageArray;
            else
                return FieldInputControlType.Text;
        }

        private void DeleteButton_Click(object sender, EventArgs e) => OnDelete();

        private void ContentFieldRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var container = (DynamicControl)e.Item.FindControl("Container");
            container.ControlAdded += ContentFieldContainer_ControlAdded;
        }

        private void ContentFieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var data = (FieldInfo)e.Item.DataItem;
            var container = (DynamicControl)e.Item.FindControl("Container");
            var type = GetFieldInputControlType(data.Name);

            if (type == FieldInputControlType.Html)
            {
                var content = data.Content[data.Name];
                var editor = container.LoadControl<MarkdownHtmlEditor>();

                editor.UploadPath = "/web/" + data.ID;
                editor.Html = content.Html;
                editor.Text = content.Text;

                if (content.Text.IsEmpty)
                    editor.DefaultMode = MarkdownHtmlMode.Html;
            }
            else if (type == FieldInputControlType.Image)
            {
                var img = (ContentBlocks.Image)container.LoadControl("~/UI/Admin/Sites/Pages/Controls/ContentBlocks/Image.ascx");
                img.UploadPath = "/web/" + data.ID;
                img.ImageAlt = data.Content[data.Name + ":Alt"].Text.Default;
                img.ImageUrl = data.Content[data.Name + ":Url"].Text.Default;
            }
            else if (type == FieldInputControlType.ImageArray)
            {
                var imgArray = (ContentBlocks.ImageArray)container.LoadControl("~/UI/Admin/Sites/Pages/Controls/ContentBlocks/ImageArray.ascx");
                imgArray.UploadPath = "/web/" + data.ID;

                var imgData = new List<ContentBlocks.ImageArray.DataItem>();

                while (true)
                {
                    var key = $"{data.Name}:{imgData.Count}";
                    var alt = (ContentContainerItem)data.Content[key + "." + nameof(ContentBlocks.ImageArray.DataItem.Alt)];
                    var url = (ContentContainerItem)data.Content[key + "." + nameof(ContentBlocks.ImageArray.DataItem.Url)];

                    var hasAlt = alt != null && !alt.IsEmpty;
                    var hasUrl = url != null && !url.IsEmpty;

                    if (!hasAlt && !hasUrl)
                        break;

                    imgData.Add(new ContentBlocks.ImageArray.DataItem
                    {
                        Alt = hasAlt ? alt.Text.Default : null,
                        Url = hasUrl ? url.Text.Default : null
                    });
                }

                imgArray.LoadData(imgData);
            }
            else
            {
                var input = container.LoadControl<Common.Web.UI.TextBox>();
                input.Text = data.Content[data.Name].Text.Default;
            }
        }

        private void ContentFieldContainer_ControlAdded(object sender, EventArgs e)
        {
            var container = (DynamicControl)sender;
            var control = container.GetControl();

            if (control is ContentBlocks.Image fwti)
                fwti.Alert += Image_Alert;
        }

        private void Image_Alert(object sender, AlertArgs args)
        {
            OnAlert(args);
        }

        #endregion

        #region Data binding

        public void SetInputValues(QPage page, Shift.Common.ContentContainer content)
        {
            BindControlSelector(ControlSelector);

            ControlSelector.Value = page.ContentControl;
            Title.Text = page.Title;
            Hook.Text = page.Hook;

            if (content == null)
                content = new Shift.Common.ContentContainer();

            var block = ControlPath.GetPageBlock(page.ContentControl);
            var hasLabels = block != null && block.Labels.Length > 0;

            ContentRow.Visible = hasLabels;

            if (hasLabels)
            {
                ContentFieldRepeater.DataSource = block.Labels.Select(x => new FieldInfo
                {
                    ID = page.PageIdentifier,
                    Name = x,
                    Content = content
                });
                ContentFieldRepeater.DataBind();
            }

            DeleteButton.NavigateUrl = $"/ui/admin/sites/pages/delete?id={page.PageIdentifier}";
        }

        public void GetInputValues(QPage page, Shift.Common.ContentContainer content)
        {
            if (page != null)
            {
                page.ContentControl = ControlSelector.Value;
                page.Title = StringHelper.FirstValue(Title.Text, ControlSelector.GetSelectedOption().Text);
                page.PageSlug = StringHelper.Sanitize(page.Title, '-', true, new[] { '_' });
                page.Hook = Hook.Text;
            }

            if (content != null)
            {
                foreach (RepeaterItem item in ContentFieldRepeater.Items)
                {
                    var fieldName = (ITextControl)item.FindControl("FieldName");
                    var container = (DynamicControl)item.FindControl("Container");

                    var name = fieldName.Text;
                    var type = GetFieldInputControlType(name);

                    if (type == FieldInputControlType.Html)
                    {
                        var html = (MarkdownHtmlEditor)container.GetControl();
                        content[name].Text = html.Text;
                        content[name].Html = html.Html;
                    }
                    else if (type == FieldInputControlType.Image)
                    {
                        var img = (ContentBlocks.Image)container.GetControl();
                        content[name + ":Alt"].Text.Default = img.ImageAlt;
                        content[name + ":Url"].Text.Default = img.ImageUrl;
                    }
                    else if (type == FieldInputControlType.ImageArray)
                    {
                        var imgArr = (ContentBlocks.ImageArray)container.GetControl();
                        var imgData = imgArr.GetData();

                        {
                            var removePattern = name + ":";
                            foreach (var key in content.GetLabels().ToArray())
                                if (key.StartsWith(removePattern))
                                    content.Remove(key);
                        }

                        for (var i = 0; i < imgData.Count; i++)
                        {
                            var dataItem = imgData[i];
                            var key = $"{name}:{i}";
                            content[key + "." + nameof(ContentBlocks.ImageArray.DataItem.Alt)].Text.Default = dataItem.Alt;
                            content[key + "." + nameof(ContentBlocks.ImageArray.DataItem.Url)].Text.Default = dataItem.Url;
                        }
                    }
                    else
                    {
                        var textBox = (ITextBox)container.GetControl();
                        content[name].Text.Default = textBox.Text;
                    }
                }
            }
        }

        #endregion
    }
}