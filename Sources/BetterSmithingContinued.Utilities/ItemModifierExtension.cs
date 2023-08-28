using TaleWorlds.Core;

namespace BetterSmithingContinued.Utilities
{
	public static class ItemModifierExtension
	{
		public static bool CompareTo(this ItemModifier _this, ItemModifier _other)
		{
			if (_this == null || _other == null)
			{
				return _this == _other;
			}
			return _this.Equals(_other);
		}
	}
}
