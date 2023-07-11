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
    public List<Ordering> Ordering { get; } = new();
    public List<Expression> Wheres { get; } = new();

    public void WriteTake(int limit)
    {
        throw new NotImplementedException();
    }

    public NewStatement BuildStatement(DocumentCollection document)
    {
        throw new NotImplementedException();
    }

    public ISelectClause SelectClause { get; internal set; }
}

