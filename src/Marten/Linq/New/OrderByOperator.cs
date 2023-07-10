using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
    public OrderingDirection Direction { get; }

    public CasingRule CasingRule { get; set; } = CasingRule.CaseSensitive;

    public Ordering(Expression expression, OrderingDirection direction)
    {
        Expression = expression;
        Direction = direction;
    }
}

/// <summary>
/// Specifies the direction used to sort the result items in a query using an <see cref="T:Remotion.Linq.Clauses.OrderByClause" />.
/// </summary>
public enum OrderingDirection
{
    /// <summary>
    /// Sorts the items in an ascending way, from smallest to largest.
    /// </summary>
    Asc,
    /// <summary>
    /// Sorts the items in an descending way, from largest to smallest.
    /// </summary>
    Desc,
}
