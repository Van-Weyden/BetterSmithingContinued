using TaleWorlds.Core;

using HarmonyLib;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(Crafting))]
	public class CraftingPatches
	{
		[HarmonyPatch("InitializePreCraftedWeaponOnLoad")]
		[HarmonyPostfix]
		private static void InitializePreCraftedWeaponOnLoadPostfix(ref ItemObject __result)
		{
			ItemObject.InitAsPlayerCraftedItem(ref __result);
		}
	}
}
