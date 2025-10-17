namespace Engine.Api.Internal
{
    public class ClientLockService : IClientLockService
    {
        private class TryItem
        {
            public DateTime? Locked { get; set; }
            public int TryCount { get; set; }
        }

        private const int MaxTryCount = 3;
        private const int LockTimeInMinutes = 15;

        private static Dictionary<string, TryItem> _ipTries = new Dictionary<string, TryItem>(StringComparer.OrdinalIgnoreCase);
        private static object _syncRoot = new();

        public bool IsLocked(string ipAddress)
        {
            lock (_syncRoot)
            {
                return IsLocked(_ipTries, ipAddress);
            }
        }

        public ClientLockStatus Success(string ipAddress)
        {
            lock (_syncRoot)
            {
                if (IsLocked(_ipTries, ipAddress))
                    return ClientLockStatus.Locked;

                _ipTries.Remove(ipAddress);
            }

            return ClientLockStatus.NotLocked;
        }

        public ClientLockStatus Fail(string ipAddress)
        {
            lock (_syncRoot)
            {
                if (IsLocked(_ipTries, ipAddress))
                    return ClientLockStatus.Locked;

                return IncTry(_ipTries, ipAddress);
            }
        }

        private static ClientLockStatus IncTry(Dictionary<string, TryItem> tries, string? key)
        {
            if (string.IsNullOrEmpty(key))
                return ClientLockStatus.NotLocked;

            if (!tries.TryGetValue(key, out var last))
                tries.Add(key, last = new TryItem());

            last.TryCount++;

            if (last.TryCount >= MaxTryCount)
                last.Locked = DateTime.UtcNow;

            return last.Locked.HasValue ? ClientLockStatus.Locked : ClientLockStatus.NotLocked;
        }

        private static bool IsLocked(Dictionary<string, TryItem> tries, string? key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (!tries.TryGetValue(key, out var last))
                return false;

            if (last.Locked == null)
                return false;

            if (last.Locked.Value.AddMinutes(LockTimeInMinutes) >= DateTime.UtcNow)
                return true;

            tries.Remove(key);

            return false;
        }

    }
}
