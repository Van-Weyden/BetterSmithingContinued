using System;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class ItemObjectExtensions
	{
		public static bool CompareTo(this ItemObject _this, ItemObject _other)
		{
			if (_this == null)
			{
				return _other == null;
			}
			return _other != null && !(_this.Name.ToString() != _other.Name.ToString()) && _this.ItemType == _other.ItemType && _this.Tier == _other.Tier && _this.WeaponDesign.CompareTo(_other.WeaponDesign) && _this.WeaponComponent.CompareTo(_other.WeaponComponent);
		}
	}
}
