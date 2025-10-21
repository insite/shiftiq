using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Common.Contents;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentBlocksViewer : ContentBlocksEditorBase
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
                var input = container.LoadControl<System.Web.UI.WebControls.Literal>();
                input.Text = data.Content[data.Name].Html.Default;
            }
            else
            if (type == FieldInputControlType.Image)
            {
                var img = (ContentBlocks.Image)container.LoadControl("~/UI/Admin/Sites/Pages/Controls/ContentBlocks/Image.ascx");
                img.UploadPath = "/web/" + data.ID;
                img.ImageAlt = data.Content[data.Name + ":Alt"].Text.Default;
                img.ImageUrl = data.Content[data.Name + ":Url"].Text.Default;
            }
            else if (type == FieldInputControlType.ImageArray)
            {
                var imgArray = (ContentBlocks.ImageArray)container.LoadControl("~/UI/Admin/Sites/Pages/Controls/ContentBlocks/ImageArray.ascx");
                imgArray.ID = "ImageArray";
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

                imgArray.LoadData(imgData, false);
            }
            else
            {
                var input = container.LoadControl<System.Web.UI.WebControls.Literal>();
                input.Text = data.Content[data.Name].Text.Default;
            }
        }

        private void ContentFieldContainer_ControlAdded(object sender, EventArgs e)
        {
            var container = (DynamicControl)sender;
            var control = container.GetControl();

            if (control is ContentBlocks.Image fwti)
                fwti.Alert += Image_StatusUpdated;
        }

        private void Image_StatusUpdated(object sender, AlertArgs args)
        {
            OnAlert(args);
        }

        #endregion

        #region Data binding

        public void SetInputValues(QPage page, Shift.Common.ContentContainer content, string editUrl)
        {
            var selectorVal = ControlPath.BlockControlTypes.FirstOrDefault(x => x.Name == page.ContentControl);

            ControlSelector.Text = (selectorVal != null ? selectorVal.Title : "");
            Title.Text = page.Title;
            Hook.Text = page.Hook;
            EditLink.NavigateUrl = editUrl;


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
        }
        #endregion
    }
}