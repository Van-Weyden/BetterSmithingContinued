using System;

using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

using HarmonyLib;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(WeaponDesignResultPopupVM))]
	public class WeaponDesignResultPopupVMPatches
	{
		[HarmonyPatch(MethodType.Constructor, new Type[] {
			typeof(ItemObject),
			typeof(string),
			typeof(Action),
			typeof(Crafting),
			typeof(CraftingOrder),
			typeof(ItemCollectionElementViewModel),
			typeof(MBBindingList<ItemFlagVM>),
			typeof(Func<CraftingSecondaryUsageItemVM, MBBindingList<WeaponDesignResultPropertyItemVM>>),
			typeof(Action<CraftingSecondaryUsageItemVM>)
		})]
		[HarmonyPostfix]
		public static void ConstructorPostfix(ref WeaponDesignResultPopupVM __instance, string itemName)
		{
			__instance.ItemName = itemName;
		}
	}
}
