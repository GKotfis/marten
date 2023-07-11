using System.Reflection;
using Weasel.Core;
using Weasel.Postgresql;

namespace Marten.Linq.New.Members;

internal class NumericMember: QueryableMember
{
    public NumericMember(string parentLocator, Casing casing, MemberInfo member) : base(parentLocator, casing, member)
    {
        // TODO -- throw if not numeric later

        var pgType = PostgresqlProvider.Instance.GetDatabaseType(MemberType, EnumStorage.AsInteger);

        TypedLocator = $"CAST({RawLocator} as {pgType})";

    }
}