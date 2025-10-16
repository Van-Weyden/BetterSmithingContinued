using System;
using HarmonyLib;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(GauntletCraftingScreen))]
	public class GauntletCraftingScreenPatches
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPostfix]
		public static void InitializePostfix(ref GauntletCraftingScreen __instance)
		{
			Instances.ScreenSwitcher.GauntletCraftingScreen = __instance;
		}

		[HarmonyPatch("OnFinalize")]
		[HarmonyPostfix]
		public static void OnFinalizePostfix()
		{
			Instances.SmithingManager.CraftingVM = null;
			Instances.ScreenSwitcher.GauntletCraftingScreen = null;
		}
	}
}
