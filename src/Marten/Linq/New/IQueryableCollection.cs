using System;
using System.Reflection;

namespace Marten.Linq.New;

public interface IQueryableCollection
{
    Type ElementType { get; }
    IQueryableMember FindMember(MemberInfo member);

}