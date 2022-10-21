using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BetterSmithingContinued.Utilities
{
	public static class TypeExtensions
	{
		public static Activator<T> GetActivator<T>(this Type _type, params Type[] _parameterTypes)
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(_parameterTypes);
			ParameterExpression parameterExpression = Expression.Parameter(typeof(object[]), "_arguments");
			if (constructor == null)
			{
				return null;
			}
			Expression[] array = new Expression[_parameterTypes.Length];
			for (int i = 0; i < _parameterTypes.Length; i++)
			{
				UnaryExpression unaryExpression = Expression.Convert(Expression.ArrayIndex(parameterExpression, Expression.Constant(i)), _parameterTypes[i]);
				array[i] = unaryExpression;
			}
			return Expression.Lambda<Activator<T>>(Expression.New(constructor, array), new ParameterExpression[]
			{
				parameterExpression
			}).Compile();
		}
	}
}
