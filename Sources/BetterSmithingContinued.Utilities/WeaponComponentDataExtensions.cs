using System;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class WeaponComponentDataExtensions
	{
		public static bool CompareTo(this WeaponComponentData _this, WeaponComponentData _other)
		{
			if (_this == null || _other == null)
			{
				return _this == _other;
			}

			return !(_this.PhysicsMaterial != _other.PhysicsMaterial)
				&& !(_this.ItemUsage != _other.ItemUsage)
				&& _this.ThrustSpeed == _other.ThrustSpeed
				&& _this.SwingSpeed == _other.SwingSpeed
				&& _this.MissileSpeed == _other.MissileSpeed
				&& _this.WeaponLength == _other.WeaponLength
				&& _this.ThrustDamage == _other.ThrustDamage
				&& _this.ThrustDamageType == _other.ThrustDamageType
				&& _this.SwingDamage == _other.SwingDamage
				&& _this.SwingDamageType == _other.SwingDamageType
				&& _this.Accuracy == _other.Accuracy
				&& _this.WeaponClass == _other.WeaponClass
				&& _this.AmmoClass == _other.AmmoClass
				&& _this.MissileDamage == _other.MissileDamage
				&& _this.Handling == _other.Handling
				&& _this.MissileDamage == _other.MissileDamage;
		}
	}
}
