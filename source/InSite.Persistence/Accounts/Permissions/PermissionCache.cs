using System;

using Shift.Common;

namespace InSite.Persistence
{
    /// <summary>
    /// Thread-safe cache for the application's permission matrix
    /// </summary>
    public static class PermissionCache
    {
        private static volatile PermissionMatrix _matrix;

        private static readonly object _lock = new object();

        private static PermissionRefreshExchange _refreshExchange;

        /// <summary>
        /// The date and time when the permission cache was last loaded (or refreshed).
        /// </summary>
        public static DateTimeOffset LoadedAt { get; private set; }

        /// <summary>
        /// Partition identifier used to merge partition-wide permissions with organization-specific permissions
        /// </summary>
        public static string Partition { get; private set; }

        /// <summary>
        /// Subroute configuration for permission inheritance
        /// </summary>
        public static RouteSettings RouteSettings { get; private set; }

        /// <summary>
        /// Gets the permission matrix, loading it on first access
        /// </summary>
        public static PermissionMatrix Matrix
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
                return _matrix;
            }
        }

        /// <summary>
        /// Sets the partition identifier and subroute configuration; must be called before the matrix is accessed
        /// </summary>
        public static void Initialize(string partition, RouteSettings routeSettings, FilePaths filePaths)
        {
            if (_matrix != null)
            {
                throw new InvalidOperationException("Cannot initialize after matrix is loaded");
            }

            Partition = partition;
            RouteSettings = routeSettings;

            _refreshExchange = new PermissionRefreshExchange(filePaths.PermissionRefreshFilePath);
        }

        private static void LoadPermissionMatrix(Guid? organizationId)
        {
            LoadedAt = DateTimeOffset.Now;

            var loader = new PermissionMatrixLoader(RouteSettings);

            PermissionMatrix matrix;

            // The partition's permissions are merged into every other organization, and the merge
            // only adds. Reloading the partition alone would leave a revoked grant in every other
            // organization until the next full load, so a partition refresh reloads everything.
            var isPartialLoad = organizationId != null && organizationId != Guid.Empty;

            if (isPartialLoad && Partition.HasValue())
            {
                var organizationCode = loader.GetOrganizationCode(organizationId.Value);

                if (StringHelper.Equals(organizationCode, Partition))
                    isPartialLoad = false;
            }

            if (isPartialLoad)
            {
                matrix = _matrix ?? new PermissionMatrix();

                loader.Load(matrix, organizationId.Value);
            }
            else
            {
                matrix = new PermissionMatrix();

                loader.Load(matrix);
            }

            if (Partition.HasValue() && matrix.ContainsOrganization(Partition))
            {
                matrix.MergePermissions(Partition);
            }

            _matrix = matrix;
        }

        /// <summary>
        /// Reloads the matrix, or loads a single organization's permissions if specified
        /// </summary>
        public static void Refresh(Guid? organizationId)
        {
            lock (_lock)
            {
                LoadPermissionMatrix(organizationId);

                if (_refreshExchange != null)
                    _refreshExchange.MarkRefreshed();
            }
        }
    }
}
