using System;
using System.Reflection;
using Marten.Linq.New.Members;

namespace Marten.Linq.New;

public interface IQueryableCollection
{
    Type ElementType { get; }
    IQueryableMember FindMember(MemberInfo member);

}