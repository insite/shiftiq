using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Common;

using Button = InSite.Common.Web.UI.Button;

namespace InSite.Admin.Sites.Pages.Controls.ContentBlocks
{
    public partial class ImageArray : BaseUserControl
    {
        public class DataItem
        {
            public string Alt { get; set; }
            public string Url { get; set; }
        }

        public string UploadPath
        {
            get => (string)ViewState[nameof(UploadPath)];
            set => ViewState[nameof(UploadPath)] = value;
        }

        public bool IsEditable
        {
            get => ViewState[nameof(IsEditable)] is bool value ? value : true;
            set => ViewState[nameof(IsEditable)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CommonStyle.ContentKey = typeof(ImageArray).FullName;

            ImageRepeater.ItemDataBound += ImageRepeater_ItemDataBound;
            ImageRepeater.ItemCommand += ImageRepeater_ItemCommand;

            AddImage.Click += AddImage_Click;
        }

        private void AddImage_Click(object sender, EventArgs e)
        {
            var items = GetDataInternal(false);
            items.Add(new DataItem());
            LoadData(items);
        }

        private void ImageRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (DataItem)e.Item.DataItem;

            var img = (Image)e.Item.FindControl("Image");
            img.UploadPath = UploadPath;
            img.ImageAlt = dataItem.Alt;
            img.ImageUrl = dataItem.Url;
            img.IsBrowsable = IsEditable;

            if (IsEditable)
                return;

            var button = (Button)e.Item.FindControl("Remove");
            button.Visible = IsEditable;

        }

        private void ImageRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var data = GetDataInternal(false);
                data.RemoveAt(e.Item.ItemIndex);
                LoadData(data);
            }
        }

        public void LoadData(IEnumerable<DataItem> data, bool isEditable = true)
        {
            IsEditable = isEditable;
            AddImage.Visible = isEditable;

            ImageRepeater.Visible = true;
            ImageRepeater.DataSource = data;
            ImageRepeater.DataBind();
            ImageRepeater.Visible = ImageRepeater.Items.Count > 0;
        }

        public List<DataItem> GetData() => GetDataInternal(true);

        public List<DataItem> GetDataInternal(bool removeEmpty)
        {
            var list = new List<DataItem>();

            foreach (RepeaterItem rItem in ImageRepeater.Items)
            {
                var img = (Image)rItem.FindControl("Image");
                if (removeEmpty && img.ImageUrl.HasNoValue())
                    continue;

                list.Add(new DataItem
                {
                    Alt = img.ImageAlt,
                    Url = img.ImageUrl
                });
            }

            return list;
        }
    }
}