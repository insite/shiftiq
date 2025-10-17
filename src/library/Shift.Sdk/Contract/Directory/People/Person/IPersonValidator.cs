using System.Threading;
using System.Threading.Tasks;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonValidator
    {
        Task<ValidationFailure> ValidateCommandAsync(ImportPerson person, int index,
            CancellationToken cancellation);
    }
}
