using System;
using BetterSmithingContinued.MainFrame.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;

namespace BetterSmithingContinued.MainFrame
{
	public interface ISmithingManager
	{
		event EventHandler EnteredSmithingMenu;

		event EventHandler LeavingSmithingMenu;

		event EventHandler<CraftingScreen> CraftingScreenChanged;

		event EventHandler<CraftingAvailableHeroItemVM> CurrentCraftingHeroChanged;

		CraftingVM CraftingVM { get; set; }

		RefinementVM RefinementVM { get; }

		SmeltingVM SmeltingVM { get; }

		WeaponDesignVM WeaponDesignVM { get; }

		CraftingCampaignBehavior CraftingCampaignBehavior { get; }

		SmeltingItemRosterWrapper SmeltingItemRoster { get; }

		int LastSmithedWeaponTier { get; set; }

		CraftingScreen CurrentCraftingScreen { get; set; }

		CraftingAvailableHeroItemVM CurrentCraftingHero { get; set; }

		bool ApplyNamePrefix { get; set; }

		bool HeroHasEnoughStaminaForMainAction(Hero _hero);
	}
}
