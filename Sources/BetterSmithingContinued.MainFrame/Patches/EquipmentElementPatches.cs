using TaleWorlds.Core;
using TaleWorlds.Localization;

using HarmonyLib;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(EquipmentElement))]
	public class EquipmentElementPatches
	{
		[HarmonyPatch("GetModifiedItemName")]
		[HarmonyPostfix]
		private static void GetModifiedItemNamePostfix(ref EquipmentElement __instance, ref TextObject __result)
		{
			if (__instance.Item.IsCraftedByPlayer && __instance.ItemModifier != null)
			{
				__result = __instance.ItemModifier.Name;
				__result.SetTextVariable("ITEMNAME", __instance.Item.Name);
			}
		}
	}
}