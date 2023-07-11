using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Marten.Linq.SqlGeneration;

namespace Marten.Linq.New;

public class CollectionUsage
{
    public CollectionUsage(Type elementType)
    {
        ElementType = elementType;
    }

    public Type ElementType { get; }
    public List<NewOrdering> Ordering { get; } = new();
    public List<Expression> Wheres { get; } = new();


    private int? _limit;
    public void WriteLimit(int limit)
    {
        _limit ??= limit; // don't overwrite
    }

    private int? _offset;
    public void WriteOffset(int offset)
    {
        _offset ??= offset; // don't overwrite
    }




    public NewStatement BuildStatement(DocumentCollection collection)
    {
        var statement = new NewSelectorStatement
        {
            SelectClause = collection.SelectClause,
            Limit = _limit,
            Offset = _offset
        };

        // TODO - Skip, Ordering, Where

        foreach (var ordering in Ordering)
        {
            statement.Ordering.Expressions.Add(ordering.BuildExpression(collection));
        }


        return statement;
    }


}

