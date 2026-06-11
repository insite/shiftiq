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
        public async Task<IActionResult> RegisterAsync([FromBody] PartitionRegistration partition)
        {
            await _store.EnsureSchemaAsync();

            await _store.UpsertAsync(partition);

            return Ok();
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
