using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Marten.Internal;
using Marten.Linq.SqlGeneration;

namespace Marten.Linq.New;

public class CollectionUsage
{
    public CollectionUsage(Type elementType)
    {
        ElementType = elementType;
    }

    public Type ElementType { get; }
    public List<Ordering> Ordering { get; } = new();
    public List<Expression> Wheres { get; } = new();

    // TODO -- later?
    public CollectionUsage? Next { get; internal set; }

    public void WriteTake(int limit)
    {
        throw new NotImplementedException();
    }

    // This is the entry point at the top
    public SelectorStatement BuildStatement(IMartenSession session)
    {
        var storage = session.StorageFor(ElementType);

        var statement = new DocumentStatement(storage);

        return statement;
    }
}
