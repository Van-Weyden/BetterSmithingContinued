using System;

using TaleWorlds.CampaignSystem.CraftingSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

using HarmonyLib;
using BetterSmithingContinued.Core;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(WeaponDesignResultPopupVM))]
	public class WeaponDesignResultPopupVMPatches
	{
		[HarmonyPatch(MethodType.Constructor, new Type[] {
			typeof(Action),
			typeof(Crafting),
			typeof(CraftingOrder),
			typeof(MBBindingList<ItemFlagVM>),
			typeof(ItemObject),
			typeof(MBBindingList<WeaponDesignResultPropertyItemVM>),
			typeof(string),
			typeof(ItemCollectionElementViewModel)
		})]
		[HarmonyPostfix]
		public static void ConstructorPostfix(ref WeaponDesignResultPopupVM __instance, string itemName)
		{
			__instance.ItemName = itemName;
		}
	}
}
