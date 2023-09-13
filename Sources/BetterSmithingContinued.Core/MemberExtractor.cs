using System.Reflection;

namespace BetterSmithingContinued.Core
{
	public class MemberExtractor
	{
		// FieldInfo

		public static FieldInfo GetFieldInfo<ObjectType>(string fieldName)
		{
			return typeof(ObjectType).GetField(fieldName, PublicMemberFlags);
		}

		public static FieldInfo GetPrivateFieldInfo<ObjectType>(string fieldName)
		{
			return typeof(ObjectType).GetField(fieldName, PrivateMemberFlags);
		}

		public static FieldInfo GetStaticFieldInfo<ObjectType>(string fieldName)
		{
			return typeof(ObjectType).GetField(fieldName, StaticPublicMemberFlags);
		}

		public static FieldInfo GetStaticPrivateFieldInfo<ObjectType>(string fieldName)
		{
			return typeof(ObjectType).GetField(fieldName, StaticPrivateMemberFlags);
		}

		// PropertyInfo

		public static PropertyInfo GetPropertyInfo<ObjectType>(string propertyName)
		{
			return typeof(ObjectType).GetProperty(propertyName, PublicMemberFlags);
		}

		public static PropertyInfo GetPrivatePropertyInfo<ObjectType>(string propertyName)
		{
			return typeof(ObjectType).GetProperty(propertyName, PrivateMemberFlags);
		}

		public static PropertyInfo GetStaticPropertyInfo<ObjectType>(string propertyName)
		{
			return typeof(ObjectType).GetProperty(propertyName, StaticPublicMemberFlags);
		}

		public static PropertyInfo GetStaticPrivatePropertyInfo<ObjectType>(string propertyName)
		{
			return typeof(ObjectType).GetProperty(propertyName, StaticPrivateMemberFlags);
		}

		// MethodInfo

		public static MethodInfo GetMethodInfo<ObjectType>(string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, PublicMemberFlags);
		}

