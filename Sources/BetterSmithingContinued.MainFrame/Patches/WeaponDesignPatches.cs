using TaleWorlds.Core;

using HarmonyLib;

using BetterSmithingContinued.Core;

namespace BetterSmithingContinued.MainFrame.Patches
{
    [HarmonyPatch(typeof(WeaponDesign))]
    public class WeaponDesignPatches
    {
        [HarmonyPatch("SetWeaponName")]
        [HarmonyPostfix]
        private static void SetWeaponNamePostfix(ref WeaponDesign __instance)
        {
            MemberExtractor.CallPrivateMethod(__instance, "BuildHashedCode");
        }
    }
}