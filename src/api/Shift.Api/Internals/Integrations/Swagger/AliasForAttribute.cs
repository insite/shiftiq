namespace Shift.Api;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AliasForAttribute : Attribute
{
    public string AliasFor { get; }

    public AliasForAttribute(string aliasFor)
    {
        AliasFor = aliasFor;
    }
}