		public static MethodInfo GetPrivateMethodInfo<ObjectType>(string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, PrivateMemberFlags);
		}

		public static MethodInfo GetStaticMethodInfo<ObjectType>(string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, StaticPublicMemberFlags);
		}

		public static MethodInfo GetStaticPrivateMethodInfo<ObjectType>(string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, StaticPrivateMemberFlags);
		}

		// FieldValue

		public static FieldType GetFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName)
		{
			return (FieldType) GetFieldInfo<ObjectType>(fieldName)?.GetValue(obj);
		}

		public static FieldType GetPrivateFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName)
		{
			return (FieldType) GetPrivateFieldInfo<ObjectType>(fieldName)?.GetValue(obj);
		}

		public static FieldType GetStaticFieldValue<ObjectType, FieldType>(string fieldName)
		{
			return (FieldType) GetStaticFieldInfo<ObjectType>(fieldName)?.GetValue(null);
		}

		public static FieldType GetStaticPrivateFieldValue<ObjectType, FieldType>(string fieldName)
		{
			return (FieldType) GetStaticPrivateFieldInfo<ObjectType>(fieldName)?.GetValue(null);
		}

		// PropertyValue

		public static PropertyType GetPropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName)
		{
			return (PropertyType) GetPropertyInfo<ObjectType>(propertyName)?.GetValue(obj);
		}

		public static PropertyType GetPrivatePropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName)
		{
			return (PropertyType) GetPrivatePropertyInfo<ObjectType>(propertyName)?.GetValue(obj);
		}

		public static PropertyType GetStaticPropertyValue<ObjectType, PropertyType>(string propertyName)
		{
			return (PropertyType) GetStaticPropertyInfo<ObjectType>(propertyName)?.GetValue(null);
		}

		public static PropertyType GetStaticPrivatePropertyValue<ObjectType, PropertyType>(string propertyName)
		{
			return (PropertyType) GetStaticPrivatePropertyInfo<ObjectType>(propertyName)?.GetValue(null);
		}

		/// Aliases with out parameters:

		// FieldInfo

		public static FieldInfo GetFieldInfo<ObjectType>(string fieldName, out FieldInfo fieldInfo)
		{
			return fieldInfo = GetFieldInfo<ObjectType>(fieldName);
		}

		public static FieldInfo GetPrivateFieldInfo<ObjectType>(string fieldName, out FieldInfo fieldInfo)
		{
			return fieldInfo = GetPrivateFieldInfo<ObjectType>(fieldName);
		}

		public static FieldInfo GetStaticFieldInfo<ObjectType>(string fieldName, out FieldInfo fieldInfo)
		{
			return fieldInfo = GetStaticFieldInfo<ObjectType>(fieldName);
		}

		public static FieldInfo GetStaticPrivateFieldInfo<ObjectType>(string fieldName, out FieldInfo fieldInfo)
		{
			return fieldInfo = GetStaticPrivateFieldInfo<ObjectType>(fieldName);
		}

		// PropertyInfo

		public static PropertyInfo GetPropertyInfo<ObjectType>(string propertyName, out PropertyInfo propertyInfo)
		{
			return propertyInfo = GetPropertyInfo<ObjectType>(propertyName);
		}

		public static PropertyInfo GetPrivatePropertyInfo<ObjectType>(string propertyName, out PropertyInfo propertyInfo)
		{
			return propertyInfo = GetPrivatePropertyInfo<ObjectType>(propertyName);
		}

		public static PropertyInfo GetStaticPropertyInfo<ObjectType>(string propertyName, out PropertyInfo propertyInfo)
		{
			return propertyInfo = GetStaticPropertyInfo<ObjectType>(propertyName);
		}

		public static PropertyInfo GetStaticPrivatePropertyInfo<ObjectType>(string propertyName, out PropertyInfo propertyInfo)
		{
			return propertyInfo = GetStaticPrivatePropertyInfo<ObjectType>(propertyName);
		}

		// MethodInfo

		public static MethodInfo GetMethodInfo<ObjectType>(string methodName, out MethodInfo methodInfo)
		{
			return methodInfo = GetMethodInfo<ObjectType>(methodName);
		}

		public static MethodInfo GetPrivateMethodInfo<ObjectType>(string methodName, out MethodInfo methodInfo)
		{
			return methodInfo = GetPrivateMethodInfo<ObjectType>(methodName);
		}

		public static MethodInfo GetStaticMethodInfo<ObjectType>(string methodName, out MethodInfo methodInfo)
		{
			return methodInfo = GetStaticMethodInfo<ObjectType>(methodName);
		}

		public static MethodInfo GetStaticPrivateMethodInfo<ObjectType>(string methodName, out MethodInfo methodInfo)
		{
			return methodInfo = GetStaticPrivateMethodInfo<ObjectType>(methodName);
		}

		// FieldValue

		public static FieldType GetFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			return fieldValue = GetFieldValue<ObjectType, FieldType>(obj, fieldName);
		}

		public static FieldType GetPrivateFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			return fieldValue = GetPrivateFieldValue<ObjectType, FieldType>(obj, fieldName);
		}

		public static FieldType GetStaticFieldValue<ObjectType, FieldType>(string fieldName, out FieldType fieldValue)
		{
			return fieldValue = GetStaticFieldValue<ObjectType, FieldType>(fieldName);
		}

		public static FieldType GetStaticPrivateFieldValue<ObjectType, FieldType>(string fieldName, out FieldType fieldValue)
		{
			return fieldValue = GetStaticPrivateFieldValue<ObjectType, FieldType>(fieldName);
		}

		// PropertyValue

		public static PropertyType GetPropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			return propertyValue = GetPropertyValue<ObjectType, PropertyType>(obj, propertyName);
		}

		public static PropertyType GetPrivatePropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			return propertyValue = GetPrivatePropertyValue<ObjectType, PropertyType>(obj, propertyName);
		}

		public static PropertyType GetStaticPropertyValue<ObjectType, PropertyType>(string propertyName, out PropertyType propertyValue)
		{
			return propertyValue = GetStaticPropertyValue<ObjectType, PropertyType>(propertyName);
		}

		public static PropertyType GetStaticPrivatePropertyValue<ObjectType, PropertyType>(string propertyName, out PropertyType propertyValue)
		{
			return propertyValue = GetStaticPrivatePropertyValue<ObjectType, PropertyType>(propertyName);
		}

		public static readonly BindingFlags PublicMemberFlags			= BindingFlags.Instance | BindingFlags.Public;
		public static readonly BindingFlags PrivateMemberFlags			= BindingFlags.Instance | BindingFlags.NonPublic;
		public static readonly BindingFlags StaticPublicMemberFlags		= BindingFlags.Static	| BindingFlags.Public;
		public static readonly BindingFlags StaticPrivateMemberFlags	= BindingFlags.Static	| BindingFlags.NonPublic;
	}
}
