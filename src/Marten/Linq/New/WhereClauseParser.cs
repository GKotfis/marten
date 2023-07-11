using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

public class WhereClauseParser : RelinqExpressionVisitor
{
    private readonly IQueryableCollection _collection;
    private readonly IWhereFragmentHolder _holder;

    public WhereClauseParser(IQueryableCollection collection, IWhereFragmentHolder holder)
    {
        _collection = collection;
        _holder = holder;
    }

    protected override Expression VisitNew(NewExpression expression)
    {
        return base.VisitNew(expression);
    }

    protected override Expression VisitSubQuery(SubQueryExpression expression)
    {
        return base.VisitSubQuery(expression);
    }

    protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
    {
        return base.VisitQuerySourceReference(expression);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        return base.VisitBinary(node);
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        return base.VisitUnary(node);
    }


}
