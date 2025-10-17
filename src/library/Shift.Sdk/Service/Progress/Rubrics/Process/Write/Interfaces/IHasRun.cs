using InSite.Domain.Records;

namespace InSite.Application.Rubrics.Write
{
    internal interface IHasRun
    {
        bool Run(RubricAggregate course);
    }
}
