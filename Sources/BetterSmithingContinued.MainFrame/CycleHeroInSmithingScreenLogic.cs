using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.Inputs.Code;
using BetterSmithingContinued.MainFrame.HotKeys;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using MCM.Abstractions.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using static BetterSmithingContinued.Settings.CharacterCycleDropdownOption;

namespace BetterSmithingContinued.MainFrame
{
	public class CycleHeroInSmithingScreenLogic : Module, ICycleHeroInSmithingScreenLogic
	{
		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
            this.RegisterModule<ICycleHeroInSmithingScreenLogic>("");
		}

		public override void Load()
		{
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_InputManager = base.PublicContainer.GetModule<IInputManager>("");
			this.m_SettingsManager = base.PublicContainer.GetModule<ISettingsManager>("");
			this.m_BetterSmithingUIContext = base.PublicContainer.GetModule<IBetterSmithingUIContext>("");
			this.m_SettingsManager.SettingsSectionChanged += this.OnSettingsSectionChanged;
			this.m_SmithingSettings = this.m_SettingsManager.GetSettings<SmithingSettings>();
			this.m_RefiningSettings = this.m_SettingsManager.GetSettings<RefiningSettings>();
			CycleHeroForwardKey cycleHeroForwardKey = new CycleHeroForwardKey {
				IsEnabled = new Func<bool>(this.CanCycleHero)
			};
			cycleHeroForwardKey.KeyStateChanged += this.OnCycleHeroForwardKeyStateChanged;
			this.m_InputManager.AddHotkey(cycleHeroForwardKey);
			CycleHeroBackwardKey cycleHeroBackwardKey = new CycleHeroBackwardKey {
				IsEnabled = new Func<bool>(this.CanCycleHero)
			};
			cycleHeroBackwardKey.KeyStateChanged += this.OnCycleHeroBackwardKeyStateChanged;
			this.m_InputManager.AddHotkey(cycleHeroBackwardKey);
		}

		public override void Unload()
		{
			this.m_SettingsManager.SettingsSectionChanged -= this.OnSettingsSectionChanged;
		}

		private bool CanCycleHero()
		{
			if (this.m_SmithingManager.CraftingVM == null || this.m_BetterSmithingUIContext.IsEditableTextWidgetFocused)
			{
				return false;
			}
			WeaponDesignVM weaponDesign = this.m_SmithingManager.CraftingVM.WeaponDesign;
			return weaponDesign == null || !weaponDesign.IsInFinalCraftingStage;
		}

		private void OnSettingsSectionChanged(object _sender, SettingsSection _e)
		{
            if (_e is SmithingSettings smithingSettings)
            {
                this.m_SmithingSettings = smithingSettings;
            }
            else if (_e is RefiningSettings refiningSettings)
            {
                this.m_RefiningSettings = refiningSettings;
            }
        }

