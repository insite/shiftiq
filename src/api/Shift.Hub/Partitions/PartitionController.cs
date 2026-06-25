using Microsoft.AspNetCore.Mvc;

namespace Shift.Hub.Partitions
{
    [ApiController]
    [Route("api/partitions")]
    [ApiExplorerSettings(GroupName = "Partitions")]
    public class PartitionController : ControllerBase
    {
        private readonly PartitionStore _store;

        public PartitionController(PartitionStore store)
        {
            _store = store;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterAsync([FromBody] PartitionRegistration partition)
        {
            try
            {
                await _store.EnsureSchemaAsync();

                await _store.UpsertAsync(partition);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.ToString(),
                    title: "Partition registration failed.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [ProducesResponseType<PartitionRegistration[]>(StatusCodes.Status200OK, "application/json")]
        public async Task<ActionResult<List<PartitionRegistration>>> GetAsync()
        {
            await _store.EnsureSchemaAsync();

            return await _store.GetAllAsync();
        }
    }
}
