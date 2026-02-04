using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
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
			List<Crafting.RefiningFormula> list = __result.ToList();
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

			// Alternate recipes directly from the crude iron
			//int count = list.Count;
   //         for (int i = 0; i < count; i++)
			//{
			//	int ironTier = list[i].Output - CraftingMaterials.Iron2; // Wrought iron is 0 tier here, thamaskene is 4
   //             if (
   //                 list[i].OutputCount == 1 && list[i].Output2Count == 1 &&
   //                 ironTier >= 0 && ironTier <= 4 &&
   //                 list[i].Output2 == CraftingMaterials.Iron1
			//	)
			//	{
			//		list.Add(new Crafting.RefiningFormula(
			//			CraftingMaterials.Iron1, 1 << ironTier, 
			//			CraftingMaterials.Charcoal, (1 << (ironTier + 1)) - 1,
   //                     list[i].Output, list[i].OutputCount,
   //                     list[i].Output2, list[i].Output2Count
			//		));
   //             }
			//}

			__result = list;
		}
	}
}
