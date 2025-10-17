using System.Drawing;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public sealed class CertificateElementLayout
    {
        #region Properties

        [JsonProperty(PropertyName = "x")]
        public decimal X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public decimal Y { get; set; }

        [JsonProperty(PropertyName = "width")]
        public decimal Width { get; set; }

        [JsonProperty(PropertyName = "height")]
        public decimal Height { get; set; }

        #endregion

        #region Methods

        public RectangleF CreateRectangleF(Size imgSize) =>
            new RectangleF((float)(X * imgSize.Width), (float)(Y * imgSize.Height), (float)(Width * imgSize.Width), (float)(Height * imgSize.Height));

        public Rectangle CreateRectangle(Size imgSize) =>
            new Rectangle((int)(X * imgSize.Width), (int)(Y * imgSize.Height), (int)(Width * imgSize.Width), (int)(Height * imgSize.Height));

        #endregion
    }
}