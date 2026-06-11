using System.Collections.Generic;

namespace InSite.Persistence
{
    public class CreateAssetNodeComparer : IEqualityComparer<CreateAssetNode>
    {
        public bool Equals(CreateAssetNode x, CreateAssetNode y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(CreateAssetNode x) { return x.Id; }
    }
}
