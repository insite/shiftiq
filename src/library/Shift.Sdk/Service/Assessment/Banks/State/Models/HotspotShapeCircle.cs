using System;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Banks
{
    [Serializable]
    public sealed class HotspotShapeCircle : HotspotShape
    {
        public const string ShapeID = "CIRCLE";

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
        public int Radius
        {
            get => _radius;
            set => _radius = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(Radius));
        }

        [JsonProperty(PropertyName = nameof(X))]
        private int _x;

        [JsonProperty(PropertyName = nameof(Y))]
        public int _y;

        [JsonProperty(PropertyName = nameof(Radius))]
        public int _radius;

        [JsonConstructor]
        private HotspotShapeCircle()
        {

        }

        public HotspotShapeCircle(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public HotspotShapeCircle(string value)
        {
            Read(value);
        }

        public override void Read(string value)
        {
            if (value.IsEmpty())
                throw new ArgumentNullException(nameof(value));

            var items = value.Split(' ');
            if (items.Length != 4)
                throw ApplicationError.Create("Wrong string format: " + value);

            if (items[0] != ShapeID)
                throw ApplicationError.Create("Wrong shape identifier: " + value);

            X = int.Parse(items[1]);
            Y = int.Parse(items[2]);
            Radius = int.Parse(items[3]);
        }

        public override bool IsPointInside(int x, int y)
        {
            return Math.Pow(x - X, 2) + Math.Pow(y - Y, 2) <= Math.Pow(Radius, 2);
        }

        public override string ToString()
        {
            return $"{ShapeID} {X} {Y} {Radius}";
        }

        public override HotspotShape Clone()
        {
            var clone = new HotspotShapeCircle();

            CopyTo(clone);

            return clone;
        }

        public override void Resize(HotspotImage.ResizeEventArgs args)
        {
            X = (int)(X * args.WidthFactor);
            Y = (int)(Y * args.HeightFactor);

            var rx = (int)(Radius * args.WidthFactor);
            var ry = (int)(Radius * args.HeightFactor);

            Radius = rx < ry ? rx : ry;
        }

        public override bool IsEqual(HotspotShape other)
        {
            return other is HotspotShapeCircle circle
                && this._x == circle._x
                && this._y == circle._y
                && this._radius == circle._radius;
        }

        public override void CopyTo(HotspotShape other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other is HotspotShapeCircle circle)
            {
                circle._x = this._x;
                circle._y = this._y;
                circle._radius = this._radius;
            }
            else
                throw ApplicationError.Create("Unexpected shape type: " + other.GetType());
        }
    }
}
