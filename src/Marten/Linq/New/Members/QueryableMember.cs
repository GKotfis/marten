using System;
using System.Reflection;
using JasperFx.Core.Reflection;
using Marten.Linq.New.Operators;
using Marten.Util;
using Remotion.Linq.Clauses;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New.Members;

public abstract class QueryableMember: IQueryableMember
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="parentLocator">JSONB location of the parent element</param>
    /// <param name="member"></param>
    protected QueryableMember(string parentLocator, Casing casing, MemberInfo member)
    {
        Member = member;
        MemberType = member.GetMemberType();

        RawLocator = TypedLocator = $"{parentLocator} ->> '{member.ToJsonKey(casing)}'";
    }

    public MemberInfo Member { get; }

    public string RawLocator { get; protected set; }


    public string TypedLocator { get; protected set; }

    public Type MemberType { get; }

    public virtual string BuildOrderingExpression(NewOrdering ordering)
    {
        // TODO -- memoize or intern this
        if (ordering.Direction == OrderingDirection.Desc) return $"{TypedLocator} desc";

        return TypedLocator;
    }


    void ISqlFragment.Apply(CommandBuilder builder)
    {
        builder.Append(TypedLocator);
    }

    bool ISqlFragment.Contains(string sqlText)
    {
        return false;
    }
}
