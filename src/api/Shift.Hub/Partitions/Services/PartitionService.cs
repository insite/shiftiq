using System.Collections.Concurrent;

namespace Shift.Hub.Partitions
{
    public class PartitionService(PartitionRepository repo)
    {
        private static readonly ConcurrentQueue<PartitionRegistration> _queue = new();
        private static readonly SemaphoreSlim _lock = new(1, 1);

        public async Task RegisterAsync(PartitionRegistration partition)
        {
            _queue.Enqueue(partition);

            await _lock.WaitAsync();

            try
            {
                while (_queue.TryDequeue(out var current))
                {
                    await repo.UpsertAsync(current);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<List<PartitionRegistration>> GetAllAsync()
        {
            return await repo.GetAllAsync();
        }
    }
}