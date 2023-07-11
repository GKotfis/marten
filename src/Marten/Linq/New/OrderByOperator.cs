using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Marten.Linq.Parsing;
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
        usage.Ordering.Add(new NewOrdering(expression.Arguments.Last(), OrderingDirection.Asc));
        return null;
    }
}

public enum CasingRule
{
    CaseSensitive,
    CaseInsensitive
}

public class NewOrdering
{
    public Expression Expression { get; }

    [Obsolete("eliminate OrderingDirection from Relinq")]
    public OrderingDirection Direction { get; }

    public CasingRule CasingRule { get; set; } = CasingRule.CaseInsensitive;

    public NewOrdering(Expression expression, OrderingDirection direction)
    {
        Expression = expression;
        Direction = direction;
    }

    public string BuildExpression(IQueryableCollection collection)
    {
        var members = FindMembers.Determine(Expression);
        if (members.Length != 1) throw new NotImplementedException("Not ready for multi-level ordering quite yet");

        var member = collection.FindMember(members[0]);

        return member.BuildOrderingExpression(this);
    }
}

