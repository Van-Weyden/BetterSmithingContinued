using System;
using System.Diagnostics;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame
{
	public class SmithingManager : Module, ISmithingManager
	{
		public event EventHandler<CraftingAvailableHeroItemVM> CurrentCraftingHeroChanged;

		public event EventHandler EnteredSmithingMenu;

		public event EventHandler LeavingSmithingMenu;

		public event EventHandler<CraftingScreen> CraftingScreenChanged;

		public CraftingAvailableHeroItemVM CurrentCraftingHero
		{
			get
			{
				return this.m_CurrentCraftingHero;
			}
			set
			{
				if (this.m_CurrentCraftingHero != value)
				{
					this.m_CurrentCraftingHero = value;
					this.OnCurrentCraftingHeroChanged(this.m_CurrentCraftingHero);
				}
			}
		}

		public CraftingVM CraftingVM
		{
			get
			{
				return this.m_CraftingVm;
			}
			set
			{
				if (this.m_CraftingVm != value)
				{
					this.m_CraftingVm = value;
					if (this.m_CraftingVm != null)
					{
						this.m_CurrentCraftingScreen = CraftingScreen.Crafting;
						this.SmeltingItemRoster = new SmeltingItemRosterWrapper(this.m_CraftingVm.Smelting);
						this.m_MainActionTextModifier = new MainActionTextModifier(base.PublicContainer);
						this.m_MainActionTextModifier.Load();
						this.m_CraftingVm.SmartRefreshEnabledMainAction();
						this.CurrentCraftingHero = this.m_CraftingVm.CurrentCraftingHero;
						this.OnEnteredSmithingMenu();
						return;
					}
					this.OnLeavingSmithingMenu();
					this.CurrentCraftingHero = null;
					this.m_MainActionTextModifier.Unload();
					this.m_MainActionTextModifier = null;
					this.m_CurrentCraftingScreen = CraftingScreen.None;
					this.SmeltingItemRoster.Dispose();
					this.SmeltingItemRoster = null;
				}
			}
		}

		public RefinementVM RefinementVM
		{
			get
			{
				CraftingVM craftingVM = this.CraftingVM;
				if (craftingVM == null)
				{
					return null;
				}
				return craftingVM.Refinement;
			}
		}

		public SmeltingVM SmeltingVM
		{
			get
			{
				CraftingVM craftingVM = this.CraftingVM;
				if (craftingVM == null)
				{
					return null;
				}
				return craftingVM.Smelting;
			}
		}

		public WeaponDesignVM WeaponDesignVM
		{
			get
			{
				CraftingVM craftingVM = this.CraftingVM;
				if (craftingVM == null)
				{
					return null;
				}
				return craftingVM.WeaponDesign;
			}
		}

		public CraftingCampaignBehavior CraftingCampaignBehavior
		{
			get
			{
				CraftingVM craftingVM = this.CraftingVM;
				if (craftingVM == null)
				{
					return null;
				}
				return craftingVM.GetCraftingCampaignBehavior();
			}
		}

		public SmeltingItemRosterWrapper SmeltingItemRoster { get; private set; }

		public ItemModifier LastSmithedWeaponModifier { get; set; }

		public CraftingScreen CurrentCraftingScreen
		{
			get
			{
				return this.m_CurrentCraftingScreen;
			}
			set
			{
				if (this.m_CurrentCraftingScreen != value)
				{
					this.m_CurrentCraftingScreen = value;
					this.OnCraftingScreenChanged(this.m_CurrentCraftingScreen);
				}
			}
		}

		public bool HeroHasEnoughStaminaForMainAction(Hero _hero)
		{
			int num = 0;
			try
			{
				switch (this.m_CurrentCraftingScreen)
				{
				case CraftingScreen.Smelting:
				{
					if (this.SmeltingVM == null)
					{
						return this.CraftingCampaignBehavior.GetHeroCraftingStamina(_hero) > (_hero.GetPerkValue(DefaultPerks.Crafting.PracticalSmelter) ? 5 : 10);
					}
					SmithingModel smithingModel = Campaign.Current.Models.SmithingModel;
					SmeltingVM smeltingVM = this.SmeltingVM;
					ItemObject item;
					if (smeltingVM == null)
					{
						item = null;
					}
					else
					{
						SmeltingItemVM currentSelectedItem = smeltingVM.CurrentSelectedItem;
						item = ((currentSelectedItem != null) ? currentSelectedItem.EquipmentElement.Item : null);
					}
					num = smithingModel.GetEnergyCostForSmelting(item, _hero);
					break;
				}
				case CraftingScreen.Crafting:
				{
					if (this.WeaponDesignVM == null)
					{
						return this.CraftingCampaignBehavior.GetHeroCraftingStamina(_hero) > 15;
					}
					SmithingModel smithingModel2 = Campaign.Current.Models.SmithingModel;
					WeaponDesignVM weaponDesignVM = this.WeaponDesignVM;
					num = smithingModel2.GetEnergyCostForSmithing((weaponDesignVM != null) ? weaponDesignVM.GetCraftingComponent().GetCurrentCraftedItemObject(false) : null, _hero);
					break;
				}
				case CraftingScreen.Refining:
				{
					if (this.RefinementVM == null)
					{
						return this.CraftingCampaignBehavior.GetHeroCraftingStamina(_hero) > (_hero.GetPerkValue(DefaultPerks.Crafting.PracticalRefiner) ? 3 : 6);
					}
					RefinementActionItemVM currentSelectedAction = this.RefinementVM.CurrentSelectedAction;
					Crafting.RefiningFormula refiningFormula = (currentSelectedAction != null) ? currentSelectedAction.RefineFormula : null;
					if (refiningFormula == null)
					{
						num = (_hero.GetPerkValue(DefaultPerks.Crafting.PracticalRefiner) ? 3 : 6);
					}
					else
					{
						num = Campaign.Current.Models.SmithingModel.GetEnergyCostForRefining(ref refiningFormula, _hero);
					}
					break;
				}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(new TextObject("{=BSC_EM_05}Exception occurred while calling HeroHasEnoughStaminaForMainAction. Exception: ", null) + ex.ToString());
			}
			return num <= this.CraftingCampaignBehavior.GetHeroCraftingStamina(_hero);
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.RegisterModule<ISmithingManager>("");
		}

		public override void Load()
		{
			this.m_SettingsManager = base.PublicContainer.GetModule<ISettingsManager>("");
			Instances.SettingsManager = this.m_SettingsManager;
			Instances.SmithingManager = this;
		}

		public override void Unload()
		{
			Instances.SettingsManager = null;
			Instances.SmithingManager = null;
		}

		public void OnLeavingSmithingMenu()
		{
			this.m_SettingsManager.Save();
			EventHandler leavingSmithingMenu = this.LeavingSmithingMenu;
			if (leavingSmithingMenu == null)
			{
				return;
			}
			leavingSmithingMenu(null, EventArgs.Empty);
		}

		protected virtual void OnEnteredSmithingMenu()
		{
			EventHandler enteredSmithingMenu = this.EnteredSmithingMenu;
			if (enteredSmithingMenu == null)
			{
				return;
			}
			enteredSmithingMenu(this, EventArgs.Empty);
		}

		protected virtual void OnCurrentCraftingHeroChanged(CraftingAvailableHeroItemVM _e)
		{
			EventHandler<CraftingAvailableHeroItemVM> currentCraftingHeroChanged = this.CurrentCraftingHeroChanged;
			if (currentCraftingHeroChanged == null)
			{
				return;
			}
			currentCraftingHeroChanged(this, _e);
		}

		private void OnCraftingScreenChanged(CraftingScreen _e)
		{
			EventHandler<CraftingScreen> craftingScreenChanged = this.CraftingScreenChanged;
			if (craftingScreenChanged == null)
			{
				return;
			}
			craftingScreenChanged(null, _e);
		}

		private CraftingScreen m_CurrentCraftingScreen;

		private CraftingVM m_CraftingVm;

		private ISettingsManager m_SettingsManager;

		private MainActionTextModifier m_MainActionTextModifier;

		private CraftingAvailableHeroItemVM m_CurrentCraftingHero;
	}
}
