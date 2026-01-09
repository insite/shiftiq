using System.Collections.Generic;

using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class ImageGallery : BlockControl, IBlockControl
    {
        private class DataItem
        {
            public string Alt { get; set; }
            public string Url { get; set; }
        }

        private static readonly string[] _contentLabels = new[] { "Image List" };

        public override string[] GetContentLabels() => _contentLabels;

        public override void BindContent(ContentContainer content, string hook = null)
        {
            var data = new List<DataItem>();

            while (true)
            {
                var key = $"Image List:{data.Count}";
                var alt = (ContentContainerItem)content[key + ".Alt"];
                var url = (ContentContainerItem)content[key + ".Url"];

                var hasAlt = alt != null && !alt.IsEmpty;
                var hasUrl = url != null && !url.IsEmpty;

                if (!hasAlt && !hasUrl)
                    break;

                data.Add(new DataItem
                {
                    Alt = hasAlt ? alt.Text.Default : null,
                    Url = hasUrl ? url.Text.Default : null
                });
            }

            ImageRepeater.DataSource = data;
            ImageRepeater.DataBind();
        }

        private static readonly BlockContentControlTypeInfo _blockInfo =
            new BlockContentControlTypeInfo(2, typeof(ImageGallery), "Image Gallery", "~/UI/Layout/Common/Controls/ImageGallery.ascx", _contentLabels);
        public static BlockContentControlTypeInfo GetBlockInfo() => _blockInfo;
    }
}