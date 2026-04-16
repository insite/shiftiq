using System;
using System.Threading.Tasks;

using Shift.Constant;

namespace Shift.Contract
{
    public interface ISequence
    {
        Task<int> IncrementAsync(Guid organizationId, SequenceType type, int startNumber = 1);
        Task<int[]> IncrementManyAsync(Guid organizationId, SequenceType type, int count, int startNumber = 1);
    }
}
