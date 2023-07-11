using System;
using System.Reflection;
using System.Text.Json.Serialization;
using JasperFx.Core.Reflection;
using Marten.Util;
using Newtonsoft.Json;
using Weasel.Core;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

public interface IQueryableMember
{
    MemberInfo Member { get; }
    Type MemberType { get; }

    /// <summary>
    ///     Postgresql locator that also casts the raw string data to the proper Postgresql type
    /// </summary>
    string TypedLocator { get; }

    /// <summary>
    ///     Postgresql locator that returns the raw string value within the JSONB document
    /// </summary>
    string RawLocator { get; }

    /// <summary>
    /// Build the locator or expression for usage within "ORDER BY" clauses
    /// </summary>
    /// <param name="ordering"></param>
    /// <returns></returns>
    string BuildOrderingExpression(Ordering ordering);
}

public abstract class QueryableMember: IQueryableMember
{

    public MemberInfo Member { get; }

    public string RawLocator { get; protected set; }

    public virtual string BuildOrderingExpression(Ordering ordering)
    {
        // TODO -- memoize or intern this
        return $"{TypedLocator} {ordering.Direction}";
    }

    public string TypedLocator { get; protected set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="parentLocator">JSONB location of the parent element</param>
    /// <param name="member"></param>
    protected QueryableMember(string parentLocator, Casing casing, MemberInfo member)
    {
        Member = member;
        MemberType = member.GetMemberType();

        RawLocator = TypedLocator = $"{parentLocator} ->> {member.ToJsonKey(casing)}";

    }

    public Type MemberType { get; }


}

internal class NumericMember: QueryableMember
{
    public NumericMember(string parentLocator, Casing casing, MemberInfo member) : base(parentLocator, casing, member)
    {
        // TODO -- throw if not numeric later

        var pgType = PostgresqlProvider.Instance.GetDatabaseType(MemberType, EnumStorage.AsInteger);

        TypedLocator = $"CAST({RawLocator} as {pgType})";

    }
}



