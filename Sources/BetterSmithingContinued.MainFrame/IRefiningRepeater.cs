using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame
{
	public interface IRefiningRepeater
	{
		void DoRefinement(ref CraftingCampaignBehavior __instance, Hero hero, Crafting.RefiningFormula refineFormula);
	}
}
