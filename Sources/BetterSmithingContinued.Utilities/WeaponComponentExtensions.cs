using System;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class WeaponComponentExtensions
	{
		public static bool CompareTo(this WeaponComponent _this, WeaponComponent _other)
		{
			if (_this == null)
			{
				return _other == null;
			}
			if (_other == null)
			{
				return false;
			}
			if (_this.Weapons.Count == _other.Weapons.Count)
			{
				for (int i = 0; i < _this.Weapons.Count; i++)
				{
					if (!_this.Weapons[i].CompareTo(_other.Weapons[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}
}
