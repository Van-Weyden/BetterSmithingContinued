using System;
using BetterSmithingContinued.Inputs.Code;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.HotKeys
{
	public class CycleHeroBackwardKey : BetterSmithingContinued.Inputs.Code.HotKey
	{
		public CycleHeroBackwardKey() : base("Better Smithing Continued: " + new TextObject("{=BSC_HKN_02}Cycle Hero Backwards", null), new TextObject("{=BSC_HKH_02}HotKey to cycle backward between different heroes in forge.", null).ToString(), "MenuShortcutCategory", InputKey.A)
		{
		}
	}
}
