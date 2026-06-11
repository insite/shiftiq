using System;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Encapsulates the extended properties that apply only to image attachments.
    /// </summary>
    [Serializable]
    public class AttachmentImage
    {
        #region Properties

        /// <summary>
        /// Is the image color or black and white?
        /// </summary>
        public bool IsColor { get; set; }

        /// <summary>
        /// Dots Per Inch (DPI)
        /// </summary>
        public int Resolution { get; set; }

        /// <summary>
        /// What are the actual dimensions of the image file? These should be calculated and stored when the image is
        /// uploaded (and not determined on-the-fly).
        /// </summary>
        public ImageDimension Actual { get; set; }

        /// <summary>
        /// What are the desired dimensions when this image is displayed on screen?
        /// </summary>
        public ImageDimension TargetOnline { get; set; }

        /// <summary>
        /// What are the desired dimensions when this image is printed on paper?
        /// </summary>
        public ImageDimension TargetPaper { get; set; }

        #endregion

        #region Construction

        public AttachmentImage Clone()
        {
            var clone = new AttachmentImage();

            this.ShallowCopyTo(clone);

            clone.Actual = Actual?.Clone();
            clone.TargetOnline = TargetOnline?.Clone();
            clone.TargetPaper = TargetPaper?.Clone();

            return clone;
        }

        #endregion

        #region Methods

        public bool Equals(AttachmentImage image)
        {
            if (image == null)
                return false;

            if (IsColor != image.IsColor || Resolution != image.Resolution)
                return false;

            return IsEqualImageDimension(Actual, image.Actual)
                && IsEqualImageDimension(TargetOnline, image.TargetOnline)
                && IsEqualImageDimension(TargetPaper, image.TargetPaper);
        }

        private bool IsEqualImageDimension(ImageDimension a, ImageDimension b)
        {
            if (a == null && b == null)
                return true;

            if ((a == null && b != null) || (a != null && b == null))
                return false;

            return a.Equals(b);
        }

        #endregion
    }
}
