using System;
using System.Collections.Generic;

using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Utilities
{
	public static class WeaponTierUtils
	{
		public static List<ItemQuality> GetWeaponTierOrderedList()
		{
			return new List<ItemQuality>
			{
				ItemQuality.Legendary,
				ItemQuality.Masterwork,
				ItemQuality.Fine,
				ItemQuality.Common,
				ItemQuality.Inferior,
				ItemQuality.Poor
			};
		}

		public static string GetWeaponTierPrefix(ItemQuality _weaponQuality, bool _includeNormal = false)
		{
			string result;
			switch (_weaponQuality)
			{
			case ItemQuality.Poor:
				result = new TextObject("{=BSC_WT_2}Rusty", null).ToString();
				break;
			case ItemQuality.Inferior:
				result = new TextObject("{=BSC_WT_3}Dull", null).ToString();
				break;
			case ItemQuality.Common:
				result = (_includeNormal ? new TextObject("{=BSC_WT_4}Normal", null).ToString() : "");
				break;
			case ItemQuality.Fine:
				result = new TextObject("{=BSC_WT_5}Balanced", null).ToString();
				break;
			case ItemQuality.Masterwork:
				result = new TextObject("{=BSC_WT_6}Masterwork", null).ToString();
				break;
			case ItemQuality.Legendary:
				result = new TextObject("{=BSC_WT_7}Legendary", null).ToString();
				break;
			default:
				throw new ArgumentOutOfRangeException("_weaponTier", _weaponQuality, null);
			}
			return result;
		}
	}
}
