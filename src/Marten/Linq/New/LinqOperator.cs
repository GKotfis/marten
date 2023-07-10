using System.Linq.Expressions;

namespace Marten.Linq.New;

public abstract class LinqOperator
{
    public string MethodName { get; }

    public LinqOperator(string methodName)
    {
        MethodName = methodName;
    }

    public abstract MethodCallExpression Apply(ILinqQuery query, MethodCallExpression expression);
}