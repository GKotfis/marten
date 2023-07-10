using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Marten.Exceptions;
using Marten.Internal;
using Marten.Linq.QueryHandlers;
using Marten.Linq.SqlGeneration;

namespace Marten.Linq.New;

internal class LinqQueryParser: ExpressionVisitor, ILinqQuery
{
    // TODO -- inject somehow later
    private static readonly OperatorLibrary _operators = new();
    private readonly Stack<CollectionUsage> _collectionUsages = new();
    private readonly Expression _expression;

    private readonly IMartenSession _session;
    private CollectionUsage? _currentUsage;

    public LinqQueryParser(IMartenSession session,
        Expression expression)
    {
        _session = session;
        _expression = expression;

        Visit(expression);
    }

    public SelectorStatement CurrentStatement { get; set; }

    public Statement TopStatement { get; private set; }

    public CollectionUsage CollectionUsageFor(MethodCallExpression expression)
    {
        var elementType = expression.Arguments.First().Type.GetGenericArguments()[0];
        if (_currentUsage == null || _currentUsage.ElementType != elementType)
        {
            _currentUsage = new CollectionUsage(elementType);
            _collectionUsages.Push(_currentUsage);

            return _currentUsage;
        }

        return _currentUsage;
    }

    public IQueryHandler<IReadOnlyList<T>> BuildListHandler<T>()
    {
        if (!_collectionUsages.Any())
        {
            var usage = new CollectionUsage(typeof(T));
            _collectionUsages.Push(usage);
        }


        buildOutStatement();

        return CurrentStatement.SelectClause.BuildHandler<IReadOnlyList<T>>(_session, TopStatement, CurrentStatement);
    }

    public IQueryHandler<TResult> BuildHandler<TResult>()
    {

        throw new NotImplementedException();

        //
        // if (CurrentStatement.SingleValue)
        // {
        //     return CurrentStatement.BuildSingleResultHandler<TResult>(_session, TopStatement);
        // }


    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (_operators.TryFind(node.Method.Name, out var op))
        {
            return op.Apply(this, node);
        }

        throw new BadLinqExpressionException($"Marten does not (yet) support Linq operator '{node.Method.Name}'");
    }


    private void buildOutStatement()
    {
        var top = _collectionUsages.Peek();
        var statement = top.BuildStatement(_session);

        TopStatement = statement.Top();
        CurrentStatement = (SelectorStatement)statement.Current();

        TopStatement.CompileStructure(_session);
    }


}
