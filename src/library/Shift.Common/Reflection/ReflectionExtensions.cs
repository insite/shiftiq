using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Shift.Common
{
    public static class ReflectionExtensions
    {
        #region MemberInfo

        public static Func<TInstance, TOutput> BuildGetter<TInstance, TOutput>(this MemberInfo memberInfo)
        {
            // DeclaringType x
            var paramInstance = Expression.Parameter(memberInfo.DeclaringType, "x");

            // x.{MemberName}
            var getMember = Expression.MakeMemberAccess(paramInstance, memberInfo);

            // (DeclaringType x) => x.{MemberName}
            var lambda = Expression.Lambda<Func<TInstance, TOutput>>(getMember, paramInstance);

            return lambda.Compile();
        }

        public static Func<TInstance, object> BuildGetterWithSpecifiedInstance<TInstance>(this MemberInfo memberInfo)
        {
            // DeclaringType x
            var paramInstance = Expression.Parameter(memberInfo.DeclaringType, "x");

            // x.{MemberName}
            var accessMember = Expression.MakeMemberAccess(paramInstance, memberInfo);

            // x => x.{MemberName}
            var lambda = Expression.Lambda<Func<TInstance, object>>(accessMember, paramInstance);

            return lambda.Compile();
        }

        public static Func<object, TValue> BuildGetterWithSpecifiedValue<TValue>(this MemberInfo memberInfo)
        {
            // object x
            var paramInstance = Expression.Parameter(typeof(object), "x");

            // (DeclaringType)x
            var convertInstance = Expression.Convert(paramInstance, memberInfo.DeclaringType);

            // ((DeclaringType)x).{MemberName}
            var accessMember = Expression.MakeMemberAccess(convertInstance, memberInfo);

            // x => ((DeclaringType)x).{MemberName}
            var lambda = Expression.Lambda<Func<object, TValue>>(accessMember, paramInstance);
            
            return lambda.Compile();
        }

        public static Func<object, object> BuildGetter(this MemberInfo memberInfo)
        {
            // object x
            var paramInstance = Expression.Parameter(typeof(object), "x");

            // ((DeclaringType)x)
            var convertInstance = Expression.Convert(paramInstance, memberInfo.DeclaringType);

            // ((DeclaringType)x).{MemberName}
            var accessMember = Expression.MakeMemberAccess(convertInstance, memberInfo);

            // (object x) => ((DeclaringType)x).{MemberName}
            var lambda = Expression.Lambda<Func<object, object>>(accessMember, paramInstance);

            return lambda.Compile();
        }

        public static Action<TInstance, TInput> BuildSetter<TInstance, TInput>(this MemberInfo memberInfo)
        {
            // DeclaringType x
            var paramInstance = Expression.Parameter(memberInfo.DeclaringType, "x");

            // TInput v
            var paramValue = Expression.Parameter(typeof(TInput), "v");

            // x.{MemberName}
            var accessMember = Expression.MakeMemberAccess(paramInstance, memberInfo);

            // x.{MemberName} = v
            var bodyAssign = Expression.Assign(accessMember, paramValue);

            // (DeclaringType x, TInput v) => x.{MemberName} = v
            var lambda = Expression.Lambda<Action<TInstance, TInput>>(bodyAssign, paramInstance, paramValue);

            return lambda.Compile();
        }

        public static Action<TInstance, object> BuildSetterWithSpecifiedInstance<TInstance>(this MemberInfo memberInfo)
        {
            // DeclaringType x
            var paramInstance = Expression.Parameter(memberInfo.DeclaringType, "x");

            // object v
            var paramValue = Expression.Parameter(typeof(object), "v");

            // x.{MemberName}
            var accessMember = Expression.MakeMemberAccess(paramInstance, memberInfo);

            // (MemberType)v
            var convertValue = Expression.Convert(paramValue, memberInfo.GetUnderlyingType());

            // x.{MemberName} = (MemberType)v
            var bodyAssign = Expression.Assign(accessMember, convertValue);

            // (DeclaringType x, object v) => x.{MemberName} = (MemberType)v
            var lambda = Expression.Lambda<Action<TInstance, object>>(bodyAssign, paramInstance, paramValue);

            return lambda.Compile();
        }

        public static Action<object, TValue> BuildSetterWithSpecifiedValue<TValue>(this MemberInfo memberInfo)
        {
            // object x
            var paramInstance = Expression.Parameter(typeof(object), "x");

            // TValue v
            var paramValue = Expression.Parameter(typeof(TValue), "v");

            // (DeclaringType)x
            var convertInstance = Expression.Convert(paramInstance, memberInfo.DeclaringType);

            // ((DeclaringType)x).{MemberName}
            var accessMember = Expression.MakeMemberAccess(convertInstance, memberInfo);

            // ((DeclaringType)x).{MemberName} = v
            var bodyAssign = Expression.Assign(accessMember, paramValue);

            // (object x, TValue v) => ((DeclaringType)x).{MemberName} = v
            var lambda = Expression.Lambda<Action<object, TValue>>(bodyAssign, paramInstance, paramValue);

            return lambda.Compile();
        }

        public static Action<object, object> BuildSetter(this MemberInfo memberInfo)
        {
            // object x
            var paramInstance = Expression.Parameter(typeof(object), "x");

            // object v
            var paramValue = Expression.Parameter(typeof(object), "v");

            // ((DeclaringType)x)
            var convertInstance = Expression.Convert(paramInstance, memberInfo.DeclaringType);

            // ((DeclaringType)x).{MemberName}
            var accessMember = Expression.MakeMemberAccess(convertInstance, memberInfo);

            // (MemberType)v
            var convertValue = Expression.Convert(paramValue, memberInfo.GetUnderlyingType());

            // ((DeclaringType)x).{MemberName} = (MemberType)v
            var bodyAssign = Expression.Assign(accessMember, convertValue);

            // (object x, object v) => ((DeclaringType)x).{MemberName} = (MemberType)v
            var lambda = Expression.Lambda<Action<object, object>>(bodyAssign, paramInstance, paramValue);

            return lambda.Compile();
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Type

        public static IEnumerable<Type> GetAllAscendants(this Type t)
        {
            var current = t;

            while (current.BaseType != typeof(object))
            {
                yield return current.BaseType;

                current = current.BaseType;
            }
        }

        #endregion
    }
}
