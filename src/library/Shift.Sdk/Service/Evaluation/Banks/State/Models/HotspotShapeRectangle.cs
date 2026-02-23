using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable]
    public sealed class HotspotShapeRectangle : HotspotShape
    {
        public const string ShapeID = "RECTANGLE";

        [JsonIgnore]
        public int X
        {
            get => _x;
            set => _x = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(X));
        }

        [JsonIgnore]
        public int Y
        {
            get => _y;
            set => _y = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(Y));
        }

        [JsonIgnore]
        public int Width
        {
            get => _width;
            set => _width = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Width));
        }

        [JsonIgnore]
        public int Height
        {
            get => _height;
            set => _height = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Height));
        }

        [JsonProperty(PropertyName = nameof(X))]
        private int _x;

        [JsonProperty(PropertyName = nameof(Y))]
        public int _y;

        [JsonProperty(PropertyName = nameof(Width))]
        private int _width;

        [JsonProperty(PropertyName = nameof(Height))]
        public int _height;

        [JsonConstructor]
        private HotspotShapeRectangle()
        {

        }

        public HotspotShapeRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public HotspotShapeRectangle(string value)
        {
            Read(value);
        }

        public override void Read(string value)
        {
            if (value.IsEmpty())
                throw new ArgumentNullException(nameof(value));

            var items = value.Split(' ');
            if (items.Length != 5)
                throw ApplicationError.Create("Wrong string format: " + value);

            if (items[0] != ShapeID)
                throw ApplicationError.Create("Wrong shape identifier: " + value);

            X = int.Parse(items[1]);
            Y = int.Parse(items[2]);
            Width = int.Parse(items[3]);
            Height = int.Parse(items[4]);
        }

        public override bool IsPointInside(int x, int y)
        {
            return x >= X && x <= X + Width
                && y >= Y && y <= Y + Height;
        }

        public override string ToString()
        {
            return $"{ShapeID} {X} {Y} {Width} {Height}";
        }

        public override HotspotShape Clone()
        {
            var clone = new HotspotShapeRectangle();

            CopyTo(clone);

            return clone;
        }

        public override void Resize(HotspotImage.ResizeEventArgs args)
        {
            X = (int)(X * args.WidthFactor);
            Y = (int)(Y * args.HeightFactor);
            Width = (int)(Width * args.WidthFactor);
            Height = (int)(Height * args.HeightFactor);
        }

        public override bool IsEqual(HotspotShape other)
        {
            return other is HotspotShapeRectangle rect
                && this._x == rect._x
                && this._y == rect._y
                && this._width == rect._width
                && this._height == rect._height;
        }

        public override void CopyTo(HotspotShape other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other is HotspotShapeRectangle rect)
            {
                rect._x = this._x;
                rect._y = this._y;
                rect._width = this._width;
                rect._height = this._height;
            }
            else
                throw ApplicationError.Create("Unexpected shape type: " + other.GetType());
        }
    }
}
