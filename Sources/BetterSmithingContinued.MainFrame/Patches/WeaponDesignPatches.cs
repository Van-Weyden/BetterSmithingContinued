using BetterSmithingContinued.Core;
using HarmonyLib;
using System;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Patches
{
    [HarmonyPatch(typeof(WeaponDesign))]
    public class WeaponDesignPatches
    {
        [HarmonyPatch(MethodType.Constructor, new Type[]
        {
            typeof(CraftingTemplate),
            typeof(TextObject),
            typeof(WeaponDesignElement[]),
            typeof(string)
        })]
        [HarmonyPostfix]
        public static void ConstructorPostfix(ref WeaponDesign __instance)
        {
            if (string.IsNullOrEmpty(__instance.HashedCode) || __instance.HashedCode.StartsWith("crafted_item_"))
            {
                MemberExtractor.SetPrivatePropertyValue(__instance, "HashedCode", WeaponDesignHash(__instance));
            }
        }

        [HarmonyPatch("HashedCode", MethodType.Setter)]
        [HarmonyPrefix]
        public static bool HashedCodeSetterPrefix(ref WeaponDesign __instance, ref string __0)
        {
            if (string.IsNullOrEmpty(__0) || __0.StartsWith("crafted_item_"))
            {
                __0 = WeaponDesignHash(__instance);
            }

            return true;
        }

        [HarmonyPatch("SetWeaponName")]
        [HarmonyPostfix]
        public static void SetWeaponNamePostfix(ref WeaponDesign __instance)
        {
            MemberExtractor.SetPrivatePropertyValue(__instance, "HashedCode", WeaponDesignHash(__instance));
        }

        private static string WeaponDesignHash(WeaponDesign weaponDesign)
        {
            string hash = "";
            foreach (WeaponDesignElement weaponDesignElement in weaponDesign.UsedPieces)
            {
                if (weaponDesignElement.IsValid)
                {
                    hash = string.Concat(new object[] {
                        hash,
                        weaponDesignElement.CraftingPiece.StringId,
                        ";",
                        weaponDesignElement.ScalePercentage,
                        ";"
                    });
                }
                else
                {
                    hash += "invalid_piece;";
                }
            }
            hash += weaponDesign.Template.StringId;
            hash += weaponDesign.WeaponName;

            return hash;
        }
    }
}
