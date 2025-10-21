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
			if (_this == null || _other == null)
			{
				return _this == _other;
			}

			return _this.HashedCode == _other.HashedCode && _this.WeaponName.Equals(_other.WeaponName);
		}

		public static Crafting GetCraftingComponent(this WeaponDesignVM _weaponDesignVM)
		{
			if (m_CraftingFieldInfo == null)
			{
				m_CraftingFieldInfo = MemberExtractor.GetPrivateFieldInfo<WeaponDesignVM>("_crafting");
			}
			return (Crafting) m_CraftingFieldInfo?.GetValue(_weaponDesignVM);
		}

        public static void SetWeaponName(this WeaponDesign _this, string name)
        {
            PropertyInfo info = MemberExtractor.GetPropertyInfo<WeaponDesign>("WeaponName");
            if (_this != null && info != null)
            {
                info.SetValue(_this, name);
                MemberExtractor.CallPrivateMethod(_this, "BuildHashedCode");
            }
        }

        private static FieldInfo m_CraftingFieldInfo;
	}
}
