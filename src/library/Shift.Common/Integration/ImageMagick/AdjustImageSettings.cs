namespace Shift.Common.Integration.ImageMagick
{
    public class AdjustImageSettings
    {
        public ImageType OutputType { get; set; }
        public bool Crop { get; set; }
        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
    }
}
