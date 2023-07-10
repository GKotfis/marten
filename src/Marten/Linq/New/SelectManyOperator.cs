using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using JasperFx.Core.Reflection;

namespace Marten.Linq.New;

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
