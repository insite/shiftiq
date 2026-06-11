using Microsoft.AspNetCore.Mvc;

using Endpoints = Shift.Common.Integration.Premailer.Endpoints;

namespace Shift.Hub.PreMailer
{
    [ApiController]
    [Route("premailer")]
    [ApiExplorerSettings(GroupName = "Integration: PreMailer")]
    public class PreMailerController(IMonitor monitor) : ControllerBase
    {
        private readonly IMonitor _monitor = monitor;

        [HttpPost]
        [Route(Endpoints.MoveCssInline)]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> MoveCssInlineAsync()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    string html = await reader.ReadToEndAsync();

                    var inline = global::PreMailer.Net.PreMailer.MoveCssInline(html, true, null, null, false, false).Html;

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
}
