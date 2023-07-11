using System;
using System.Reflection;

namespace Marten.Linq.New;

public interface IQueryableMember
{
    MemberInfo Member { get; }
    Type MemberType { get; }
}