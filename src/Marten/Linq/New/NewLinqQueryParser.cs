using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Marten.Exceptions;
using Marten.Internal;
using Marten.Internal.Storage;
using Marten.Linq.Fields;
using Marten.Linq.QueryHandlers;
using Marten.Linq.SqlGeneration;

namespace Marten.Linq.New;

public interface ILinqStatement
{

}

internal class NewLinqQueryParser : ExpressionVisitor, ILinqStatement
{
    // TODO -- inject somehow later
    private static OperatorLibrary _operators = new();

    private readonly IMartenSession _session;
    private readonly Expression _expression;
    private readonly Stack<CollectionUsage> _collectionUsages = new();

    public SelectorStatement CurrentStatement { get; set; }

    public Statement TopStatement { get; private set; }

    public NewLinqQueryParser(IMartenSession session,
        Expression expression)
    {
        _session = session;
        _expression = expression;

        Visit(expression);
    }

    public IQueryHandler<TResult> BuildHandler<TResult>()
    {
        throw new NotImplementedException();
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (_operators.TryFind(node.Method.Name, out var op))
        {
            var response = op.Apply(this, node);
            var returnValue = base.VisitMethodCall(node);

            Debug.WriteLine( node.Method.Name + ": Is is the same? " + ReferenceEquals(response, returnValue));

            return response;
        }
        else
        {
            return base.VisitMethodCall(node);
            //throw new BadLinqExpressionException($"Marten does not (yet) support Linq operator '{node.Method.Name}'");
        }


    }
}

internal class CollectionUsage
{
    public Type ElementType { get; }

    public CollectionUsage(Type elementType)
    {
        ElementType = elementType;
    }
}

internal class OperatorLibrary
{
    private ImHashMap<string, LinqOperator> _operators = ImHashMap<string, LinqOperator>.Empty;

    public void Add<T>() where T : LinqOperator, new()
    {
        var op = new T();
        _operators = _operators.AddOrUpdate(op.MethodName, op);
    }

    public OperatorLibrary()
    {
        Add<TakeOperator>();
        Add<WhereOperator>();
        Add<OrderByOperator>();
        Add<SelectManyOperator>();
    }

    public bool TryFind(string methodName, out LinqOperator? op)
    {
        return (_operators.TryFind(methodName, out op)) ;
    }
}

public class TakeOperator: LinqOperator
{
    public TakeOperator() : base("Take")
    {
    }

    public override MethodCallExpression Apply(ILinqStatement statement, MethodCallExpression expression)
    {
        // Nothing
        Debug.WriteLine("Applying " + GetType().Name);
        return expression;
    }
}

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

    private readonly Cache<Type, object> _always = new Cache<Type, object>(type =>
    {
        return typeof(FuncBuilder<>).CloseAndBuildAs<IFuncBuilder>(type).Build();
    });


    public WhereOperator() : base("Where")
    {

    }

    public override MethodCallExpression Apply(ILinqStatement statement, MethodCallExpression expression)
    {
        Debug.WriteLine("Applying " + GetType().Name);
        var elementType = expression.Type.GetGenericArguments()[0];
        return Expression.Call(expression.Method, expression.Arguments[0], Expression.Constant(_always[elementType]));
    }
}



public class OrderByOperator: LinqOperator
{
    public OrderByOperator() : base("OrderBy")
    {

    }

    public override MethodCallExpression Apply(ILinqStatement statement, MethodCallExpression expression)
    {
        Debug.WriteLine("Applying " + GetType().Name);
        return expression;
    }
}

public class SelectManyOperator: LinqOperator
{
    public SelectManyOperator() : base("SelectMany")
    {
    }

    public override MethodCallExpression Apply(ILinqStatement statement, MethodCallExpression expression)
    {
        // Nothing
        Debug.WriteLine("Applying " + GetType().Name);
        return expression;
    }
}

public abstract class LinqOperator
{
    public string MethodName { get; }

    public LinqOperator(string methodName)
    {
        MethodName = methodName;
    }

    public abstract MethodCallExpression Apply(ILinqStatement statement, MethodCallExpression expression);
}



