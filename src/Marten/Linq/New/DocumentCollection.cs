using System;
using System.Reflection;
using Marten.Internal.Storage;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

// TODO -- make this deal with default Where clauses
public class DocumentCollection: IQueryableCollection
{
    public DocumentCollection(IDocumentStorage storage)
    {
        ElementType = storage.DocumentType;
        From = new FromFragment(storage.TableName, "d");
    }

    public Type ElementType { get; }
    public ISqlFragment From { get; }

    public IQueryableMember FindMember(MemberInfo member)
    {
        throw new NotImplementedException();
    }
}
