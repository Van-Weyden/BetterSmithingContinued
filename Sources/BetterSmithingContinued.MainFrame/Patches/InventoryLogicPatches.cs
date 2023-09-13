using TaleWorlds.CampaignSystem.Inventory;
using TaleWorlds.CampaignSystem.Roster;

using HarmonyLib;

using MCM.Abstractions.Base.Global;

using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(InventoryLogic))]
	public class InventoryLogicPatches
	{
		[HarmonyPatch("InitializeRosters")]
		[HarmonyPostfix]
		private static void InitializeRostersPrefix(ItemRoster leftItemRoster, ItemRoster rightItemRoster)
		{
			if (GlobalSettings<MCMBetterSmithingSettings>.Instance?.GroupIdenticalCraftedWeapons ?? false)
			{
				leftItemRoster?.CompressIdenticalCraftedWeapons();
				rightItemRoster?.CompressIdenticalCraftedWeapons();
			}
		}
	}
}
