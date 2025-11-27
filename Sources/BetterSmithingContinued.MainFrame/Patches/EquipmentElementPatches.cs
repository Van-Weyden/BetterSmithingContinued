using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Localization;

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
                TextObject name = __instance.ItemModifier.Name.CopyTextObject();
                name.SetTextVariable("ITEMNAME", __instance.Item.Name);
                __result = name;
			}
		}
    }
}