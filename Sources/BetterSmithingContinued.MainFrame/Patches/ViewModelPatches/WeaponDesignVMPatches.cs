using System;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using HarmonyLib;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.Utilities;
using BetterSmithingContinued.MainFrame.UI.ViewModels;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(WeaponDesignVM))]
	public class WeaponDesignVMPatches
	{
		[HarmonyPatch("CreateCraftingResultPopup")]
		[HarmonyPrefix]
		public static bool CreateCraftingResultPopupPrefix(ref WeaponDesignVM __instance)
        {
            if (__instance.IsInOrderMode)
            {
                return true;
            }

			if (Instances.SettingsManager.GetSettings<CraftingSettings>().SkipWeaponFinalizationPopup)
			{
                __instance.ExecuteFinalizeCrafting();
				return false;
			}

			return true;
		}

        [HarmonyPatch("ExecuteFinalizeCrafting")]
        [HarmonyPrefix]
        public static bool ExecuteFinalizeCrafting(ref WeaponDesignVM __instance)
        {
			if (__instance.IsInOrderMode)
			{
				return true;
			}

            ICraftingCampaignBehavior craftingBehavior = Campaign.Current.GetCampaignBehavior<ICraftingCampaignBehavior>();

            MemberExtractor.GetPrivateFieldValue(__instance, "_crafting", out Crafting crafting);
            bool skipWeaponFinalizationPopup = Instances.SettingsManager.GetSettings<CraftingSettings>().SkipWeaponFinalizationPopup;
			if (skipWeaponFinalizationPopup)
            {
                TextObject weaponName = new TextObject("{=!}" + __instance.ItemName, null);
                crafting.SetCraftedWeaponName(weaponName);
                craftingBehavior.SetCraftedWeaponName(__instance.CraftedItemObject, weaponName);
            }
			else
			{
                if (Instances.ScreenSwitcher.ConnectedViewModel(Utilities.CraftingScreen.Crafting) is BetterCraftingVM craftingVM)
                {
					craftingVM.WeaponName = crafting.CraftedWeaponName.ToString();
                }
            }

			if (!CraftingCampaignBehaviorPatches.IsCrafting)
			{
				CraftingCampaignBehaviorPatches.IsCrafting = true;
				try
				{
					Instances.CraftingRepeater.DoMultiCrafting(craftingBehavior);
				}
				catch (Exception value)
				{
					Core.Logger.Add("ExecuteFinalizeCrafting ERROR: " + value.Message);
				}
				finally
				{
					Instances.SmithingManager.CraftingVM.SmartRefreshEnabledMainAction();
				}
				CraftingCampaignBehaviorPatches.IsCrafting = false;
			}

            return true;
		}

        [HarmonyPatch(typeof(WeaponDesignVM))]
		[HarmonyPatch("RefreshStats")]
		[HarmonyPostfix]
		private static void RefreshStatsPostfix(WeaponDesignVM __instance)
		{
			Crafting crafting;
			MemberExtractor.GetPrivateFieldValue(__instance, "_crafting", out crafting);
			ItemObject currentCraftedItemObject = crafting.GetCurrentCraftedItemObject(false);
			EquipmentElement itemRosterElement = new EquipmentElement(currentCraftedItemObject, null, null, false);
			int price = Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(itemRosterElement, Campaign.Current.MainParty, null, true, 0f, 0f, 0f);
			CraftingListPropertyItem craftingListPropertyItem = new CraftingListPropertyItem(new TextObject("{=BSC_UI_Value}Value: ", null), 50000f, (float)price, 0f, CraftingTemplate.CraftingStatTypes.NumStatTypes, false);
			craftingListPropertyItem.IsValidForUsage = true;
            MemberExtractor.GetPrivateFieldValue(__instance, "_primaryPropertyList", out MBBindingList<CraftingListPropertyItem> mbbindingList);
            mbbindingList.Add(craftingListPropertyItem);
		}

		[HarmonyPatch(typeof(WeaponDesignVM))]
		[HarmonyPatch("GetResultPropertyList")]
		[HarmonyPostfix]
		private static void GetResultPropertyListPostfix(ref MBBindingList<WeaponDesignResultPropertyItemVM> __result)
		{
			__result.RemoveAt(__result.Count - 1);
		}
	}
}
