using System;
using BetterSmithingContinued.Inputs.Code;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.HotKeys
{
	public class CycleHeroForwardKey : BetterSmithingContinued.Inputs.Code.HotKey
	{
		public CycleHeroForwardKey() : base("Better Smithing Continued: " + new TextObject("{=BSC_HKN_01}Cycle Hero Forwards", null), new TextObject("{=BSC_HKH_01}HotKey to cycle forward between different heroes in forge.", null).ToString(), "MenuShortcutCategory", InputKey.D)
		{
		}
	}
}
