using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Utilities
{
	public static class WeaponTierUtils
	{
		public static List<WeaponTier> GetWeaponTierOrderedList()
		{
			return new List<WeaponTier>
			{
				WeaponTier.Legendary,
				WeaponTier.Masterwork,
				WeaponTier.Fine,
				WeaponTier.Normal,
				WeaponTier.Crude,
				WeaponTier.Rusty,
				WeaponTier.Broken
			};
		}

		public static string GetWeaponTierPrefix(int _weaponTier)
		{
			return WeaponTierUtils.GetWeaponTierPrefix(WeaponTierUtils.GetWeaponTier(_weaponTier), false);
		}

		public static string GetWeaponTierPrefix(WeaponTier _weaponTier, bool _includeNormal = false)
		{
			string result;
			switch (_weaponTier)
			{
			case WeaponTier.Broken:
				result = new TextObject("{=BSC_WT_1}Broken", null).ToString();
				break;
			case WeaponTier.Rusty:
				result = new TextObject("{=BSC_WT_2}Rusty", null).ToString();
				break;
			case WeaponTier.Crude:
				result = new TextObject("{=BSC_WT_3}Crude", null).ToString();
				break;
			case WeaponTier.Normal:
				result = (_includeNormal ? new TextObject("{=BSC_WT_4}Normal", null).ToString() : "");
				break;
			case WeaponTier.Fine:
				result = new TextObject("{=BSC_WT_5}Fine", null).ToString();
				break;
			case WeaponTier.Masterwork:
				result = new TextObject("{=BSC_WT_6}Masterwork", null).ToString();
				break;
			case WeaponTier.Legendary:
				result = new TextObject("{=BSC_WT_7}Legendary", null).ToString();
				break;
			default:
				throw new ArgumentOutOfRangeException("_weaponTier", _weaponTier, null);
			}
			return result;
		}

		public static WeaponTier GetWeaponTier(int _weaponTier)
		{
			if (_weaponTier <= -3)
			{
				return WeaponTier.Broken;
			}
			WeaponTier result;
			switch (_weaponTier)
			{
			case -2:
				result = WeaponTier.Rusty;
				break;
			case -1:
				result = WeaponTier.Crude;
				break;
			case 0:
				result = WeaponTier.Normal;
				break;
			case 1:
				result = WeaponTier.Fine;
				break;
			case 2:
				result = WeaponTier.Masterwork;
				break;
			default:
				result = WeaponTier.Legendary;
				break;
			}
			return result;
		}
	}
}
