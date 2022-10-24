using System;
using HarmonyLib;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(CraftingGauntletScreen))]
	public class CraftingGauntletScreenPatches
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPostfix]
		public static void InitializePostfix(ref CraftingGauntletScreen __instance)
		{
			Instances.ScreenSwitcher.CraftingGauntletScreen = __instance;
		}

		[HarmonyPatch("OnFinalize")]
		[HarmonyPostfix]
		public static void OnFinalizePostfix()
		{
			Instances.SmithingManager.CraftingVM = null;
			Instances.ScreenSwitcher.CraftingGauntletScreen = null;
		}
	}
}
