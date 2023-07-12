using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Marten.Exceptions;
using Marten.Linq.Fields;
using Marten.Linq.New.Members;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;
using Weasel.Postgresql.SqlGeneration;

namespace Marten.Linq.New;

public partial class NewWhereClauseParser : RelinqExpressionVisitor
{
    private static readonly Dictionary<ExpressionType, string> _operators = new()
    {
        { ExpressionType.Equal, "=" },
        { ExpressionType.NotEqual, "!=" },
        { ExpressionType.GreaterThan, ">" },
        { ExpressionType.GreaterThanOrEqual, ">=" },
        { ExpressionType.LessThan, "<" },
        { ExpressionType.LessThanOrEqual, "<=" }
    };


    private readonly IQueryableCollection _collection;
    private IWhereFragmentHolder _holder;

    public NewWhereClauseParser(IQueryableCollection collection, IWhereFragmentHolder holder)
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
        if (_operators.TryGetValue(node.NodeType, out var op))
        {
            var binary = new BinaryExpressionVisitor(this);
            _holder.Register(binary.BuildWhereFragment(node, op));

            return null;
        }

        switch (node.NodeType)
        {
            case ExpressionType.AndAlso:
                throw new NotImplementedException("Not yet");
                //buildCompoundWhereFragment(node, "and");
                break;

            case ExpressionType.OrElse:
                throw new NotImplementedException("Not yet");
                //buildCompoundWhereFragment(node, "or");
                break;

            default:
                throw new BadLinqExpressionException(
                    $"Unsupported expression type '{node.NodeType}' in binary expression");
        }


        return null;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.NodeType == ExpressionType.Not)
        {
            var original = _holder;

            if (original is IReversibleWhereFragment r)
            {
                _holder.Register(r.Reverse());
                return Visit(node.Operand);
            }

            _holder = new NotWhereFragment(original);
            var returnValue = Visit(node.Operand);

            _holder = original;

            return returnValue;
        }

        return null;
    }

    internal class BinaryExpressionVisitor: RelinqExpressionVisitor
    {
        private readonly NewWhereClauseParser _parent;
        private BinarySide _left;
        private BinarySide _right;

        public BinaryExpressionVisitor(NewWhereClauseParser parent)
        {
            _parent = parent;
        }

        public ISqlFragment BuildWhereFragment(BinaryExpression node, string op)
        {
            _left = analyze(node.Left);
            _right = analyze(node.Right);

            return _left.CompareTo(_right, op);
        }

        private BinarySide analyze(Expression expression)
        {
            switch (expression)
            {
                case ConstantExpression c:
                    return new BinarySide(expression) { Constant = c };
                case PartialEvaluationExceptionExpression p:
                {
                    var inner = p.Exception;

                    throw new BadLinqExpressionException(
                        $"Error in value expression inside of the query for '{p.EvaluatedExpression}'. See the inner exception:",
                        inner);
                }
                case SubQueryExpression subQuery:
                {
                    throw new NotImplementedException("Not yet");
                    // var parser = new WhereClauseParser.SubQueryFilterParser(_parent, subQuery);
                    //
                    // return new BinarySide(expression) { Comparable = parser.BuildCountComparisonStatement() };
                }
                case QuerySourceReferenceExpression source:
                    throw new NotImplementedException("Not yet");
                    //return new BinarySide(expression) { Member = new SimpleDataField(source.Type) };
                case BinaryExpression { NodeType: ExpressionType.Modulo } binary:
                    throw new NotImplementedException("Not yet");
                    // return new BinarySide(expression)
                    // {
                    //     Comparable = new ModuloFragment(binary, _parent._statement.Fields)
                    // };

                case BinaryExpression { NodeType: ExpressionType.NotEqual } ne:
                    throw new NotImplementedException("Not yet");
                    // if (ne.Right is ConstantExpression v && v.Value == null)
                    // {
                    //     var field = _parent._statement.Fields.FieldFor(ne.Left);
                    //     return new BinarySide(expression) { Comparable = new HasValueField(field) };
                    // }

                    throw new BadLinqExpressionException("Invalid Linq Where() clause with expression: " + ne);
                case BinaryExpression binary:
                    throw new BadLinqExpressionException(
                        $"Unsupported nested operator '{binary.NodeType}' as an operand in a binary expression");

                case UnaryExpression u when u.NodeType == ExpressionType.Not:
                    throw new NotImplementedException("Not yet");
                    // return new BinarySide(expression)
                    // {
                    //     Comparable = new NotField(_parent._statement.Fields.FieldFor(u.Operand))
                    // };

                default:
                    return new BinarySide(expression) { Member = _parent._collection.MemberFor(expression) };
            }
        }



    }

    internal class BinarySide
    {
        public BinarySide(Expression memberExpression)
        {
            MemberExpression = memberExpression as MemberExpression;
        }

        public ConstantExpression Constant { get; set; }
        public IQueryableMember Member { get; set; }

        public MemberExpression MemberExpression { get; }

        public ISqlFragment CompareTo(BinarySide right, string op)
        {
            if (Constant != null)
            {
                return right.CompareTo(this, ComparisonFilter.OppositeOperators[op]);
            }

            if (Member is IComparableMember m && right.Constant != null)
            {
                return m.CreateComparison(op, right.Constant);
            }

            if (Member == null)
            {
                throw new BadLinqExpressionException("Unsupported binary value expression in a Where() clause");
            }

            if (right.Constant != null && Member is IComparableMember comparableMember)
            {
                return comparableMember.CreateComparison(op, right.Constant);
            }

            if (right.Member != null)
            {
                // TODO -- this will need to evaluate extra methods in the comparison. Looking for StringProp.ToLower() == "foo"
                return new ComparisonFilter(Member, right.Member, op);
            }


            throw new BadLinqExpressionException("Unsupported binary value expression in a Where() clause");
        }
    }
}
