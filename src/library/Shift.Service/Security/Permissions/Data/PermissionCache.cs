using Shift.Common;

namespace Shift.Service
{
    /// <summary>
    /// Thread-safe cache for the application's permission matrix
    /// </summary>
    public class PermissionCache
    {
        private readonly PermissionMatrixLoader _loader;
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

        public PermissionCache(PermissionMatrixLoader loader)
        {
            _loader = loader;
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
            if (organizationId != null && organizationId != Guid.Empty)
            {
                if (_matrix == null)
                {
                    _matrix = new PermissionMatrix();
                }

                _loader.Load(_matrix, organizationId.Value);
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
    }
}
