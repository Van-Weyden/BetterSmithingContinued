using System;
using System.Linq;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI.ViewModels.Templates;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public class BetterSmithingVM : ConnectedViewModel
	{
		[DataSourceProperty]
		public HintViewModel SettingHint
		{
			get
			{
				return this.m_SettingsHint;
			}
			set
			{
				if (value != this.m_SettingsHint)
				{
					this.m_SettingsHint = value;
					base.OnPropertyChanged("SettingHint");
				}
			}
		}

		[DataSourceProperty]
		public ButtonToggleVM ExcludeNoEnergyHeroesToggle
		{
			get
			{
				return this.m_ExcludeNoEnergyHeroesToggle;
			}
			set
			{
				if (value != this.m_ExcludeNoEnergyHeroesToggle)
				{
					this.m_ExcludeNoEnergyHeroesToggle = value;
					base.OnPropertyChanged("ExcludeNoEnergyHeroesToggle");
				}
			}
		}

		[DataSourceProperty]
		public ButtonToggleVM ExcludeHeroesWithoutSameRecipeToggle
		{
			get
			{
				return this.m_ExcludeHeroesWithoutSameRecipeToggle;
			}
			set
			{
				if (value != this.m_ExcludeHeroesWithoutSameRecipeToggle)
				{
					this.m_ExcludeHeroesWithoutSameRecipeToggle = value;
					base.OnPropertyChanged("ExcludeHeroesWithoutSameRecipeToggle");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<PerkPanelItemVM> PerkPanelItems
		{
			get
			{
				return this.m_PerkPanelItems;
			}
			set
			{
				if (value != this.m_PerkPanelItems)
				{
					this.m_PerkPanelItems = value;
					base.OnPropertyChanged("PerkPanelItems");
				}
			}
		}

		public void OpenSettings()
		{
			if (this.m_SettingsLayer == null)
			{
				this.m_SettingsLayer = new GauntletLayer(52, "GauntletLayer", false);
				this.m_SettingsVM = new BetterSmithingSettingsVM(base.PublicContainer, this.m_ParentScreen, new Action(this.CloseSettings));
				this.m_SettingsMovie = this.m_SettingsLayer.LoadMovie("BetterSmithingContinuedSettingsScreen", this.m_SettingsVM);
				this.m_SettingsLayer.IsFocusLayer = true;
				ScreenManager.TrySetFocus(this.m_SettingsLayer);
				this.m_SettingsLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
				this.m_ParentScreen.AddLayer(this.m_SettingsLayer);
				this.m_SettingsLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			}
		}

		public void CloseSettings()
		{
			if (this.m_SettingsLayer != null)
			{
				this.m_SettingsLayer.ReleaseMovie(this.m_SettingsMovie);
				this.m_ParentScreen.RemoveLayer(this.m_SettingsLayer);
				this.m_SettingsLayer.InputRestrictions.ResetInputRestrictions();
				this.m_SettingsLayer = null;
				this.m_SettingsVM = null;
				this.RefreshValues();
			}
		}

		public BetterSmithingVM(IPublicContainer _publicContainer, GauntletCraftingScreen _parentScreen) : base(_publicContainer)
		{
			this.m_ParentScreen = _parentScreen;
		}

		public ISmithingManager SmithingManager()
		{
			return this.m_SmithingManager;
		}

		public override void Load()
		{
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_SettingsManager = base.PublicContainer.GetModule<ISettingsManager>("");
			this.InitializeChildren();
			this.m_SmithingManager.CurrentCraftingHeroChanged += this.OnCurrentCraftingHeroChanged;
			this.ExcludeNoEnergyHeroesToggle.ToggleStateChanged += this.OnExcludeNoEnergyHeroesToggleStateChanged;
			this.ExcludeHeroesWithoutSameRecipeToggle.ToggleStateChanged += this.OnExcludeHeroesWithoutSameRecipeToggleStateChanged;
			this.m_SmithingManager.CraftingScreenChanged += this.OnCraftingScreenChanged;
			this.OnCraftingScreenChanged(this.m_SmithingManager, this.m_SmithingManager.CurrentCraftingScreen);
			this.RefreshValues();
		}

		public override void Unload()
		{
			this.m_SmithingManager.CurrentCraftingHeroChanged -= this.OnCurrentCraftingHeroChanged;
			this.m_SmithingManager.CraftingScreenChanged -= this.OnCraftingScreenChanged;
			this.ExcludeNoEnergyHeroesToggle.ToggleStateChanged -= this.OnExcludeNoEnergyHeroesToggleStateChanged;
			this.ExcludeHeroesWithoutSameRecipeToggle.ToggleStateChanged -= this.OnExcludeHeroesWithoutSameRecipeToggleStateChanged;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.m_SettingsHint.RefreshValues();
			this.m_ExcludeNoEnergyHeroesToggle.RefreshValues();
			this.m_ExcludeHeroesWithoutSameRecipeToggle.RefreshValues();
			this.m_PerkPanelItems.ApplyActionOnAllItems(delegate(PerkPanelItemVM x) {
				x.RefreshValues();
			});
		}

		public void OpenSettingsScreen()
		{
			SettingsButtonMessages.DisplayNextSettingsButtonMessage();
		}

		private void OnCurrentCraftingHeroChanged(object _sender, CraftingAvailableHeroItemVM _currentCraftingHero)
		{
			this.UpdatePerksPanel(_currentCraftingHero, this.m_SmithingManager.CurrentCraftingScreen);
		}

		private void UpdatePerksPanel(CraftingAvailableHeroItemVM _currentCraftingHero, CraftingScreen _currentCraftingScreen)
		{
			if (_currentCraftingHero == null)
			{
				return;
			}
			foreach (BasicTooltipViewModel basicTooltipViewModel in from x in this.PerkPanelItems
			select x.PerkBasicHint)
			{
				basicTooltipViewModel.ExecuteCommand("ExecuteEndHint", new object[0]);
			}
			this.PerkPanelItems.Clear();
			MBBindingList<PerkPanelItemVM> perkPanelItems;
			switch (_currentCraftingScreen)
			{
			case CraftingScreen.None:
				perkPanelItems = new MBBindingList<PerkPanelItemVM>();
				break;
			case CraftingScreen.Smelting:
				perkPanelItems = this.GetSmeltingPerksPanel(_currentCraftingHero);
				break;
			case CraftingScreen.Crafting:
				perkPanelItems = this.GetCraftingPerksPanel(_currentCraftingHero);
				break;
			case CraftingScreen.Refining:
				perkPanelItems = this.GetRefiningPerksPanel(_currentCraftingHero);
				break;
			default:
				perkPanelItems = this.PerkPanelItems;
				break;
			}
			this.PerkPanelItems = perkPanelItems;
			this.PerkPanelItems.ApplyActionOnAllItems(delegate(PerkPanelItemVM x)
			{
				x.RefreshValues();
			});
		}

		private MBBindingList<PerkPanelItemVM> GetSmeltingPerksPanel(CraftingAvailableHeroItemVM _currentCraftingHero)
		{
			MBBindingList<PerkPanelItemVM> mbbindingList = new MBBindingList<PerkPanelItemVM>();
			PerkPanelItemVM item;
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.CuriousSmelter, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.PracticalSmelter, out item))
			{
				mbbindingList.Add(item);
			}
			return mbbindingList;
		}

		private MBBindingList<PerkPanelItemVM> GetCraftingPerksPanel(CraftingAvailableHeroItemVM _currentCraftingHero)
		{
			MBBindingList<PerkPanelItemVM> mbbindingList = new MBBindingList<PerkPanelItemVM>();
			PerkPanelItemVM item;
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.CuriousSmith, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.PracticalSmith, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.SharpenedEdge, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.SharpenedTip, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.ExperiencedSmith, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.MasterSmith, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.LegendarySmith, out item))
			{
				mbbindingList.Add(item);
			}
			return mbbindingList;
		}

		private MBBindingList<PerkPanelItemVM> GetRefiningPerksPanel(CraftingAvailableHeroItemVM _currentCraftingHero)
		{
			MBBindingList<PerkPanelItemVM> mbbindingList = new MBBindingList<PerkPanelItemVM>();
			PerkPanelItemVM item;
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.PracticalRefiner, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.CharcoalMaker, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.IronMaker, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.SteelMaker, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.SteelMaker2, out item))
			{
				mbbindingList.Add(item);
			}
			if (this.TryGetPerkPanelItem(_currentCraftingHero, DefaultPerks.Crafting.SteelMaker3, out item))
			{
				mbbindingList.Add(item);
			}
			return mbbindingList;
		}

		private bool TryGetPerkPanelItem(CraftingAvailableHeroItemVM _currentCraftingHero, PerkObject _perkObject, out PerkPanelItemVM _perkPanelItem)
		{
			if (_currentCraftingHero.Hero.HeroDeveloper.GetPerkValue(_perkObject))
			{
				_perkPanelItem = new PerkPanelItemVM
				{
					PerkBasicHint = new BasicTooltipViewModel(() => CampaignUIHelper.GetPerkEffectText(_perkObject, true)),
					PerkHint = HintViewModelUtilities.CreateHintViewModel(CampaignUIHelper.GetPerkEffectText(_perkObject, true).First<TooltipProperty>().ValueLabel),
					PerkText = _perkObject.Name.ToString(),
					SpriteAsStr = "SPPerks\\" + _perkObject.StringId
				};
				return true;
			}
			_perkPanelItem = null;
			return false;
		}

		private void InitializeChildren()
		{
			this.SettingHint = HintViewModelUtilities.CreateHintViewModel("Better Smithing Continued " + new TextObject("{=BSC_BH_Settings}Settings (Coming Soon... Maybe)", null).ToString());
			this.ExcludeNoEnergyHeroesToggle = new ButtonToggleVM(true)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_RLEH}Toggle on to remove heroes with not enough energy from cycles.", null).ToString()),
				SpriteAsStr = "General\\Icons\\Food",
				IsToggledOn = this.m_SettingsManager.GetSettings<SmithingSettings>().OnlyCycleHeroesWithStamina
			};
			this.ExcludeHeroesWithoutSameRecipeToggle = new ButtonToggleVM(true)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_RHWR}Toggle on to remove heroes that do not have the currently selected recipe from cycles.", null).ToString()),
				SpriteAsStr = "General\\Icons\\Militia",
				IsToggledOn = this.m_SettingsManager.GetSettings<RefiningSettings>().OnlyCycleHeroesWithCurrentRecipe
			};
			this.PerkPanelItems = new MBBindingList<PerkPanelItemVM>();
		}

		private void OnCraftingScreenChanged(object _sender, CraftingScreen _currentCraftingScreen)
		{
			this.m_ExcludeHeroesWithoutSameRecipeToggle.IsVisible = (_currentCraftingScreen == CraftingScreen.Refining);
			this.UpdatePerksPanel(this.m_SmithingManager.CurrentCraftingHero, _currentCraftingScreen);
		}

		private void OnExcludeNoEnergyHeroesToggleStateChanged(object _sender, bool _isToggleOn)
		{
			this.m_SettingsManager.GetSettings<SmithingSettings>().OnlyCycleHeroesWithStamina = _isToggleOn;
		}

		private void OnExcludeHeroesWithoutSameRecipeToggleStateChanged(object _sender, bool _isToggleOn)
		{
			this.m_SettingsManager.GetSettings<RefiningSettings>().OnlyCycleHeroesWithCurrentRecipe = _isToggleOn;
		}

		private readonly GauntletCraftingScreen m_ParentScreen;

		private HintViewModel m_SettingsHint;

		private ButtonToggleVM m_ExcludeNoEnergyHeroesToggle;

		private ButtonToggleVM m_ExcludeHeroesWithoutSameRecipeToggle;

		private ISmithingManager m_SmithingManager;

		private ISettingsManager m_SettingsManager;

		private MBBindingList<PerkPanelItemVM> m_PerkPanelItems;

		private GauntletLayer m_SettingsLayer;

		private IGauntletMovie m_SettingsMovie;

		private BetterSmithingSettingsVM m_SettingsVM;
	}
}
