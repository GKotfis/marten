using System;
using System.Linq;
using System.Linq.Expressions;
using JasperFx.Core;
using JasperFx.Core.Reflection;

namespace Marten.Linq.New.Operators;

public class WhereOperator: LinqOperator
{
    private interface IFuncBuilder
    {
        object Build();
    }

    private class FuncBuilder<T> : IFuncBuilder
    {
        public object Build()
        {
            Expression<Func<T, bool>> filter = _ => true;
            return filter;
        }
    }

    private readonly Cache<Type, object> _always
        = new(type => typeof(FuncBuilder<>).CloseAndBuildAs<IFuncBuilder>(type).Build());


    public WhereOperator() : base("Where")
    {

    }

    public override MethodCallExpression Apply(ILinqQuery query, MethodCallExpression expression)
    {
        var usage = query.CollectionUsageFor(expression);
        var where = expression.Arguments.Last();
        if (where is UnaryExpression e) where = e.Operand;
        if (where is LambdaExpression l) where = l.Body;

        usage.Wheres.Add(where);

        var elementType = usage.ElementType;
        return Expression.Call(expression.Method, expression.Arguments[0], Expression.Constant(_always[elementType]));
    }
}
