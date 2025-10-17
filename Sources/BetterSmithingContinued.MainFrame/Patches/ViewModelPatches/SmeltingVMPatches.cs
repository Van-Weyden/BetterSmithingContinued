using System;
using BetterSmithingContinued.Core;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(SmeltingVM))]
	public class SmeltingVMPatches : HarmonyCustomPatches
	{
		public new static void RegisterCustomPatches(Harmony _harmony)
		{
			_harmony.Patch(typeof(SmeltingVM).GetMethod("RefreshList", MemberExtractor.PublicMemberFlags), new HarmonyMethod(typeof(SmeltingVMPatches).GetMethod("RefreshListPrefix", MemberExtractor.StaticPrivateMemberFlags)), null, null, null);
			_harmony.Patch(typeof(SmeltingVM).GetMethod("SmeltSelectedItems", MemberExtractor.PrivateMemberFlags), new HarmonyMethod(typeof(SmeltingVMPatches).GetMethod("SmeltSelectedItemsPrefix", MemberExtractor.StaticPrivateMemberFlags)), null, null, null);
		}

		private static bool RefreshListPrefix(ref SmeltingVM __instance, ref Action ____updateValuesOnSelectItemAction, ref Action ____updateValuesOnSmeltItemAction)
		{
			SmeltingVM smeltingVM = __instance;
			if (smeltingVM.SmeltableItemList == null)
			{
				smeltingVM.SmeltableItemList = new MBBindingList<SmeltingItemVM>();
			}
			Action action = ____updateValuesOnSmeltItemAction;
			if (action != null)
			{
				action();
			}
			__instance.RefreshValues();
			Action action2 = ____updateValuesOnSelectItemAction;
			if (action2 != null)
			{
				action2();
			}
			return false;
		}

		private static bool SmeltSelectedItemsPrefix(ref SmeltingVM __instance, ref Hero currentCraftingHero, ref SmeltingItemVM ____currentSelectedItem, ref ICraftingCampaignBehavior ____smithingBehavior, ref Action ____updateValuesOnSmeltItemAction)
		{
			if (____currentSelectedItem != null)
			{
				ICraftingCampaignBehavior craftingCampaignBehavior = ____smithingBehavior;
				if (craftingCampaignBehavior != null)
				{
					craftingCampaignBehavior.DoSmelting(currentCraftingHero, ____currentSelectedItem.EquipmentElement);
				}
			}
			Action action = ____updateValuesOnSmeltItemAction;
			if (action != null)
			{
				action();
			}
			__instance.RefreshValues();
			return false;
		}
	}
}
