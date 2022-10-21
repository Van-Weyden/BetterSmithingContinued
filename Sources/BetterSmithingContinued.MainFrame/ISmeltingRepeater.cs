using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame
{
	public interface ISmeltingRepeater
	{
		void DoSmelting(ref CraftingCampaignBehavior _instance, Hero _hero, EquipmentElement _equipmentElement, int _startIndex);
	}
}
