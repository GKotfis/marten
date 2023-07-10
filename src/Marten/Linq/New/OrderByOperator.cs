using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;

namespace Marten.Linq.New;

public class OrderByOperator: LinqOperator
{
    public OrderByOperator() : base("OrderBy")
    {

    }

    public override MethodCallExpression Apply(ILinqQuery query, MethodCallExpression expression)
    {
        var usage = query.CollectionUsageFor(expression);
        usage.Ordering.Add(new Ordering(expression.Arguments.Last(), OrderingDirection.Asc));
        return null;
    }
}

public enum CasingRule
{
    CaseSensitive,
    CaseInsensitive
}

public class Ordering
{
    public Expression Expression { get; }

    [Obsolete("eliminate OrderingDirection from Relinq")]
    public OrderingDirection Direction { get; }

    public CasingRule CasingRule { get; set; } = CasingRule.CaseInsensitive;

    public Ordering(Expression expression, OrderingDirection direction)
    {
        Expression = expression;
        Direction = direction;
    }

    [Obsolete("Temporary")]
    public (Remotion.Linq.Clauses.Ordering Ordering, bool CaseInsensitive) Convert()
    {
        return (new Remotion.Linq.Clauses.Ordering(Expression, Direction), CasingRule == CasingRule.CaseSensitive);
    }
}

