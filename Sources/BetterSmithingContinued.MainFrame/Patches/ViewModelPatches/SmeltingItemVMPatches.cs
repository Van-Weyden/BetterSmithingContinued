using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;

using HarmonyLib;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(SmeltingItemVM))]
	public class SmeltingItemVMPatches
	{
		[HarmonyPatch("RefreshValues")]
		[HarmonyPostfix]
		public static void RefreshValuesPostfix(ref SmeltingItemVM __instance)
		{
			__instance.Name = __instance.EquipmentElement.GetModifiedItemName().ToString();
		}
	}
}
