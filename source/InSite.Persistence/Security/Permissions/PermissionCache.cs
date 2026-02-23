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
        public static void Initialize(string partition, RouteSettings routeSettings)
        {
            if (_matrix != null)
            {
                throw new InvalidOperationException("Cannot initialize after matrix is loaded");
            }

            Partition = partition;
            RouteSettings = routeSettings;
        }

        private static void LoadPermissionMatrix(Guid? organizationId)
        {
            var loader = new PermissionMatrixLoader(RouteSettings);

            if (organizationId != null && organizationId != Guid.Empty)
            {
                if (_matrix == null)
                {
                    _matrix = new PermissionMatrix();
                }

                loader.Load(_matrix, organizationId.Value);
            }
            else
            {
                _matrix = new PermissionMatrix();

                loader.Load(_matrix);
            }

            if (Partition.HasValue() && _matrix.ContainsOrganization(Partition))
            {
                _matrix.MergePermissions(Partition);
            }
        }

        /// <summary>
        /// Reloads the matrix, or loads a single organization's permissions if specified
        /// </summary>
        public static void Refresh(Guid? organizationId)
        {
            lock (_lock)
            {
                LoadPermissionMatrix(organizationId);
            }
        }
    }
}