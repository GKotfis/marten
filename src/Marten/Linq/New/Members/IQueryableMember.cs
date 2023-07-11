using System;
using System.Reflection;
using Marten.Linq.New.Operators;

namespace Marten.Linq.New.Members;

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
    string BuildOrderingExpression(NewOrdering ordering);
}
