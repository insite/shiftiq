using System;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// The two dimensions of an image are height and width.
    /// </summary>
    [Serializable]
    public class ImageDimension
    {
        #region Properties

        public int Height { get; set; }

        public int Width { get; set; }

        public bool HasValue => Width > 0 && Height > 0;

        #endregion

        #region Methods

        public ImageDimension Clone()
        {
            var clone = new ImageDimension();

            this.ShallowCopyTo(clone);

            return clone;
        }

        public bool Equals(ImageDimension dimension)
        {
            return Height == dimension.Height
                && Width == dimension.Width;
        }

        #endregion
    }
}
