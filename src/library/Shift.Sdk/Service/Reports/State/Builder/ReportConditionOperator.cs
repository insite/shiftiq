namespace InSite.Domain.Reports
{
    public enum ReportConditionOperator
    {
        None,
        IsNull,
        IsNotNull,
        Equal,
        NotEqual,
        In,
        NotIn,
        Less,
        LessOrEqual,
        Greater,
        GreaterOrEqual,
        StartWith,
        Contain,
        EndWith,
        NotStartWith,
        NotContain,
        NotEndWith
    }
}
