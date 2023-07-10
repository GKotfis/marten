using System.Linq.Expressions;
using System.Reflection;

namespace Marten.Linq.New;

public interface ILinqQuery
{
    CollectionUsage CollectionUsageFor(MethodCallExpression expression);

}
