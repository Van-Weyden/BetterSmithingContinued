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
			_harmony.Patch(
				MemberExtractor.GetMethodInfo<SmeltingVM>("RefreshList"), 
				new HarmonyMethod(MemberExtractor.GetStaticPrivateMethodInfo<SmeltingVMPatches>("RefreshListPrefix")),
				null, null, null
			);

			_harmony.Patch(
				MemberExtractor.GetPrivateMethodInfo<SmeltingVM>("SmeltSelectedItems"),
				new HarmonyMethod(MemberExtractor.GetStaticPrivateMethodInfo<SmeltingVMPatches>("SmeltSelectedItemsPrefix")),
				null, null, null
			);
		}

		private static bool RefreshListPrefix(ref SmeltingVM __instance, ref Action ____updateValuesOnSelectItemAction, ref Action ____updateValuesOnSmeltItemAction)
		{
			SmeltingVM smeltingVM = __instance;
			if (smeltingVM.SmeltableItemList == null)
			{
				smeltingVM.SmeltableItemList = new MBBindingList<SmeltingItemVM>();
			}
			____updateValuesOnSmeltItemAction?.Invoke();
			smeltingVM.SortController.SetListToControl(smeltingVM.SmeltableItemList);
			__instance.RefreshValues();
			____updateValuesOnSelectItemAction?.Invoke();
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
			____updateValuesOnSmeltItemAction?.Invoke();
			__instance.RefreshValues();
			return false;
		}
	}
}
