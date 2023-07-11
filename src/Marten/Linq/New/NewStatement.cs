using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
        apply(builder);

        builder.Append(";");

        if (Next != null)
        {
            builder.Append(" ");
            Next.Apply(builder);
        }
    }

    protected abstract void apply(CommandBuilder builder);

    bool ISqlFragment.Contains(string sqlText)
    {
        // TODO -- revisit this later for multi-tenancy searching
        return false;
    }
}

public class NewSelectorStatement : NewStatement
{
    //public ISqlFragment FromClause { get; set; }

    public int? Limit { get; set; }
    public int? Offset { get; set; }

    protected override void apply(CommandBuilder builder)
    {
        SelectClause.Apply(builder);

        if (Wheres.Any())
        {
            builder.Append(" where ");
            foreach (var where in Wheres)
            {
                where.Apply(builder);
            }
        }

        Ordering.Apply(builder);

        if (Offset.HasValue)
        {
            builder.Append(" OFFSET ");
            builder.AppendParameter(Offset.Value);
        }

        if (Limit.HasValue)
        {
            builder.Append(" LIMIT ");
            builder.AppendParameter(Limit.Value);
        }
    }


    public List<ISqlFragment> Wheres { get; } = new();
    public OrderByFragment Ordering { get; } = new();

    // TODO -- this would conceivably overwritten depending on usage of
    // Linq operators
    public ISelectClause SelectClause { get; internal set; }
}

public class OrderByFragment: ISqlFragment
{
    public List<string> Expressions { get; } = new();

    public void Apply(CommandBuilder builder)
    {
        if (!Expressions.Any()) return;

        builder.Append(" order by ");
        builder.Append(Expressions[0]);
        for (int i = 1; i < Expressions.Count; i++)
        {
            builder.Append(", ");
            builder.Append(Expressions[i]);
        }
    }

    public bool Contains(string sqlText)
    {
        return Expressions.Any(x => x.Contains(sqlText));
    }
}




