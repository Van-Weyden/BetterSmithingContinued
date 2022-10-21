using System;
using BetterSmithingContinued.MainFrame.Utilities;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(Crafting))]
	public class CraftingPatches
	{
		[HarmonyPatch("CraftedWeaponName", MethodType.Getter)]
		[HarmonyPostfix]
		private static void CraftedWeaponNameGetterPostfix(ref TextObject __result)
		{
			if (Instances.SmithingManager.ApplyNamePrefix && Instances.SmithingManager.LastSmithedWeaponTier != 0)
			{
				string value = WeaponTierUtils.GetWeaponTierPrefix(Instances.SmithingManager.LastSmithedWeaponTier) + " " + __result;
				__result = new TextObject(value, __result.Attributes);
			}
			Instances.SmithingManager.ApplyNamePrefix = false;
		}

		[HarmonyPatch("InitializePreCraftedWeaponOnLoad")]
		[HarmonyPostfix]
		private static void InitializePreCraftedWeaponOnLoadPostfix(ref ItemObject __result)
		{
			ItemObject.InitAsPlayerCraftedItem(ref __result);
		}
	}
}
