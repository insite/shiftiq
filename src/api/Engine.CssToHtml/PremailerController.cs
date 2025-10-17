using Microsoft.AspNetCore.Mvc;

using Shift.Common;

namespace Engine.CssToHtml;

[ApiController]
public class PreMailerController(IMonitor monitor) : ControllerBase
{
    private readonly IMonitor _monitor = monitor;

    [HttpPost]
    [Route(Endpoints.MoveCssInline)]
    [ProducesResponseType<string>(StatusCodes.Status200OK)]
    [EndpointName("moveCssInline")]
    public async Task<ActionResult<string>> MoveCssInlineAsync()
    {
        try
        {
            using (var reader = new StreamReader(Request.Body))
            {
                string html = await reader.ReadToEndAsync();

                var inline = PreMailer.Net.PreMailer.MoveCssInline(html, true, null, null, false, false).Html;

                return Ok(inline);
            }
        }
        catch (Exception ex)
        {
            _monitor.Error(ex.Message);

            return StatusCode(500, ex.Message);
        }
    }
}