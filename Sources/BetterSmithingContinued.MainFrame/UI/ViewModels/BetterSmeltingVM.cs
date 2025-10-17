using System;

using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI.ViewModels.Templates;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public sealed class BetterSmeltingVM : ConnectedViewModel
	{
		[DataSourceProperty]
		public MBBindingList<CraftingResourceToggleVM> CraftingResourceToggles
		{
			get
			{
				return this.m_CraftingResourceToggles;
			}
			set
			{
				if (value != this.m_CraftingResourceToggles)
				{
					this.m_CraftingResourceToggles = value;
					base.OnPropertyChanged("CraftingResourceToggles");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel ResetAllHint
		{
			get
			{
				return this.m_ResetAllHint;
			}
			set
			{
				if (value != this.m_ResetAllHint)
				{
					this.m_ResetAllHint = value;
					base.OnPropertyChanged("ResetAllHint");
				}
			}
		}

		[DataSourceProperty]
		public ButtonToggleVM LockedItemsToggle
		{
			get
			{
				return this.m_LockedItemsToggle;
			}
			set
			{
				if (value != this.m_LockedItemsToggle)
				{
					this.m_LockedItemsToggle = value;
					base.OnPropertyChanged("LockedItemsToggle");
				}
			}
		}

		[DataSourceProperty]
		public ButtonToggleVM PlayerCraftedItemsToggle
		{
			get
			{
				return this.m_PlayerCraftedItemsToggle;
			}
			set
			{
				if (value != this.m_PlayerCraftedItemsToggle)
				{
					this.m_PlayerCraftedItemsToggle = value;
					base.OnPropertyChanged("PlayerCraftedItemsToggle");
				}
			}
		}

		[DataSourceProperty]
		public ButtonToggleVM NonPlayerCraftedItemsToggle
		{
			get
			{
				return this.m_NonPlayerCraftedItemsToggle;
			}
			set
			{
				if (value != this.m_NonPlayerCraftedItemsToggle)
				{
					this.m_NonPlayerCraftedItemsToggle = value;
					base.OnPropertyChanged("NonPlayerCraftedItemsToggle");
				}
			}
		}

		public BetterSmeltingVM(IPublicContainer _publicContainer, GauntletCraftingScreen _parentScreen) : base(_publicContainer)
		{
			this.m_ParentScreen = _parentScreen;
		}

		public override void Load()
		{
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_SettingsManager = base.PublicContainer.GetModule<ISettingsManager>("");
			this.m_SmeltingSettings = this.m_SettingsManager.GetSettings<SmeltingSettings>();
			this.m_IgnoreIsEnabledChanged = false;
			this.m_SettingsManager.SettingsSectionChanged += this.OnSettingsChanged;
			this.InitializeChildren();
			this.ReconstructSmeltableItemList();
			this.RefreshValues();
		}

		public override void Unload()
		{
			this.m_SettingsManager.SettingsSectionChanged -= this.OnSettingsChanged;
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			this.ResetAllHint.RefreshValues();
			this.LockedItemsToggle.RefreshValues();
			this.PlayerCraftedItemsToggle.RefreshValues();
			this.NonPlayerCraftedItemsToggle.RefreshValues();
			this.m_CraftingResourceToggles.ApplyActionOnAllItems(delegate(CraftingResourceToggleVM x) {
				x.RefreshValues();
			});
		}

		public void OnResetAllClicked()
		{
			this.m_SmeltingSettings.RestoreDefaults();
			this.m_IgnoreIsEnabledChanged = true;
			this.LockedItemsToggle.IsToggledOn = this.m_SmeltingSettings.DisplayLockedWeapons;
			this.PlayerCraftedItemsToggle.IsToggledOn = this.m_SmeltingSettings.DisplayPlayerCraftedItems;
			this.NonPlayerCraftedItemsToggle.IsToggledOn = this.m_SmeltingSettings.DisplayNonPlayerCraftedItems;
			foreach (CraftingResourceToggleVM craftingResourceToggleVM in this.m_CraftingResourceToggles)
			{
				craftingResourceToggleVM.IsToggledOn = this.m_SmeltingSettings.GetMaterialIsDisplayed(craftingResourceToggleVM.ResourceMaterial);
			}
			this.m_IgnoreIsEnabledChanged = false;
			this.ReconstructSmeltableItemList();
		}

		private void OnSettingsChanged(object _sender, SettingsSection _e)
		{
			if (_e is SmeltingSettings smeltingSettings)
			{
				this.m_SmeltingSettings = smeltingSettings;
			}
		}

		private void InitializeChildren()
		{
			this.ResetAllHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Reset}Press this to reset all toggles to their default states.", null).ToString());
			this.LockedItemsToggle = new ButtonToggleVM(false)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_Locked}Toggle on to include Locked Weapons.", null).ToString()),
				SpriteAsStr = "bettersmithingcontinued_lock",
				IsToggledOn = this.m_SmeltingSettings.DisplayLockedWeapons
			};
			this.LockedItemsToggle.ToggleStateChanged += this.OnLockedItemsToggleStateChanged;
			this.PlayerCraftedItemsToggle = new ButtonToggleVM(true)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_PCW}Toggle on to include Player Crafted weapons.", null).ToString()),
				SpriteAsStr = "SPGeneral\\MapOverlay\\Settlement\\icon_loyalty",
				IsToggledOn = this.m_SmeltingSettings.DisplayPlayerCraftedItems
			};
			this.PlayerCraftedItemsToggle.ToggleStateChanged += this.OnPlayerCraftedItemsToggleStateChanged;
			this.NonPlayerCraftedItemsToggle = new ButtonToggleVM(true)
			{
				ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_NPCW}Toggle on to include Non-Player Crafted weapons.", null).ToString()),
				SpriteAsStr = "SPGeneral\\MapOverlay\\Settlement\\icon_security",
				IsToggledOn = this.m_SmeltingSettings.DisplayNonPlayerCraftedItems
			};
			this.NonPlayerCraftedItemsToggle.ToggleStateChanged += this.OnNonPlayerCraftedItemsToggleStateChanged;
			this.CraftingResourceToggles = new MBBindingList<CraftingResourceToggleVM>();
			for (int i = 0; i < (int) CraftingMaterials.NumCraftingMats; i++)
			{
				CraftingMaterials craftingMaterials = (CraftingMaterials)i;
				if (craftingMaterials != CraftingMaterials.Charcoal && craftingMaterials != CraftingMaterials.IronOre)
				{
					CraftingResourceToggleVM craftingResourceToggleVM = new CraftingResourceToggleVM(craftingMaterials)
					{
						IsToggledOn = this.m_SmeltingSettings.GetMaterialIsDisplayed(craftingMaterials)
					};
					craftingResourceToggleVM.ToggleStateChanged += this.OnCraftingResourceIsEnabledChanged;
					this.CraftingResourceToggles.Add(craftingResourceToggleVM);
				}
			}
		}

		private void OnLockedItemsToggleStateChanged(object _sender, bool _isToggledOn)
		{
			if (this.m_IgnoreIsEnabledChanged)
			{
				return;
			}
			this.m_SmeltingSettings.DisplayLockedWeapons = _isToggledOn;
			this.ReconstructSmeltableItemList();
		}

		private void OnPlayerCraftedItemsToggleStateChanged(object _sender, bool _isToggledOn)
		{
			if (this.m_IgnoreIsEnabledChanged)
			{
				return;
			}
			this.m_SmeltingSettings.DisplayPlayerCraftedItems = _isToggledOn;
			this.ReconstructSmeltableItemList();
		}

		private void OnNonPlayerCraftedItemsToggleStateChanged(object _sender, bool _isToggledOn)
		{
			if (this.m_IgnoreIsEnabledChanged)
			{
				return;
			}
			this.m_SmeltingSettings.DisplayNonPlayerCraftedItems = _isToggledOn;
			this.ReconstructSmeltableItemList();
		}

		private void OnCraftingResourceIsEnabledChanged(object _sender, bool _isToggledOn)
		{
			if (this.m_IgnoreIsEnabledChanged)
			{
				return;
			}
			if (_sender is CraftingResourceToggleVM craftingResourceToggleVM)
			{
				if (Input.IsKeyDown(InputKey.LeftControl))
				{
					this.m_IgnoreIsEnabledChanged = true;
					this.m_SmeltingSettings.SetMaterialIsDisplayed(craftingResourceToggleVM.ResourceMaterial, true);
					craftingResourceToggleVM.IsToggledOn = true;
					foreach (CraftingResourceToggleVM craftingResourceToggleVM2 in this.CraftingResourceToggles)
					{
						if (craftingResourceToggleVM2.ResourceMaterial != craftingResourceToggleVM.ResourceMaterial)
						{
							craftingResourceToggleVM2.IsToggledOn = false;
							this.m_SmeltingSettings.SetMaterialIsDisplayed(craftingResourceToggleVM2.ResourceMaterial, false);
						}
					}
					this.m_IgnoreIsEnabledChanged = false;
				}
				else
				{
					this.m_SmeltingSettings.SetMaterialIsDisplayed(craftingResourceToggleVM.ResourceMaterial, _isToggledOn);
				}
				this.ReconstructSmeltableItemList();
			}
		}

		private void ReconstructSmeltableItemList()
		{
			try
			{
				this.m_SmithingManager.SmeltingItemRoster.UpdateFilter(new Func<EquipmentElement, bool>(this.ItemIsVisible));
			}
			catch (Exception)
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_EM_09}Item Filtering caused an issue.", null).ToString());
			}
		}

		private bool ItemIsVisible(EquipmentElement _equipmentElement)
		{
			ItemObject item = _equipmentElement.Item;
			if (!item.IsCraftedWeapon)
			{
				return false;
			}
			SmithingModel smithingModel = Campaign.Current.Models.SmithingModel;
			int[] array = (smithingModel != null) ? smithingModel.GetSmeltingOutputForItem(item) : null;
			if (array == null)
			{
				return false;
			}
			if (!this.m_SmeltingSettings.DisplayPlayerCraftedItems && item.IsCraftedByPlayer)
			{
				return false;
			}
			if (!this.m_SmeltingSettings.DisplayNonPlayerCraftedItems && !item.IsCraftedByPlayer)
			{
				return false;
			}
			if (!this.m_SmeltingSettings.DisplayLockedWeapons && this.m_SmithingManager.SmeltingItemRoster.IsItemLocked(_equipmentElement))
			{
				return false;
			}
			for (int i = 0; i < (int) CraftingMaterials.NumCraftingMats; i++)
			{
				if (array[i] > 0 && this.m_SmeltingSettings.GetMaterialIsDisplayed((CraftingMaterials)i))
				{
					return true;
				}
			}
			return false;
		}

		private readonly GauntletCraftingScreen m_ParentScreen;

		private MBBindingList<CraftingResourceToggleVM> m_CraftingResourceToggles;

		private ButtonToggleVM m_PlayerCraftedItemsToggle;

		private ButtonToggleVM m_NonPlayerCraftedItemsToggle;

		private SmeltingSettings m_SmeltingSettings;

		private ISmithingManager m_SmithingManager;

		private ISettingsManager m_SettingsManager;

		private bool m_IgnoreIsEnabledChanged;

		private ButtonToggleVM m_LockedItemsToggle;

		private HintViewModel m_ResetAllHint;
	}
}
