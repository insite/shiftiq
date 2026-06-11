using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class HotspotImage
    {
        #region Events

        public class ResizeEventArgs : EventArgs
        {
            public int OldWidth { get; set; }
            public int OldHeight { get; set; }

            public int NewWidth { get; set; }
            public int NewHeight { get; set; }

            public decimal WidthFactor => (decimal)NewWidth / OldWidth;
            public decimal HeightFactor => (decimal)NewHeight / OldHeight;

            public ResizeEventArgs(int oldWidth, int oldHeight, int newWidth, int newHeight)
            {
                OldWidth = oldWidth;
                OldHeight = oldHeight;

                NewWidth = newWidth;
                NewHeight = newHeight;
            }
        }

        public delegate void ResizeEventHandler(object sender, ResizeEventArgs args);

        public event ResizeEventHandler Resized;

        #endregion

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Url { get; private set; }

        public int Width => _width ?? -1;

        public int Height => _height ?? -1;

        public bool IsEmpty => Url.IsEmpty() && !_width.HasValue && !_height.HasValue;

        [JsonProperty(PropertyName = nameof(Width), DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _width;

        [JsonProperty(PropertyName = nameof(Height), DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _height;

        public HotspotImage()
        {

        }

        public HotspotImage(string url, int width, int height)
        {
            Set(url, width, height);
        }

        public void Set(HotspotImage image) =>
            Set(image.Url, image.Width, image.Height);

        public void Set(string url, int width, int height)
        {
            if (url.IsEmpty())
                throw new ArgumentNullException(nameof(url));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            var resizeArgs = new ResizeEventArgs(_width ?? width, _height ?? height, width, height);

            Url = url;
            _width = width;
            _height = height;

            if (resizeArgs.WidthFactor != 1 || resizeArgs.HeightFactor != 1)
                Resized?.Invoke(this, resizeArgs);
        }

        public void CopyTo(HotspotImage other)
        {
            other.Url = this.Url;
            other._width = this._width;
            other._height = this._height;
        }

        public HotspotImage Clone()
        {
            var clone = new HotspotImage();

            CopyTo(clone);

            return clone;
        }

        public bool IsEqual(HotspotImage other)
        {
            return this.Url.EmptyIfNull().Equals(other.Url.EmptyIfNull(), StringComparison.OrdinalIgnoreCase)
                && this._width == other._width
                && this._height == other._height;
        }

        public override string ToString()
        {
            return IsEmpty
                ? null
                : $"{Width}x{Height}|{Url}";
        }

        public static HotspotImage FromString(string value)
        {
            if (value.IsEmpty())
                return new HotspotImage();

            var separator1 = value.IndexOf('x');
            var separator2 = value.IndexOf('|');

            if (separator1 == -1 || separator2 == -1 || separator1 > separator2)
                return new HotspotImage();

            var width = int.Parse(value.Substring(0, separator1));
            var height = int.Parse(value.Substring(separator1 + 1, separator2 - separator1 - 1));
            var url = value.Substring(separator2 + 1);

            var result = new HotspotImage();

            result.Set(url, width, height);

            return result;
        }
    }
}
