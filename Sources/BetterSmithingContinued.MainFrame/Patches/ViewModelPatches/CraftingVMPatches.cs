using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core;
using static BetterSmithingContinued.Settings.CharacterCycleDropdownOption;

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
			if (GlobalSettings<MCMBetterSmithingSettings>.Instance.ReorderCharactersInMenu)
			{
                switch (GlobalSettings<MCMBetterSmithingSettings>.Instance.CharacterCycleType.SelectedValue.Type)
                {
                    case OrderType.SkillAsc:
                        __instance?.AvailableCharactersForSmithing?.Sort(Comparer<CraftingAvailableHeroItemVM>.Create(
							(hero1, hero2) => hero1.SmithySkillLevel.CompareTo(hero2.SmithySkillLevel)
						));
                        break;

                    case OrderType.SkillDesc:
                        __instance?.AvailableCharactersForSmithing?.Sort(Comparer<CraftingAvailableHeroItemVM>.Create(
							(hero1, hero2) => hero2.SmithySkillLevel.CompareTo(hero1.SmithySkillLevel)
						));
                        break;

                    default:
                        break;
                }
            }   
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
