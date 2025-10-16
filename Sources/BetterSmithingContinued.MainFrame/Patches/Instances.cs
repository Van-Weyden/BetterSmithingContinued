using System;
using BetterSmithingContinued.MainFrame.UI;
using BetterSmithingContinued.Settings;

namespace BetterSmithingContinued.MainFrame.Patches
{
	public static class Instances
	{
		public static ISmithingManager SmithingManager { get; set; }

		public static ICraftingRepeater CraftingRepeater { get; set; }

		public static ISmeltingRepeater SmeltingRepeater { get; set; }

		public static IRefiningRepeater RefiningRepeater { get; set; }

		public static IScreenSwitcher ScreenSwitcher { get; set; }

		public static ISettingsManager SettingsManager { get; set; }

		public static IBetterSmithingUIContext BetterSmithingUIContext { get; set; }
	}
}
