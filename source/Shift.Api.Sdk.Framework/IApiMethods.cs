using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Refit;

using Shift.Contract.Requests;
using Shift.Contract.Responses;

namespace Shift.Api.Sdk.Framework
{
    [Headers("Authorization: Bearer")]
    public interface IApiMethods
    {
        [Post(ApiEndpoints.Analytics.Reports.ComplianceSummaries)]
        Task<IEnumerable<ComplianceSummaryResponse>> ComplianceSummaries(ComplianceSummaryRequest request, CancellationToken? token);
    }
}