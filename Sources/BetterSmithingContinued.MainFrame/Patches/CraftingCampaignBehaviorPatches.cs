using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using HarmonyLib;

using MCM.Abstractions.Base.Global;

using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(CraftingCampaignBehavior))]
	public class CraftingCampaignBehaviorPatches
	{
		[HarmonyPatch("CreateCraftedWeaponInFreeBuildMode")]
		[HarmonyPrefix]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void CreateCraftedWeaponPrefix(ItemModifier weaponModifier)
		{
			Instances.SmithingManager.LastSmithedWeaponQuality = (weaponModifier == null ? ItemQuality.Common : weaponModifier.ItemQuality);
		}

		[HarmonyPatch("CreateCraftedWeaponInFreeBuildMode")]
		[HarmonyPostfix]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void CreateCraftedWeaponPostfix(ref CraftingCampaignBehavior __instance, ref ItemObject __result, Hero hero, WeaponDesign weaponDesign, ItemModifier weaponModifier)
		{
			Instances.CraftingRepeater.AddWeaponTierType();
			ItemObject item = __result;
			MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
			if (instance != null && instance.GroupIdenticalCraftedWeapons)
			{
				ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
				item = ((itemRoster != null) ? itemRoster.CompressIdenticalCraftedWeapons(__result) : null);
			}
			Instances.SmithingManager.SmeltingItemRoster.ModifyItem(new EquipmentElement(item, null, null, false), 1);
			if (CraftingCampaignBehaviorPatches.m_IsCrafting)
			{
				return;
			}
			CraftingCampaignBehaviorPatches.m_IsCrafting = true;
			try
			{
				Instances.CraftingRepeater.DoMultiCrafting(ref __instance, hero, weaponDesign);
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
			}
			finally
			{
				Instances.SmithingManager.CraftingVM.SmartRefreshEnabledMainAction();
				CraftingCampaignBehaviorPatches.m_IsCrafting = false;
			}
		}

		[HarmonyPatch("DoRefinement")]
		[HarmonyPostfix]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void DoRefinementPostfix(ref CraftingCampaignBehavior __instance, Hero hero, Crafting.RefiningFormula refineFormula)
		{
			if (CraftingCampaignBehaviorPatches.m_IsRefining)
			{
				return;
			}
			CraftingCampaignBehaviorPatches.m_IsRefining = true;
			try
			{
				Instances.RefiningRepeater.DoRefinement(ref __instance, hero, refineFormula);
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
			}
			finally
			{
				Instances.SmithingManager.CraftingVM.SmartRefreshEnabledMainAction();
				CraftingCampaignBehaviorPatches.m_IsRefining = false;
			}
		}

		[HarmonyPatch("DoSmelting")]
		[HarmonyPostfix]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void DoSmeltingPostfix(ref CraftingCampaignBehavior __instance, Hero hero, EquipmentElement equipmentElement)
		{
			int num;
			int itemIndex = Instances.SmithingManager.SmeltingItemRoster.GetItemIndex(equipmentElement, out num);
			Instances.SmithingManager.SmeltingItemRoster.ModifyItem(equipmentElement, -1);
			if (CraftingCampaignBehaviorPatches.m_IsSmelting)
			{
				return;
			}
			try
			{
				CraftingCampaignBehaviorPatches.m_IsSmelting = true;
				Instances.SmeltingRepeater.DoSmelting(ref __instance, hero, equipmentElement, itemIndex);
			}
			catch (Exception)
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_EM_06}Issue occurred while attempting to smelt multiple.", null).ToString());
			}
			finally
			{
				Instances.SmithingManager.CraftingVM.SmartRefreshEnabledMainAction();
				CraftingCampaignBehaviorPatches.m_IsSmelting = false;
			}
		}

		[HarmonyPatch("GetHeroCraftingStamina")]
		[HarmonyPostfix]
		private static void GetHeroCraftingStaminaPatch(ref CraftingCampaignBehavior __instance, ref int __result, Hero hero)
		{
			MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
			if (instance != null && !instance.CraftingStaminaEnabled)
			{
				__result = __instance.GetMaxHeroCraftingStamina(hero);
			}
		}

		[HarmonyPatch("HourlyTick")]
		[HarmonyPostfix]
		private static void HourlyTickPatch(ref CraftingCampaignBehavior __instance)
		{
			MobileParty partyBelongedTo = Hero.MainHero.PartyBelongedTo;
			MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
			if (partyBelongedTo == null || instance == null || !instance.CraftingStaminaEnabled)
			{
				return;
			}
			float num = (partyBelongedTo.CurrentSettlement != null) ? instance.CraftingStaminaRecoveryRateInsideTowns : instance.CraftingStaminaRecoveryRateOutsideTowns;
			if (num == 0f)
			{
				return;
			}
			int num2 = partyBelongedTo.MemberRoster.TotalHeroes;
			foreach (TroopRosterElement troopRosterElement in partyBelongedTo.MemberRoster.GetTroopRoster())
			{
				CharacterObject character = troopRosterElement.Character;
				Hero hero = (character != null) ? character.HeroObject : null;
				if (hero != null)
				{
					int maxHeroCraftingStamina = __instance.GetMaxHeroCraftingStamina(hero);
					int num3 = __instance.GetHeroCraftingStamina(hero);
					if (num3 < maxHeroCraftingStamina)
					{
						num3 += CraftingCampaignBehaviorPatches.GetStaminaHourlyRecoveryRate(hero, num);
						if (num3 > maxHeroCraftingStamina)
						{
							num3 = maxHeroCraftingStamina;
						}
						__instance.SetHeroCraftingStamina(hero, num3);
					}
					num2--;
					if (num2 == 0)
					{
						break;
					}
				}
			}
		}

		[HarmonyPatch("GetStaminaHourlyRecoveryRate")]
		[HarmonyPostfix]
		private static void GetStaminaHourlyRecoveryRatePatch(Hero hero, ref int __result)
		{
			__result = 0;
		}

		private static int GetStaminaHourlyRecoveryRate(Hero hero, float multiplier = 1f)
		{
			int num = 5 + MathF.Round((float)hero.GetSkillValue(DefaultSkills.Crafting) * 0.025f);
			if (hero.GetPerkValue(DefaultPerks.Athletics.Stamina))
			{
				num += MathF.Round((float)num * DefaultPerks.Athletics.Stamina.PrimaryBonus);
			}
			return MathF.Round((float)num * multiplier);
		}

		private static bool m_IsSmelting;

		private static bool m_IsRefining;

		private static bool m_IsCrafting;
	}
}
