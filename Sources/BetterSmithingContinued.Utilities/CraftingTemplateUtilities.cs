using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingTemplateUtilities
	{
		public static CraftingTemplate[] GetAll()
		{
			return CraftingTemplateUtilities.m_LazyCraftingTemplateGetAll.Value();
		}

		private static readonly Lazy<Func<CraftingTemplate[]>> m_LazyCraftingTemplateGetAll = new Lazy<Func<CraftingTemplate[]>>(delegate()
		{
			PropertyInfo property = MemberExtractor.GetStaticPropertyInfo<CraftingTemplate>("All");
			if (property?.PropertyType == typeof(IEnumerable<CraftingTemplate>))
			{
				Func<IEnumerable<CraftingTemplate>> getAll = (Func<IEnumerable<CraftingTemplate>>)property.GetMethod.CreateDelegate(typeof(Func<IEnumerable<CraftingTemplate>>));
				return () => getAll().ToArray<CraftingTemplate>();
			}
			if (property?.PropertyType == typeof(MBReadOnlyList<CraftingTemplate>))
			{
				Func<MBReadOnlyList<CraftingTemplate>> getAll = (Func<MBReadOnlyList<CraftingTemplate>>)property.GetMethod.CreateDelegate(typeof(Func<MBReadOnlyList<CraftingTemplate>>));
				return () => getAll().ToArray<CraftingTemplate>();
			}
			if (property == null)
			{
				throw new NullReferenceException("[BetterSmithingContinued] Could not find the [All] Property in [CraftingTemplate].");
			}
			throw new InvalidCastException("[BetterSmithingContinued] The type of the static [All] Property of [CraftingTemplate] has changed and is now of type: " + property.PropertyType.ToString());
		});
	}
}
