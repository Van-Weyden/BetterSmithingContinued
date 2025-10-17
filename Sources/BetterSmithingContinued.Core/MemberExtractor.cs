using System;
using System.Reflection;

namespace BetterSmithingContinued.Core
{
	public class MemberExtractor
	{
		public static FieldType GetFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			FieldInfo field = typeof(ObjectType).GetField(fieldName, MemberExtractor.PublicMemberFlags);
			return fieldValue = (FieldType)((object)((field != null) ? field.GetValue(obj) : null));
		}

		public static FieldType GetPrivateFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			FieldInfo field = typeof(ObjectType).GetField(fieldName, MemberExtractor.PrivateMemberFlags);
			return fieldValue = (FieldType)((object)((field != null) ? field.GetValue(obj) : null));
		}

		public static FieldType GetStaticFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			FieldInfo field = typeof(ObjectType).GetField(fieldName, MemberExtractor.StaticPublicMemberFlags);
			return fieldValue = (FieldType)((object)((field != null) ? field.GetValue(obj) : null));
		}

		public static FieldType GetStaticPrivateFieldValue<ObjectType, FieldType>(ObjectType obj, string fieldName, out FieldType fieldValue)
		{
			FieldInfo field = typeof(ObjectType).GetField(fieldName, MemberExtractor.StaticPrivateMemberFlags);
			return fieldValue = (FieldType)((object)((field != null) ? field.GetValue(obj) : null));
		}

		public static PropertyType GetPropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			PropertyInfo property = typeof(ObjectType).GetProperty(propertyName, MemberExtractor.PublicMemberFlags);
			return propertyValue = (PropertyType)((object)((property != null) ? property.GetValue(obj) : null));
		}

		public static PropertyType GetPrivatePropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			PropertyInfo property = typeof(ObjectType).GetProperty(propertyName, MemberExtractor.PrivateMemberFlags);
			return propertyValue = (PropertyType)((object)((property != null) ? property.GetValue(obj) : null));
		}

		public static PropertyType GetStaticPropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			PropertyInfo property = typeof(ObjectType).GetProperty(propertyName, MemberExtractor.StaticPublicMemberFlags);
			return propertyValue = (PropertyType)((object)((property != null) ? property.GetValue(obj) : null));
		}

		public static PropertyType GetStaticPrivatePropertyValue<ObjectType, PropertyType>(ObjectType obj, string propertyName, out PropertyType propertyValue)
		{
			PropertyInfo property = typeof(ObjectType).GetProperty(propertyName, MemberExtractor.StaticPrivateMemberFlags);
			return propertyValue = (PropertyType)((object)((property != null) ? property.GetValue(obj) : null));
		}

		public static MethodInfo GetMethodInfo<ObjectType>(ObjectType obj, string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, MemberExtractor.PublicMemberFlags);
		}

		public static MethodInfo GetPrivateMethodInfo<ObjectType>(ObjectType obj, string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, MemberExtractor.PrivateMemberFlags);
		}

		public static MethodInfo GetStaticMethodInfo<ObjectType>(ObjectType obj, string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, MemberExtractor.StaticPublicMemberFlags);
		}

		public static MethodInfo GetStaticPrivateMethodInfo<ObjectType>(ObjectType obj, string methodName)
		{
			return typeof(ObjectType).GetMethod(methodName, MemberExtractor.StaticPrivateMemberFlags);
		}

		public static readonly BindingFlags PublicMemberFlags = BindingFlags.Instance | BindingFlags.Public;

		public static readonly BindingFlags PrivateMemberFlags = BindingFlags.Instance | BindingFlags.NonPublic;

		public static readonly BindingFlags StaticPublicMemberFlags = BindingFlags.Static | BindingFlags.Public;

		public static readonly BindingFlags StaticPrivateMemberFlags = BindingFlags.Static | BindingFlags.NonPublic;
	}
}
