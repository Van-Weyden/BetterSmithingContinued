using System;
using System.Collections.Generic;
using MCM.Abstractions.Dropdown;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.Settings
{
	public static class KeybindingDropdown
	{
		public static DropdownDefault<KeybindingDropdownOption> GetKeybindingDropdownOptions(InputKey _defaultKey)
		{
			List<KeybindingDropdownOption> list = new List<KeybindingDropdownOption>
			{
				new KeybindingDropdownOption(InputKey.LeftControl, new TextObject("{=BSC_HKN_Left}Left", null).ToString() + " Ctrl"),
				new KeybindingDropdownOption(InputKey.LeftShift, new TextObject("{=BSC_HKN_Left}Left", null).ToString() + " Shift"),
				new KeybindingDropdownOption(InputKey.LeftAlt, new TextObject("{=BSC_HKN_Left}Left", null).ToString() + " Alt"),
				new KeybindingDropdownOption(InputKey.Tab, "Tab"),
				new KeybindingDropdownOption(InputKey.A, "A"),
				new KeybindingDropdownOption(InputKey.D, "D"),
				new KeybindingDropdownOption(InputKey.Left, new TextObject("{=BSC_HKN_LA}Left Arrow", null).ToString()),
				new KeybindingDropdownOption(InputKey.Right, new TextObject("{=BSC_HKN_RA}Right Arrow", null).ToString())
			};
			return new DropdownDefault<KeybindingDropdownOption>(list.ToArray(), list.FindIndex((KeybindingDropdownOption option) => option.Key == _defaultKey));
		}
	}
}
