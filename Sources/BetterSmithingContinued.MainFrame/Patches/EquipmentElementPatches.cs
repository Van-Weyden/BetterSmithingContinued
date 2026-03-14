using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
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
                if (!GlobalSettings<MCMBetterSmithingSettings>.Instance?.AddWeaponTierPrefixes ?? true)
                {
                    return;
                }

                TextObject name = null;
                bool useOwnPrefix = GlobalSettings<MCMBetterSmithingSettings>.Instance?.UseOwnPrefixesOnly ?? false;

                if (!useOwnPrefix)
                {
                    name = __instance.ItemModifier.Name.CopyTextObject();
                    name.SetTextVariable("ITEMNAME", __instance.Item.Name);

                    // Native translation may use specific formulas for translations, so use own if the native prefix is ​​not applied
                    useOwnPrefix = (name.ToString().Trim() == __instance.Item.Name.ToString().Trim());
                }

                if (useOwnPrefix)
                {
                    name = WeaponTierUtils.GetWeaponTextObject(__instance.ItemModifier.ItemQuality);
                    name.SetTextVariable("ITEMNAME", __instance.Item.Name);
                }

                __result = name;
			}
        }
    }
}