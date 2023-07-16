using System;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.MainFrame.Persistence;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(WeaponDesignVM))]
	public class WeaponDesignVMPatches
	{
		[HarmonyPatch("CreateCraftingResultPopup")]
		[HarmonyPrefix]
		public static bool CreateCraftingResultPopupPrefix(ref WeaponDesignVM __instance)
		{
			bool skipWeaponFinalizationPopup = Instances.SettingsManager.GetSettings<CraftingSettings>().SkipWeaponFinalizationPopup;
			if (__instance.ActiveCraftingOrder == null && skipWeaponFinalizationPopup)
			{
				__instance.ExecuteFinalizeCrafting();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(WeaponDesignVM))]
		[HarmonyPatch("RefreshStats")]
		[HarmonyPostfix]
		private static void RefreshStatsPostfix(WeaponDesignVM __instance)
		{
			Crafting crafting;
			MemberExtractor.GetPrivateFieldValue<WeaponDesignVM, Crafting>(__instance, "_crafting", out crafting);
			ItemObject currentCraftedItemObject = crafting.GetCurrentCraftedItemObject(false);
			EquipmentElement itemRosterElement = new EquipmentElement(currentCraftedItemObject, null, null, false);
			int price = Campaign.Current.Models.TradeItemPriceFactorModel.GetPrice(itemRosterElement, Campaign.Current.MainParty, null, true, 0f, 0f, 0f);
			CraftingListPropertyItem craftingListPropertyItem = new CraftingListPropertyItem(new TextObject("Value: ", null), 50000f, (float)price, 0f, CraftingTemplate.CraftingStatTypes.NumStatTypes, false);
			craftingListPropertyItem.IsValidForUsage = true;
			MBBindingList<CraftingListPropertyItem> mbbindingList;
			MemberExtractor.GetPrivateFieldValue<WeaponDesignVM, MBBindingList<CraftingListPropertyItem>>(__instance, "_primaryPropertyList", out mbbindingList);
			mbbindingList.Add(craftingListPropertyItem);
		}

		[HarmonyPatch(typeof(WeaponDesignVM))]
		[HarmonyPatch("UpdateResultPropertyList")]
		[HarmonyPostfix]
		private static void UpdateResultPropertyListPostfix(WeaponDesignVM __instance)
		{
			MBBindingList<WeaponDesignResultPropertyItemVM> mbbindingList;
			MemberExtractor.GetPrivateFieldValue<WeaponDesignVM, MBBindingList<WeaponDesignResultPropertyItemVM>>(__instance, "_designResultPropertyList", out mbbindingList);
			mbbindingList.RemoveAt(mbbindingList.Count - 1);
		}
	}
}
