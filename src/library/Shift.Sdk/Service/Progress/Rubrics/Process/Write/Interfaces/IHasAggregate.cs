using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    internal interface IHasAggregate
    {
        RubricAggregate Aggregate { get; }
    }
}
