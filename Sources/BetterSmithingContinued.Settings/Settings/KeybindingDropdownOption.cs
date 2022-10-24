using System;
using TaleWorlds.InputSystem;

namespace BetterSmithingContinued.Settings
{
	public class KeybindingDropdownOption
	{
		public InputKey Key { get; }

		public string DisplayString { get; }

		public KeybindingDropdownOption(InputKey _key, string _displayString)
		{
			this.Key = _key;
			this.DisplayString = _displayString;
		}

		public override string ToString()
		{
			return this.DisplayString;
		}
	}
}
