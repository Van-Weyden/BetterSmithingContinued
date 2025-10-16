using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame
{
	public interface ICraftingRepeater
	{
		void AddWeaponTierType();

		void DoMultiCrafting(ref CraftingCampaignBehavior __instance, Hero hero, WeaponDesign weaponDesign);
	}
}
