using Shift.Common;

namespace Shift.Service
{
    /// <summary>
    /// Thread-safe cache for the application's permission matrix
    /// </summary>
    public class PermissionCache
    {
        private readonly PermissionMatrixLoader _loader;
        private readonly PermissionRefreshExchange _refreshExchange;
        private static volatile PermissionMatrix _matrix = null!;
        private static readonly object _lock = new object();

        /// <summary>
        /// Partition identifier used to merge partition-wide permissions with organization-specific permissions
        /// </summary>
        public string Partition { get; private set; } = null!;

        /// <summary>
        /// Subroute configuration for permission inheritance
        /// </summary>
        public List<Subroute> Subroutes { get; private set; } = new List<Subroute>();

        /// <summary>
        /// Gets the permission matrix, loading it on first access
        /// </summary>
        public PermissionMatrix Matrix
        {
            get
            {
                if (_matrix == null) // avoid taking the lock after the matrix is loaded
                {
                    lock (_lock)
                    {
                        if (_matrix == null) // prevent double initialization
                        {
                            LoadPermissionMatrix(null);
                        }
                    }
                }
                return _matrix!;
            }
        }

        public PermissionCache(PermissionMatrixLoader loader, FilePaths filePaths)
        {
            _loader = loader;
            _refreshExchange = new PermissionRefreshExchange(filePaths.PermissionRefreshFilePath);
        }

        /// <summary>
        /// Sets the partition identifier; must be called before the matrix is accessed
        /// </summary>
        public void Initialize(string partition)
        {
            Initialize(partition, new List<Subroute>());
        }

        /// <summary>
        /// Sets the partition identifier and subroute configuration; must be called before the matrix is accessed
        /// </summary>
        public void Initialize(string partition, List<Subroute> subroutes)
        {
            if (_matrix != null)
            {
                throw new InvalidOperationException("Cannot initialize after matrix is loaded");
            }

            Partition = partition;
            Subroutes = subroutes;
        }

        private void LoadPermissionMatrix(Guid? organizationId)
        {
            // The partition's permissions are merged into every other organization, and the merge
            // only adds. Reloading the partition alone would leave a revoked grant in every other
            // organization until the next full load, so a partition refresh reloads everything.
            var id = organizationId ?? Guid.Empty;

            var isPartialLoad = id != Guid.Empty;

            if (isPartialLoad && Partition.HasValue())
            {
                var organizationCode = _loader.GetOrganizationCode(id);

                if (StringHelper.Equals(organizationCode, Partition))
                    isPartialLoad = false;
            }

            if (isPartialLoad)
            {
                if (_matrix == null)
                {
                    _matrix = new PermissionMatrix();
                }

                _loader.Load(_matrix, id);
            }
            else
            {
                _matrix = new PermissionMatrix();

                _loader.Load(_matrix);
            }

            if (Partition.HasValue() && _matrix.ContainsOrganization(Partition))
            {
                _matrix.MergePermissions(Partition);
            }
        }

        /// <summary>
        /// Reloads the matrix, or loads a single organization's permissions if specified
        /// </summary>
        public void Refresh(Guid? organizationId)
        {
            lock (_lock)
            {
                LoadPermissionMatrix(organizationId);
            }
        }

        public bool RefreshIfNeeded()
        {
            if (!_refreshExchange.IsRefreshed())
                return false;

            _refreshExchange.RemoveMark();
            
            Refresh(null);

            return true;
        }
    }
}
