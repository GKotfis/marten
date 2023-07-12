using System;
using System.Linq.Expressions;
using System.Reflection;
using Marten.Linq.New.Operators;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New.Members;


public interface IQueryableMember : ISqlFragment
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

public interface IComparableMember
{
    ISqlFragment CreateComparison(string op, ConstantExpression constant);
}
