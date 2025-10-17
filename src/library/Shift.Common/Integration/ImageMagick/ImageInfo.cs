namespace Shift.Common.Integration.ImageMagick
{
    public class ImageInfo
    {
        public uint Height { get; set; }
        public uint Width { get; set; }
        public ImageType ImageType { get; set; }
        public ColorSpace ColorSpace { get; set; }
        public ImageFormat Format { get; set; }
        public double PixelsPerInch { get; set; }
    }
}
