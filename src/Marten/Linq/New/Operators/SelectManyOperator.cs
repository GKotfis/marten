using System;
using System.Linq.Expressions;

namespace Marten.Linq.New.Operators;

public class SelectManyOperator: LinqOperator
{
    public SelectManyOperator() : base("SelectMany")
    {
    }

    public override MethodCallExpression Apply(ILinqQuery query, MethodCallExpression expression)
    {
        throw new NotImplementedException();

        return null;
    }
}
