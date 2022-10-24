using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame.Patches
{
	[HarmonyPatch(typeof(DefaultSmithingModel))]
	public class DefaultSmithingModelPatches
	{
		[HarmonyPatch("GetRefiningFormulas")]
		[HarmonyPostfix]
		public static void GetRefiningFormulasPostfix(ref IEnumerable<Crafting.RefiningFormula> __result)
		{
			if (__result == null)
			{
				return;
			}
			List<Crafting.RefiningFormula> list = __result.ToList<Crafting.RefiningFormula>();
			Crafting.RefiningFormula[] array = list.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = i + 1; j < array.Length; j++)
				{
					if (array[i].Output == array[j].Output)
					{
						list.Remove((array[i].OutputCount > array[j].OutputCount) ? array[j] : array[i]);
					}
				}
			}
			__result = list;
		}

		[HarmonyPatch("GetModifierTierForSmithedWeapon")]
		[HarmonyPostfix]
		private static void GetModifierTierForSmithedWeaponPostfix(ref int __result)
		{
			Instances.SmithingManager.LastSmithedWeaponTier = __result;
		}
	}
}
