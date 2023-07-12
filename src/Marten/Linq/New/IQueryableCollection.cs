using System;
using System.Linq.Expressions;
using System.Reflection;
using Marten.Linq.New.Members;

namespace Marten.Linq.New;

public interface IQueryableCollection
{
    Type ElementType { get; }
    IQueryableMember FindMember(MemberInfo member);

    // Assume there's a separate member for the usage of a member with methods
    // i.e., StringMember.ToLower()
    // Dictionary fields will create a separate "dictionary value field"
    IQueryableMember MemberFor(Expression expression);
}
