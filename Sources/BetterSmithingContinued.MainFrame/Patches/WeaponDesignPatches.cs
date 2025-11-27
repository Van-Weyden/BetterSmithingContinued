using TaleWorlds.Core;
using TaleWorlds.Localization;

using HarmonyLib;

using BetterSmithingContinued.Core;

namespace BetterSmithingContinued.MainFrame.Patches
{
    [HarmonyPatch(typeof(WeaponDesign))]
    public class WeaponDesignPatches
    {
        [HarmonyPatch("HashedCode", MethodType.Setter)]
        [HarmonyPrefix]
        public static bool HashedCodeSetterPrefix(ref WeaponDesign __instance, ref string __0)
        {
            if (__0.StartsWith("crafted_item_"))
            {
                __0 = WeaponDesignHash(__instance);
            }

            return true;
        }

        [HarmonyPatch("SetWeaponName")]
        [HarmonyPrefix]
        public static void SetWeaponNamePrefix(ref WeaponDesign __instance, TextObject name)
        {
            if (!__instance.WeaponName.Equals(name))
            {
                MemberExtractor.SetPrivatePropertyValue(__instance, "HashedCode", WeaponDesignHash(__instance));
            }
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
