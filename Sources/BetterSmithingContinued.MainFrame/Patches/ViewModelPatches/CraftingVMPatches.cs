using System;
using BetterSmithingContinued.MainFrame.Utilities;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(CraftingVM))]
	public class CraftingVMPatches
	{
		[HarmonyPatch(MethodType.Constructor, new Type[]
		{
			typeof(Crafting),
			typeof(Action),
			typeof(Action),
			typeof(Action),
			typeof(Func<WeaponComponentData, ItemObject.ItemUsageSetFlags>)
		})]
		[HarmonyPostfix]
		public static void ConstructorPostfix(ref CraftingVM __instance)
		{
			Instances.SmithingManager.CraftingVM = __instance;
		}

		[HarmonyPatch("ExecuteSwitchToCrafting")]
		[HarmonyPostfix]
		public static void ExecuteSwitchToCraftingPostfix()
		{
			Instances.ScreenSwitcher.UpdateCurrentCraftingSubVM(CraftingScreen.Crafting);
		}

		[HarmonyPatch("ExecuteSwitchToSmelting")]
		[HarmonyPostfix]
		public static void ExecuteSwitchToSmeltingPostfix()
		{
			Instances.ScreenSwitcher.UpdateCurrentCraftingSubVM(CraftingScreen.Smelting);
		}

		[HarmonyPatch("ExecuteSwitchToRefinement")]
		[HarmonyPostfix]
		public static void ExecuteSwitchToRefinementPostfix()
		{
			Instances.ScreenSwitcher.UpdateCurrentCraftingSubVM(CraftingScreen.Refining);
		}

		[HarmonyPatch("CurrentCraftingHero", MethodType.Setter)]
		[HarmonyPostfix]
		public static void CurrentCraftingHeroSetterPostfix(ref CraftingAvailableHeroItemVM ____currentCraftingHero)
		{
			Instances.SmithingManager.CurrentCraftingHero = ____currentCraftingHero;
		}
	}
}
