using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.Partitions
{
    [ApiController]
    [Route("partitions")]
    // Temporary alias. PartitionClient appended its own api/ segment until 2026-08-18; every
    // partition deployed before then still registers at this spelling. Remove once no deployed
    // partition runs the old client.
    [Route("api/partitions")]
    [ApiExplorerSettings(GroupName = "Partitions")]
    public class PartitionController(PartitionService service, IMonitor monitor) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> RegisterAsync([FromBody] PartitionRegistration partition)
        {
            try
            {
                await service.RegisterAsync(partition);

                return Ok();
            }
            catch (Exception ex)
            {
                // The full exception goes to the log, not over the wire. This response used to carry
                // ex.ToString(), which handed every caller the SQL stack trace, the source file
                // paths, and the client connection id.

                monitor.Error(ex);

                return Problem(
                    detail: $"The Hub could not complete the registration for partition {partition.Number}. The correlated error is in the Hub log.",
                    title: "Partition registration failed.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [ProducesResponseType<PartitionRegistration[]>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<PartitionRegistration>>> GetAsync()
        {
            return await service.GetAllAsync();
        }
    }
}
