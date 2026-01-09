using Shift.Common.Integration.ImageMagick;

namespace Engine.ImageMagick;

public class AdjustImageModel
{
    public IFormFile? Image { get; set; }
    public AdjustImageSettings? Settings { get; set; }
}
