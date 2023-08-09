using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class WeaponDesignExtensions
	{
		public static bool CompareTo(this WeaponDesign _this, WeaponDesign _other)
		{
			return _this == _other;
		}

		public static Crafting GetCraftingComponent(this WeaponDesignVM _weaponDesignVM)
		{
			if (m_CraftingFieldInfo == null)
			{
				m_CraftingFieldInfo = MemberExtractor.GetPrivateFieldInfo<WeaponDesignVM>("_crafting");
			}
			return (Crafting) m_CraftingFieldInfo?.GetValue(_weaponDesignVM);
		}

		private static FieldInfo m_CraftingFieldInfo;
	}
}
