using System;
using System.Linq.Expressions;
using System.Reflection;
using Models.Interfaces;

namespace Logic.Extensions;

/// <summary>
/// View model extensions
/// </summary>
public static class ViewModelExtension
{
    /// <summary>
    /// Property name or display name value all via linq
    /// </summary>
    /// <param name="_"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string PropName<T>(this T _, Expression<Func<T, object>> expression) where T : IViewModel
    {
        return MemberExpressionVisitor.ResolveMember(expression)?.Name;
    }
}
    
internal class MemberExpressionVisitor : ExpressionVisitor {

    private PropertyInfo _member;

    public static PropertyInfo ResolveMember(Expression expression)
    {
        var instance = new MemberExpressionVisitor();

        instance.Visit(expression);

        var result = instance._member;

        return result;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        _member = (PropertyInfo) node.Member;
            
        return base.VisitMember(node);
    }
}