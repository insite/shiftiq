using Microsoft.AspNetCore.Mvc;

namespace Shift.Api;

[ApiController]
[ApiExplorerSettings(GroupName = "Metadata API: Formats")]
public class FormatController : ControllerBase
{
    public FormatController()
    {

    }

    [HttpGet("api/metadata/formats/time")]
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
            var error = new Problem
            {
                Detail = "Value failed parsing as DateTimeOffset. " + ex.Message
            };
            return BadRequest(error);
        }
    }
}
