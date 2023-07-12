using System;
using System.Linq.Expressions;
using System.Reflection;
using Weasel.Core;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New.Members;

internal class NumericMember: QueryableMember, IComparableMember
{
    public NumericMember(string parentLocator, Casing casing, MemberInfo member) : base(parentLocator, casing, member)
    {
        // TODO -- throw if not numeric later

        var pgType = PostgresqlProvider.Instance.GetDatabaseType(MemberType, EnumStorage.AsInteger);

        TypedLocator = $"CAST({RawLocator} as {pgType})";

    }

    public ISqlFragment CreateComparison(string op, ConstantExpression constant)
    {
        if (constant.Value == null)
        {
            throw new NotImplementedException("Not yet");
            //return op == "=" ? new IsNullFilter(this) : new IsNotNullFilter(this);
        }

        var def = new CommandParameter(constant);
        return new ComparisonFilter(this, def, op);
    }
}
