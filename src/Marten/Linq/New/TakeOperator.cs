using System.Linq;
using System.Linq.Expressions;
using JasperFx.Core.Reflection;

namespace Marten.Linq.New;

public class TakeOperator: LinqOperator
{
    public TakeOperator() : base("Take")
    {
    }

    public override MethodCallExpression Apply(ILinqQuery query, MethodCallExpression expression)
    {
        var usage = query.CollectionUsageFor(expression);
        usage.WriteTake(expression.Arguments.Last().As<int>());

        return expression;
    }
}
