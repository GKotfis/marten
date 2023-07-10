using JasperFx.Core;

namespace Marten.Linq.New;

internal class OperatorLibrary
{
    private ImHashMap<string, LinqOperator> _operators = ImHashMap<string, LinqOperator>.Empty;

    public void Add<T>() where T : LinqOperator, new()
    {
        var op = new T();
        _operators = _operators.AddOrUpdate(op.MethodName, op);
    }

    public OperatorLibrary()
    {
        Add<TakeOperator>();
        Add<WhereOperator>();
        Add<OrderByOperator>();
        Add<SelectManyOperator>();
    }

    public bool TryFind(string methodName, out LinqOperator? op)
    {
        return (_operators.TryFind(methodName, out op)) ;
    }
}