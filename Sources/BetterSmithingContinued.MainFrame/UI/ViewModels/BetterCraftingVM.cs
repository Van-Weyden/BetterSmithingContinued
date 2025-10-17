using System;

using SandBox.GauntletUI;

using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI.ViewModels.Templates;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public sealed class BetterCraftingVM : ConnectedViewModel, ICraftingStateHandler
	{
		[DataSourceProperty]
		public ButtonToggleVM ShowNewWeaponPopupToggle
		{
			get
			{
				return this.m_ShowNewWeaponPopupToggle;
			}
			set
			{
				if (value != this.m_ShowNewWeaponPopupToggle)
				{
					this.m_ShowNewWeaponPopupToggle = value;
					base.OnPropertyChanged("ShowNewWeaponPopupToggle");
				}
			}
		}

		[DataSourceProperty]
		public TextButtonVM SaveButton
		{
			get
			{
				return this.m_SaveButton;
			}
			set
			{
				if (this.m_SaveButton != value)
				{
					this.m_SaveButton = value;
					base.OnPropertyChanged("SaveButton");
				}
			}
		}

		[DataSourceProperty]
		public TextButtonVM CancelButton
		{
			get
			{
				return this.m_CancelButton;
			}
			set
			{
				if (this.m_CancelButton != value)
				{
					this.m_CancelButton = value;
					base.OnPropertyChanged("CancelButton");
				}
			}
		}

		[DataSourceProperty]
		public TextButtonVM EditButton
		{
			get
			{
				return this.m_EditButton;
			}
			set
			{
				if (this.m_EditButton != value)
				{
					this.m_EditButton = value;
					base.OnPropertyChanged("EditButton");
				}
			}
		}

		[DataSourceProperty]
		public TextButtonVM DeleteButton
		{
			get
			{
				return this.m_DeleteButton;
			}
			set
			{
				if (this.m_DeleteButton != value)
				{
					this.m_DeleteButton = value;
					base.OnPropertyChanged("DeleteButton");
				}
			}
		}

		[DataSourceProperty]
		public string SavedWeaponListToggleText
		{
			get
			{
				return this.m_SavedWeaponListToggleText;
			}
			set
			{
				if (this.m_SavedWeaponListToggleText != value)
				{
					this.m_SavedWeaponListToggleText = value;
					base.OnPropertyChanged("SavedWeaponListToggleText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel SavedWeaponListToggleHint
		{
			get
			{
				return this.m_SavedWeaponListToggleHint;
			}
			set
			{
				if (this.m_SavedWeaponListToggleHint != value)
				{
					this.m_SavedWeaponListToggleHint = value;
					base.OnPropertyChanged("SavedWeaponListToggleHint");
				}
			}
		}

		[DataSourceProperty]
		public string WeaponName
		{
			get
			{
				return this.m_WeaponName;
			}
			set
			{
				if (this.m_WeaponName != value)
				{
					if (value == null || value == "")
					{
						if (this.m_WeaponName == this.m_DefaultWeaponName)
						{
							return;
						}

						this.m_WeaponName = this.m_DefaultWeaponName;
					}
					else
					{
						this.m_WeaponName = value;
					}

					base.OnPropertyChanged("WeaponName");
					this.m_SmithingManager.WeaponDesignVM.ItemName = this.m_WeaponName;
				}
			}
		}

		[DataSourceProperty]
		public bool IsWeaponNameInputVisible
		{
			get
			{
				return this.m_IsWeaponNameInputVisible;
			}
			set
			{
				if (this.m_IsWeaponNameInputVisible != value)
				{
					this.m_IsWeaponNameInputVisible = value;
					base.OnPropertyChanged("IsWeaponNameInputVisible");
				}
			}
		}

		private bool IsEditing
		{
			get
			{
				return this.m_IsEditing;
			}
			set
			{
				if (this.m_IsEditing != value)
				{
					this.m_IsEditing = value;
					if (this.m_IsEditing)
					{
						this.m_EditTarget = this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItem.WeaponData;
						this.CancelButton.IsVisible = true;
						this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible = true;
					}
					else
					{
						this.m_EditTarget = null;
						this.CancelButton.IsVisible = false;
						this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible = false;
					}
					this.UpdateWeaponNameInputVisibility();
					this.UpdateCategorySelectionIsVisible();
				}
			}
		}

		public BetterCraftingVM(IPublicContainer _publicContainer, GauntletCraftingScreen _parentScreen) : 
			base(_publicContainer)
		{
			this.m_ParentScreen = _parentScreen;
			MemberExtractor.GetPrivateFieldValue(m_ParentScreen, "_craftingState", out this.m_CraftingState);
		}

		public override void Load()
		{
			this.m_SettingsManager = base.PublicContainer.GetModule<ISettingsManager>("");
			this.m_CraftingSettings = this.m_SettingsManager.GetSettings<CraftingSettings>();
			this.m_WeaponSaveData = this.m_SettingsManager.GetSettings<WeaponSaveData>();
			this.m_SettingsManager.SettingsSectionChanged += this.OnSettingsChanged;
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			WeaponDesignVMMixin.InstanceChanged += this.OnWeaponDesignVMMixinInstanceChanged;
			if (WeaponDesignVMMixin.Instance != null)
			{
				this.OnWeaponDesignVMMixinInstanceChanged(null, WeaponDesignVMMixin.Instance);
			}
			this.InitializeChildren();
			this.m_CraftingState.Handler = this;
			this.OnDefaultWeaponNameChanged();
			this.UpdateWeaponNameInputVisibility();
			this.RefreshValues();
			IBetterSmithingUIContext module = base.PublicContainer.GetModule<IBetterSmithingUIContext>("");
			module.DeferAction(delegate
			{
				this.m_ShowNewWeaponPopupToggle.IsVisible = false;
			}, 1);
			module.DeferAction(delegate
			{
				this.m_ShowNewWeaponPopupToggle.IsVisible = true;
			}, 2);
			module.DeferAction(delegate
			{
				this.m_ShowNewWeaponPopupToggle.IsVisible = false;
			}, 3);
			module.DeferAction(delegate
			{
				this.m_ShowNewWeaponPopupToggle.IsVisible = true;
			}, 4);
		}

		public override void Unload()
		{
			this.m_SettingsManager.SettingsSectionChanged -= this.OnSettingsChanged;
			if (this.SaveButton != null)
			{
				this.SaveButton.ButtonPressed -= this.OnSaveButtonPressed;
			}
			if (this.EditButton != null)
			{
				this.EditButton.ButtonPressed -= this.OnEditButtonPressed;
			}
			if (this.DeleteButton != null)
			{
				this.DeleteButton.ButtonPressed -= this.OnDeleteButtonPressed;
			}
			if (this.m_WeaponDesignVMMixinInstance != null)
			{
				this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItemChanged -= this.OnCurrentlySelectedItemChanged;
				this.m_WeaponDesignVMMixinInstance.ViewModeChanged -= this.OnViewModeChanged;
			}
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.SaveButton.RefreshValues();
			this.CancelButton.RefreshValues();
			this.EditButton.RefreshValues();
			this.DeleteButton.RefreshValues();
			this.SavedWeaponListToggleHint.RefreshValues();
			this.ShowNewWeaponPopupToggle.RefreshValues();
		}

		public void SavedWeaponListTogglePressed()
		{
			if (this.m_WeaponDesignVMMixinInstance != null)
			{
				this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible = !this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible;
				Instances.BetterSmithingUIContext.IsInNormalCraftingScreen = this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible;
				if (this.m_WeaponDesignVMMixinInstance.IsDefaultCraftingMenuVisible)
				{
					this.SavedWeaponListToggleText = new TextObject("{=BSC_BT_Designs}Saved Designs", null).ToString();
					return;
				}
				this.SavedWeaponListToggleText = new TextObject("{=BSC_BT_Craft}Crafting", null).ToString();
			}
		}

		public void OnCraftingLogicInitialized()
		{
			this.m_ParentScreen.OnCraftingLogicInitialized();
			this.OnDefaultWeaponNameChanged();
		}

		public void OnCraftingLogicRefreshed()
		{
			this.m_ParentScreen.OnCraftingLogicRefreshed();
			this.OnDefaultWeaponNameChanged();
		}

		private void OnWeaponDesignVMMixinInstanceChanged(object _sender, WeaponDesignVMMixin _instance)
		{
			if (this.m_WeaponDesignVMMixinInstance != null)
			{
				this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItemChanged -= this.OnCurrentlySelectedItemChanged;
				this.m_WeaponDesignVMMixinInstance.ViewModeChanged -= this.OnViewModeChanged;
			}
			this.m_WeaponDesignVMMixinInstance = _instance;
			if (_instance != null)
			{
				this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItemChanged += this.OnCurrentlySelectedItemChanged;
				this.m_WeaponDesignVMMixinInstance.ViewModeChanged += this.OnViewModeChanged;
			}
		}

		private void OnViewModeChanged(object _sender, EventArgs _e)
		{
			if (WeaponDesignVMMixin.Instance.IsDefaultCraftingMenuVisible)
			{
				this.SaveButton.IsVisible = true;
				this.EditButton.IsVisible = false;
				this.DeleteButton.IsVisible = false;
			}
			else
			{
				this.IsEditing = false;
				this.SaveButton.IsVisible = false;
				this.EditButton.IsVisible = true;
				this.DeleteButton.IsVisible = true;
			}
			this.UpdateWeaponNameInputVisibility();
			this.UpdateCategorySelectionIsVisible();
		}

		private void UpdateCategorySelectionIsVisible()
		{
			if (this.m_WeaponDesignVMMixinInstance != null)
			{
				this.m_WeaponDesignVMMixinInstance.IsCategorySelectionEnabled = !this.IsEditing;
			}
		}

		private void OnCurrentlySelectedItemChanged(object _sender, CraftingItemTemplateVM _currentlySelectedItem)
		{
			if (_currentlySelectedItem == null)
			{
				this.EditButton.IsEnabled = false;
				this.DeleteButton.IsEnabled = false;
				return;
			}
			this.WeaponName = _currentlySelectedItem.WeaponData.Name;
			this.EditButton.IsEnabled = true;
			this.DeleteButton.IsEnabled = true;
		}

		private void UpdateWeaponNameInputVisibility()
		{
			if (this.IsEditing)
			{
				this.IsWeaponNameInputVisible = true;
				return;
			}
			WeaponDesignVMMixin weaponDesignVMMixinInstance = this.m_WeaponDesignVMMixinInstance;
			if (weaponDesignVMMixinInstance != null && weaponDesignVMMixinInstance.IsSavedWeaponsListVisible)
			{
				this.IsWeaponNameInputVisible = false;
				return;
			}
			this.IsWeaponNameInputVisible = this.m_CraftingSettings.SkipWeaponFinalizationPopup;
		}

		private void InitializeChildren()
		{
			this.ShowNewWeaponPopupToggle = new ButtonToggleVM(true)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Popup}Toggle off to skip the new crafted weapon popup dialog.", null).ToString()),
				SpriteAsStr = "bettersmithingcontinued_popup",
				IsToggledOn = !this.m_CraftingSettings.SkipWeaponFinalizationPopup
			};
			this.ShowNewWeaponPopupToggle.ToggleStateChanged += this.OnShowNewWeaponPopupToggleStateChanged;
			this.SaveButton = new TextButtonVM
			{
				IsVisible = true,
				TextButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Save}Save the currently designed weapon.", null).ToString()),
				TextValue = new TextObject("{=BSC_BT_Save}Save Design", null).ToString(),
				IsEnabled = true
			};
			this.SaveButton.ButtonPressed += this.OnSaveButtonPressed;
			this.CancelButton = new TextButtonVM
			{
				IsVisible = false,
				TextButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Cancel}Undo all changes done to current designed weapon.", null).ToString()),
				TextValue = new TextObject("{=BSC_BT_Cancel}Cancel", null).ToString(),
				IsEnabled = true
			};
			this.CancelButton.ButtonPressed += this.OnCancelButtonPressed;
			this.EditButton = new TextButtonVM
			{
				IsVisible = false,
				TextButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Edit}Edit the currently selected weapon design.", null).ToString()),
				DisabledButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_DBH_Edit}Select a weapon design to edit it.", null).ToString()),
				TextValue = new TextObject("{=BSC_BT_Edit}Edit Design", null).ToString(),
				IsEnabled = false
			};
			this.EditButton.ButtonPressed += this.OnEditButtonPressed;
			this.DeleteButton = new TextButtonVM
			{
				IsVisible = false,
				TextButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Delete}Delete the currently selected weapon design.", null).ToString()),
				DisabledButtonHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_DBH_Delete}Select a weapon design to delete it.", null).ToString()),
				TextValue = new TextObject("{=BSC_BT_Delete}Delete Design", null).ToString(),
				IsEnabled = false
			};
			this.DeleteButton.ButtonPressed += this.OnDeleteButtonPressed;
			this.SavedWeaponListToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Designs}Press to switch between Crafting and Saved Weapons List views.", null).ToString());
			this.SavedWeaponListToggleText = new TextObject("{=BSC_BT_Designs}Saved Designs", null).ToString();
		}

		private void OnCancelButtonPressed(object _sender, EventArgs _e)
		{
			this.IsEditing = false;
		}

		private void OnDeleteButtonPressed(object _sender, EventArgs _e)
		{
			this.m_WeaponSaveData.DeleteWeapon(this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItem.WeaponData, true);
			this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItem = null;
		}

		private void OnEditButtonPressed(object _sender, EventArgs _e)
		{
			this.IsEditing = true;
		}

		private void OnSaveButtonPressed(object _sender, EventArgs _e)
		{
			if (this.IsEditing)
			{
				this.m_WeaponDesignVMMixinInstance.CurrentlySelectedItem = null;
				this.m_WeaponSaveData.EditWeapon(Instances.SmithingManager.WeaponDesignVM.GetCraftingComponent(), this.m_EditTarget);
				this.IsEditing = false;
				return;
			}
			this.m_WeaponSaveData.SaveWeapon(Instances.SmithingManager.WeaponDesignVM.GetCraftingComponent());
		}

		private void OnShowNewWeaponPopupToggleStateChanged(object _sender, bool _e)
		{
			this.m_CraftingSettings.SkipWeaponFinalizationPopup = !_e;
			this.UpdateWeaponNameInputVisibility();
		}

		private void OnSettingsChanged(object _sender, SettingsSection _e)
		{
			CraftingSettings craftingSettings = _e as CraftingSettings;
			if (craftingSettings != null)
			{
				this.m_CraftingSettings = craftingSettings;
				return;
			}
			WeaponSaveData weaponSaveData = _e as WeaponSaveData;
			if (weaponSaveData != null)
			{
				this.m_WeaponSaveData = weaponSaveData;
			}
		}

		private void OnDefaultWeaponNameChanged()
		{
			string oldName = this.m_DefaultWeaponName;
			this.m_DefaultWeaponName = m_CraftingState.CraftingLogic.CraftedWeaponName.ToString();
			if (this.WeaponName == oldName || this.WeaponName == null)
			{
				this.WeaponName = this.m_DefaultWeaponName;
			}
			else if (this.WeaponName != null)
			{
				// Need to change WeaponDesignVM.ItemName back to this.WeaponName
				this.m_SmithingManager.WeaponDesignVM.ItemName = this.WeaponName;
			}
		}

		private readonly GauntletCraftingScreen m_ParentScreen;

		private CraftingState m_CraftingState;

		private CraftingSettings m_CraftingSettings;

		private ISettingsManager m_SettingsManager;

		private ButtonToggleVM m_ShowNewWeaponPopupToggle;

		private HintViewModel m_SavedWeaponListToggleHint;

		private TextButtonVM m_SaveButton;

		private TextButtonVM m_EditButton;

		private TextButtonVM m_DeleteButton;

		private TextButtonVM m_CancelButton;

		private WeaponSaveData m_WeaponSaveData;

		private WeaponData m_EditTarget;

		private bool m_IsEditing;

		private string m_WeaponName;

		private string m_DefaultWeaponName;

		private string m_SavedWeaponListToggleText;

		private ISmithingManager m_SmithingManager;

		private bool m_IsWeaponNameInputVisible;

		private WeaponDesignVMMixin m_WeaponDesignVMMixinInstance;
	}
}
