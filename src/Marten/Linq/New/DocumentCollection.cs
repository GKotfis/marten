using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JasperFx.Core;
using JasperFx.Core.Reflection;
using Marten.Internal.Storage;
using Marten.Linq.New.Members;
using Marten.Linq.SqlGeneration;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

// TODO -- make this deal with default Where clauses
public class DocumentCollection: IQueryableCollection
{
    private ImHashMap<string, IQueryableMember> _members = ImHashMap<string, IQueryableMember>.Empty;

    public DocumentCollection(IDocumentStorage storage, StoreOptions options)
    {
        ElementType = storage.DocumentType;
        From = new FromFragment(storage.TableName, "d");
        SelectClause = storage;
        Serializer = options.Serializer();
    }

    public ISerializer Serializer { get; }

    public Type ElementType { get; }
    public ISqlFragment From { get; }

    public IQueryableMember FindMember(MemberInfo member)
    {
        if (_members.TryFind(member.Name, out var m)) return m;

        m = createQuerableMember(member);
        _members = _members.AddOrUpdate(member.Name, m);

        return m;
    }

    public IQueryableMember MemberFor(Expression expression)
    {
        // TODO -- NEEDS TO GET MORE COMPLICATED LATER

        var members = FindMembers.Determine(expression);

        if (members.Length != 1)
            throw new NotImplementedException("Hey, deep accessors aren't yet supported in the new Linq provider");

        return FindMember(members.Single());
    }

    private IQueryableMember createQuerableMember(MemberInfo member)
    {
        // TODO -- this will need to get MUCH fancier later
        var memberType = member.GetMemberType();
        if (memberType == typeof(int)) return new NumericMember("d.data", Serializer.Casing, member);
        if (memberType == typeof(double)) return new NumericMember("d.data", Serializer.Casing, member);

        throw new NotSupportedException("Just no there yet for fields of type " + memberType.FullNameInCode());
    }

    public ISelectClause SelectClause { get; }
}


