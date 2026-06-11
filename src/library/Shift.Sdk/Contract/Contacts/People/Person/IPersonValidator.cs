using System;
using System.Threading;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonValidator
    {
        Task<ValidationFailure> ValidateCommandAsync(
            ImportPerson person,
            Guid organization,
            int index,
            CancellationToken cancellation);
    }
}
