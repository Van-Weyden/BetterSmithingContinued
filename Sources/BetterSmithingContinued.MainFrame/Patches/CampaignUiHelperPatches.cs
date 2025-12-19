using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Patches
{
    [HarmonyPatch(typeof(CampaignUIHelper))]
    public class CampaignUiHelperPatches
    {
        [HarmonyPatch("GetItemFlagDetails")]
        [HarmonyPostfix]
        private static void GetItemFlagDetailsPostfix(ItemFlags itemFlags, ref List<Tuple<string, TextObject>> __result)
        {
            if (itemFlags.HasAnyFlag(ItemFlags.Stealth))
            {
                string text = "GeneralFlagIcons\\stealth";
                foreach (Tuple<string, TextObject> turple in __result)
                {
                    if (turple.Item1 == text)
                    {
                        return;
                    }
                }

                TextObject textObject = GameTexts.FindText("str_inventory_flag_stealth", null);
                __result.Add(new Tuple<string, TextObject>(text, textObject));
            }
        }
    }
}
