using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingUtils
	{
		public static void SmartGenerateItem(
			WeaponDesign weaponDesign,
			TextObject name,
			BasicCultureObject culture,
			ItemModifierGroup itemModifierGroup,
			ref ItemObject itemObject,
			string customId
		)
		{
			CraftingUtils.m_LazyGenerateItemInvoker.Value(weaponDesign, name, culture, itemModifierGroup, ref itemObject, customId);
		}

		private static readonly Lazy<CraftingUtils.GenerateItem> m_LazyGenerateItemInvoker = new Lazy<CraftingUtils.GenerateItem>(delegate() {
			MethodInfo generateItemMethodInfo = MemberExtractor.GetStaticMethodInfo<Crafting>("GenerateItem");
				return delegate(
					WeaponDesign _design,
					TextObject _name,
					BasicCultureObject _culture,
					ItemModifierGroup _group,
					ref ItemObject _itemObject,
					string customId
				) {
					generateItemMethodInfo.Invoke(null, new object[] {
						_design,
						_name,
						_culture,
						_group,
						_itemObject,
						customId
					}
				);
            };
		});

		private delegate void GenerateItem(
			WeaponDesign weaponDesign,
			TextObject name,
			BasicCultureObject culture,
			ItemModifierGroup itemModifierGroup,
			ref ItemObject itemObject,
			string customId
		);
	}
}
