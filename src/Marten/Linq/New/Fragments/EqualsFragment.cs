using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New.Fragments;

public record EqualsFragment(string Locator, object Value) : ISqlFragment
{
    public void Apply(CommandBuilder builder)
    {
        builder.Append(Locator);
        builder.Append(" = ");
        builder.AppendParameter(Value);
    }

    public bool Contains(string sqlText)
    {
        return false;
    }
}
