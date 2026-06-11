using Shift.Common.Integration.ImageMagick;

namespace Shift.Hub.NetVips
{
    public class AdjustImageModel
    {
        public IFormFile? Image { get; set; }
        public AdjustImageSettings? Settings { get; set; }
    }
}
