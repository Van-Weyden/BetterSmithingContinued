using System;
using System.Linq;
using BetterSmithingContinued.Utilities;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame.Patches.ViewModelPatches
{
	[HarmonyPatch(typeof(RefinementVM))]
	public class RefinementVMPatches
	{
		[HarmonyPatch("OnCraftingHeroChanged")]
		[HarmonyPrefix]
		public static void OnCraftingHeroChangedPrefix(ref RefinementVM __instance)
		{
			RefinementActionItemVM currentSelectedAction = __instance.CurrentSelectedAction;
			Crafting.RefiningFormula refiningFormula = (currentSelectedAction != null) ? currentSelectedAction.RefineFormula : null;
			if (refiningFormula != null)
			{
				RefinementVMPatches.m_PreviousOutputType = refiningFormula.Output;
				RefinementVMPatches.m_PreviousOutputCount = refiningFormula.OutputCount;
				RefinementVMPatches.m_PreviousActionIndex = __instance.AvailableRefinementActions.IndexOf(__instance.CurrentSelectedAction);
				return;
			}
			RefinementVMPatches.m_PreviousOutputCount = -1;
		}

		[HarmonyPatch("OnCraftingHeroChanged")]
		[HarmonyPostfix]
		public static void OnCraftingHeroChangedPostfix(ref RefinementVM __instance)
		{
			if (RefinementVMPatches.m_PreviousOutputCount == -1)
			{
				__instance.SmartOnSelectAction(null);
				return;
			}
			RefinementActionItemVM refinementActionItemVM;
			if ((refinementActionItemVM = __instance.AvailableRefinementActions.FirstOrDefault((RefinementActionItemVM x) => x.RefineFormula.Output == RefinementVMPatches.m_PreviousOutputType && x.RefineFormula.OutputCount == RefinementVMPatches.m_PreviousOutputCount)) == null)
			{
				refinementActionItemVM = __instance.AvailableRefinementActions.FirstOrDefault((RefinementActionItemVM x) => x.RefineFormula.Output == RefinementVMPatches.m_PreviousOutputType);
			}
			RefinementActionItemVM refinementActionItemVM2 = refinementActionItemVM;
			if (refinementActionItemVM2 == null)
			{
				if (RefinementVMPatches.m_PreviousActionIndex >= __instance.AvailableRefinementActions.Count)
				{
					RefinementVMPatches.m_PreviousActionIndex = __instance.AvailableRefinementActions.Count - 1;
				}
				refinementActionItemVM2 = __instance.AvailableRefinementActions[RefinementVMPatches.m_PreviousActionIndex];
			}
			if (!refinementActionItemVM2.IsEnabled)
			{
				while (RefinementVMPatches.m_PreviousActionIndex >= 0)
				{
					if (__instance.AvailableRefinementActions[RefinementVMPatches.m_PreviousActionIndex].IsEnabled)
					{
						refinementActionItemVM2 = __instance.AvailableRefinementActions[RefinementVMPatches.m_PreviousActionIndex];
						break;
					}
					RefinementVMPatches.m_PreviousActionIndex--;
				}
			}
			if (!refinementActionItemVM2.IsEnabled)
			{
				return;
			}
			__instance.SmartOnSelectAction(refinementActionItemVM2);
		}

		private static CraftingMaterials m_PreviousOutputType;

		private static int m_PreviousOutputCount;

		private static int m_PreviousActionIndex;
	}
}
