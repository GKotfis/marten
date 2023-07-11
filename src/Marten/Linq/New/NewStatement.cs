using System.Collections.Generic;
using Marten.Linq.SqlGeneration;
using Weasel.Postgresql;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

public abstract class NewStatement: ISqlFragment
{
    public NewStatement Next { get; set; }
    public NewStatement Previous { get; set; }

    public StatementMode Mode { get; set; } = StatementMode.Select;

    public NewStatement Top()
    {
        return Previous == null ? this : Previous.Top();
    }

    public NewStatement Current()
    {
        return Next == null ? this : Next.Current();
    }

    public void Apply(CommandBuilder builder)
    {
        foreach (var fragment in fragments())
        {
            fragment.Apply(builder);
        }

        if (Next != null)
        {
            builder.Append(" ");
            Next.Apply(builder);
        }
    }

    protected abstract IEnumerable<ISqlFragment> fragments();

    bool ISqlFragment.Contains(string sqlText)
    {
        // TODO -- revisit this later for multi-tenancy searching
        return false;
    }
}
