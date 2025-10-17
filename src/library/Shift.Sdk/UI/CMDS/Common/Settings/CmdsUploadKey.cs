using System;

namespace Shift.Sdk.UI
{
    public struct CmdsUploadKey : IEquatable<CmdsUploadKey>
    {
        #region Properties

        public Guid ContainerId { get; private set; }
        public string Name { get; private set; }

        #endregion

        #region Construction

        public CmdsUploadKey(Guid container, string name)
        {
            if (container == Guid.Empty)
                throw new ArgumentNullException(nameof(container));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            ContainerId = container;
            Name = name;
        }

        #endregion

        #region Helper methods

        public CmdsUploadKey Copy() => new CmdsUploadKey(ContainerId, Name);

        #endregion

        #region IEquatable

        public bool Equals(CmdsUploadKey other)
        {
            return this.ContainerId == other.ContainerId
                && StringComparer.OrdinalIgnoreCase.Equals(this.Name, other.Name);
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash = hash * 23 + ContainerId.GetHashCode();
            hash = hash * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(Name);

            return hash;
        }

        public override string ToString() => $"{{{ContainerId}: {Name}}}";

        #endregion
    }
}