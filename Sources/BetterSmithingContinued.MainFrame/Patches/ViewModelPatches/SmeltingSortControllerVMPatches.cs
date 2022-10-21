using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(SmeltingSortControllerVM))]
	public class SmeltingSortControllerVMPatches
	{
		[HarmonyPatch("SortByCurrentState")]
		[HarmonyFinalizer]
		public static Exception SortByCurrentStateFinalizer(Exception __exception)
		{
			return null;
		}

		[HarmonyPatch("SortByCurrentState")]
		[HarmonyPrefix]
		public static bool SortByCurrentStatePrefix()
		{
			return true;
		}

		[HarmonyPatch("ExecuteSortByName")]
		[HarmonyFinalizer]
		public static Exception ExecuteSortByNameFinalizer(Exception __exception)
		{
			return null;
		}

		[HarmonyPatch("ExecuteSortByName")]
		[HarmonyPrefix]
		public static bool ExecuteSortByNamePrefix()
		{
			return true;
		}

		[HarmonyPatch("ExecuteSortByYield")]
		[HarmonyFinalizer]
		public static Exception ExecuteSortByYieldFinalizer(Exception __exception)
		{
			return null;
		}

		[HarmonyPatch("ExecuteSortByYield")]
		[HarmonyPrefix]
		public static bool ExecuteSortByYieldPrefix()
		{
			return true;
		}

		[HarmonyPatch("ExecuteSortByType")]
		[HarmonyFinalizer]
		public static Exception ExecuteSortByTypeFinalizer(Exception __exception)
		{
			return null;
		}

		[HarmonyPatch("ExecuteSortByType")]
		[HarmonyPrefix]
		public static bool ExecuteSortByTypePrefix()
		{
			return true;
		}
	}
}