		private void OnCycleHeroForwardKeyStateChanged(object _sender, KeyState _keyState)
		{
			try
			{
				if (_keyState == KeyState.KeyPressed)
				{
                    CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = this.m_SmithingManager.CraftingVM.CurrentCraftingHero;
                    if (this.NextCycleHero(ref craftingAvailableHeroItemVM))
					{
                        this.m_SmithingManager.CraftingVM.UpdateCraftingHero(craftingAvailableHeroItemVM);
                    }
					else
					{
                        InformationManager.DisplayMessage(new InformationMessage(
                            new TextObject("{=BSC_Msg_NHA}No other valid hero available.", null).ToString()
                        ));
                    }
                }
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(
					new TextObject("{=BSC_Ethis.m_04}Exception thrown while attempting to cycle smithing character. Exception: ", null) + ex.Message,
					Colors.Red,
					"Exception"
				));
			}
		}

		private void OnCycleHeroBackwardKeyStateChanged(object _sender, KeyState _keyState)
		{
			try
			{
				if (_keyState == KeyState.KeyPressed)
				{
                    CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = this.m_SmithingManager.CraftingVM.CurrentCraftingHero;
                    if (this.PrevCycleHero(ref craftingAvailableHeroItemVM))
                    {
                        this.m_SmithingManager.CraftingVM.UpdateCraftingHero(craftingAvailableHeroItemVM);
					}
					else
					{
						InformationManager.DisplayMessage(new InformationMessage(
							new TextObject("{=BSC_Msg_NHA}No other valid hero available.", null).ToString()
						));
					}
				}
			}
			catch (Exception ex)
			{
                InformationManager.DisplayMessage(new InformationMessage(
                    new TextObject("{=BSC_Ethis.m_04}Exception thrown while attempting to cycle smithing character. Exception: ", null) + ex.Message,
                    Colors.Red,
                    "Exception"
				));
            }
		}

		private List<CraftingAvailableHeroItemVM> CycleHeroList()
		{
			if (GlobalSettings<MCMBetterSmithingSettings>.Instance.ReorderCharactersInMenu)
			{
                return this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.ToList();
            }

            OrderType orderType = GlobalSettings<MCMBetterSmithingSettings>.Instance.CharacterCycleType.SelectedValue.Type;
            switch (orderType)
            {
                case OrderType.Default:
                    return this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.ToList();

                case OrderType.SkillAsc:
                    return this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.OrderBy(
                        character => character.SmithySkillLevel
                    ).ToList();

                case OrderType.SkillDesc:
                    return this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.OrderByDescending(
                        character => character.SmithySkillLevel
                    ).ToList();
            }

			return null;
        }

        private bool NextCycleHero(ref CraftingAvailableHeroItemVM hero)
        {
			List<CraftingAvailableHeroItemVM> list = CycleHeroList();
   			if (list == null)
			{
				return false;
			}

            int index = list.IndexOf(hero);
            do
			{
                index = (index + 1 >= list.Count ? 0 : index + 1);
                hero = list[index];
                if (hero == this.m_SmithingManager.CraftingVM.CurrentCraftingHero)
                {
                    return false;
                }
            } while (!this.IsHeroValidCycleTarget(hero));
            
            return true;
		}

        private bool PrevCycleHero(ref CraftingAvailableHeroItemVM hero)
        {
            List<CraftingAvailableHeroItemVM> list = CycleHeroList();
            if (list == null)
            {
                return false;
            }

            int index = list.IndexOf(hero);
            do
            {
                index = (index - 1 < 0 ? list.Count - 1 : index - 1);
                hero = list[index];
                if (hero == this.m_SmithingManager.CraftingVM.CurrentCraftingHero)
                {
                    return false;
                }
            } while (!this.IsHeroValidCycleTarget(hero));

            return true;
        }

        private bool IsHeroValidCycleTarget(CraftingAvailableHeroItemVM _hero)
		{
			if (this.m_SmithingSettings.OnlyCycleHeroesWithStamina && !this.m_SmithingManager.HeroHasEnoughStaminaForMainAction(_hero.Hero))
			{
				return false;
			}

			if (this.m_RefiningSettings.OnlyCycleHeroesWithCurrentRecipe && this.m_SmithingManager.CurrentCraftingScreen == CraftingScreen.Refining)
			{
				Crafting.RefiningFormula currentRefiningFormula = this.m_SmithingManager.RefinementVM.CurrentSelectedAction?.RefineFormula;

				if (currentRefiningFormula == null)
				{
					return true;
				}

				currentRefiningFormula = Campaign.Current.Models.SmithingModel.GetRefiningFormulas(_hero.Hero).FirstOrDefault(
					x => x.Output == currentRefiningFormula.Output && x.OutputCount == currentRefiningFormula.OutputCount
				);

                if (currentRefiningFormula == null)
				{
					return false;
				}
			}

			return true;
		}

        private SmithingSettings m_SmithingSettings;
		private RefiningSettings m_RefiningSettings;
		private ISmithingManager m_SmithingManager;
		private IInputManager m_InputManager;
		private ISettingsManager m_SettingsManager;
		private IBetterSmithingUIContext m_BetterSmithingUIContext;
	}
}
