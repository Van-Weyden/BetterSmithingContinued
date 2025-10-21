using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame
{
	public interface ICraftingRepeater
    {
        void OnWeaponCrafted(ItemObject weapon, ItemModifier weaponModifier);

        void InitMultiCrafting(Hero hero, WeaponDesign weaponDesign);
        void DoMultiCrafting(ICraftingCampaignBehavior craftingBehavior);
	}
}
