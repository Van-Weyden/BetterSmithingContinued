using System;
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
			if (WeaponDesignExtensions.m_CraftingFieldInfo == null)
			{
				WeaponDesignExtensions.m_CraftingFieldInfo = typeof(WeaponDesignVM).GetField("_crafting", MemberExtractor.PrivateMemberFlags);
			}
			FieldInfo craftingFieldInfo = WeaponDesignExtensions.m_CraftingFieldInfo;
			return (Crafting)((craftingFieldInfo != null) ? craftingFieldInfo.GetValue(_weaponDesignVM) : null);
		}

		private static FieldInfo m_CraftingFieldInfo;
	}
}
