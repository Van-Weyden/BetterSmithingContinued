using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingUtils
	{
		public static void SmartGenerateItem(WeaponDesign weaponDesign, string name, BasicCultureObject culture, ItemModifierGroup itemModifierGroup, ref ItemObject itemObject)
		{
			CraftingUtils.m_LazyGenerateItemInvoker.Value(weaponDesign, name, culture, itemModifierGroup, ref itemObject);
		}

		private static readonly Lazy<CraftingUtils.GenerateItem> m_LazyGenerateItemInvoker = new Lazy<CraftingUtils.GenerateItem>(delegate()
		{
			MethodInfo generateItemMethodInfo = MemberExtractor.GetStaticMethodInfo<Crafting>("GenerateItem");
			if (generateItemMethodInfo.GetParameters()[3].ParameterType == typeof(ItemModifierGroup))
			{
				return delegate(WeaponDesign _design, string _name, BasicCultureObject _culture, ItemModifierGroup _group, ref ItemObject _itemObject)
				{
					generateItemMethodInfo.Invoke(null, new object[]
					{
						_design,
						new TextObject("{=!}" + _name, null),
						_culture,
						_group,
						_itemObject
					});
				};
			}
			return delegate(WeaponDesign _design, string _name, BasicCultureObject _culture, ItemModifierGroup _, ref ItemObject _itemObject)
			{
				generateItemMethodInfo.Invoke(null, new object[]
				{
					_design,
					new TextObject("{=!}" + _name, null),
					_culture,
					_itemObject
				});
			};
		});

		private delegate void GenerateItem(WeaponDesign weaponDesign, string name, BasicCultureObject culture, ItemModifierGroup itemModifierGroup, ref ItemObject itemObject);
	}
}
