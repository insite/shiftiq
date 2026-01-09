using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Platform API: Formats")]
public class FormatController : ControllerBase
{
    public FormatController()
    {

    }

    [HttpGet("platform/formats/time")]
    public IActionResult Time(string value)
    {
        try
        {
            var parsed = DateTimeOffset.Parse(value);

            var result = DateTimeInspector.Inspect(value, parsed);

            return Ok(result);
        }
        catch (Exception ex)
        {
            var error = new Error
            {
                Summary = "Value failed parsing as DateTimeOffset.",
                Description = ex.Message
            };
            return BadRequest(error);
        }
    }
}
