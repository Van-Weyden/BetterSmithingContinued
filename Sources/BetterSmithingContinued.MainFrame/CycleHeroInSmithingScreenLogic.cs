using System;
using System.Linq;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.Inputs.Code;
using BetterSmithingContinued.MainFrame.HotKeys;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

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
			CycleHeroForwardKey cycleHeroForwardKey = new CycleHeroForwardKey
			{
				IsEnabled = new Func<bool>(this.CanCycleHero)
			};
			cycleHeroForwardKey.KeyStateChanged += this.OnCycleHeroForwardKeyStateChanged;
			this.m_InputManager.AddHotkey(cycleHeroForwardKey);
			CycleHeroBackwardKey cycleHeroBackwardKey = new CycleHeroBackwardKey
			{
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
			SmithingSettings smithingSettings = _e as SmithingSettings;
			if (smithingSettings != null)
			{
				this.m_SmithingSettings = smithingSettings;
				return;
			}
			RefiningSettings refiningSettings = _e as RefiningSettings;
			if (refiningSettings != null)
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
					int num = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.FindIndex((CraftingAvailableHeroItemVM hero) => hero == this.m_SmithingManager.CraftingVM.CurrentCraftingHero);
					int count = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.Count;
					int num2 = num;
					CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = null;
					for (int i = 0; i < count - 1; i++)
					{
						num2++;
						if (num2 >= count)
						{
							num2 = 0;
						}
						CraftingAvailableHeroItemVM craftingAvailableHeroItemVM2 = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing[num2];
						if (this.IsHeroValidCycleTarget(craftingAvailableHeroItemVM2))
						{
							craftingAvailableHeroItemVM = craftingAvailableHeroItemVM2;
							break;
						}
					}
					if (craftingAvailableHeroItemVM == null)
					{
						InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_Msg_NHA}No other valid hero available.", null).ToString()));
					}
					else
					{
						this.m_SmithingManager.CraftingVM.UpdateCraftingHero(craftingAvailableHeroItemVM);
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_04}Exception thrown while attempting to cycle smithing character. Exception: ", null) + ex.Message, Colors.Red, "Exception"));
			}
		}

		private void OnCycleHeroBackwardKeyStateChanged(object _sender, KeyState _keyState)
		{
			try
			{
				if (_keyState == KeyState.KeyPressed)
				{
					int num = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.FindIndex((CraftingAvailableHeroItemVM hero) => hero == this.m_SmithingManager.CraftingVM.CurrentCraftingHero);
					int count = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.Count;
					int num2 = num;
					CraftingAvailableHeroItemVM craftingAvailableHeroItemVM = null;
					for (int i = 0; i < count - 1; i++)
					{
						num2--;
						if (num2 < 0)
						{
							num2 = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing.Count - 1;
						}
						CraftingAvailableHeroItemVM craftingAvailableHeroItemVM2 = this.m_SmithingManager.CraftingVM.AvailableCharactersForSmithing[num2];
						if (this.IsHeroValidCycleTarget(craftingAvailableHeroItemVM2))
						{
							craftingAvailableHeroItemVM = craftingAvailableHeroItemVM2;
							break;
						}
					}
					if (craftingAvailableHeroItemVM == null)
					{
						InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_Msg_NHA}No other valid hero available.", null).ToString()));
					}
					else
					{
						this.m_SmithingManager.CraftingVM.UpdateCraftingHero(craftingAvailableHeroItemVM);
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_04}Exception thrown while attempting to cycle smithing character. Exception: ", null) + ex.Message, Colors.Red, "Exception"));
			}
		}

		private bool IsHeroValidCycleTarget(CraftingAvailableHeroItemVM _hero)
		{
			if (this.m_SmithingSettings.OnlyCycleHeroesWithStamina && !this.m_SmithingManager.HeroHasEnoughStaminaForMainAction(_hero.Hero))
			{
				return false;
			}
			if (this.m_RefiningSettings.OnlyCycleHeroesWithCurrentRecipe && this.m_SmithingManager.CurrentCraftingScreen == CraftingScreen.Refining)
			{
				RefinementActionItemVM currentSelectedAction = this.m_SmithingManager.RefinementVM.CurrentSelectedAction;
				Crafting.RefiningFormula currentRefiningFormula = (currentSelectedAction != null) ? currentSelectedAction.RefineFormula : null;
				if (currentRefiningFormula == null)
				{
					return true;
				}
				if (Campaign.Current.Models.SmithingModel.GetRefiningFormulas(_hero.Hero).FirstOrDefault((Crafting.RefiningFormula x) => x.Output == currentRefiningFormula.Output && x.OutputCount == currentRefiningFormula.OutputCount) == null)
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
