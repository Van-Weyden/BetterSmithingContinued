using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Persistence;
using BetterSmithingContinued.MainFrame.UI.ViewModels;
using BetterSmithingContinued.Utilities;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.UI
{
	[ViewModelMixin]
	public sealed class WeaponDesignVMMixin : BaseViewModelMixin<WeaponDesignVM>
	{
		public event EventHandler<CraftingItemTemplateVM> CurrentlySelectedItemChanged;

		public event EventHandler ViewModeChanged;

		public static event EventHandler<WeaponDesignVMMixin> InstanceChanged;

		public static WeaponDesignVMMixin Instance
		{
			get
			{
				return WeaponDesignVMMixin.m_Instance;
			}
			private set
			{
				if (WeaponDesignVMMixin.m_Instance != value)
				{
					WeaponDesignVMMixin.m_Instance = value;
					WeaponDesignVMMixin.OnInstanceChanged(WeaponDesignVMMixin.m_Instance);
				}
			}
		}

		[DataSourceProperty]
		public CraftingItemTemplateVM CurrentlySelectedItem
		{
			get
			{
				return this.m_CurrentlySelectedItem;
			}
			set
			{
				if (this.m_CurrentlySelectedItem != value)
				{
					if (this.m_CurrentlySelectedItem != null)
					{
						this.m_CurrentlySelectedItem.IsSelected = false;
						this.m_CurrentlySelectedItem.RefreshValues();
					}
					this.m_CurrentlySelectedItem = value;
					if (this.m_CurrentlySelectedItem != null)
					{
						this.m_CurrentlySelectedItem.IsSelected = true;
						this.m_CurrentlySelectedItem.WeaponData.ApplyWeaponData(base.ViewModel);
						this.m_CurrentlySelectedItem.RefreshValues();
					}
					this.OnCurrentlySelectedItemChanged(this.m_CurrentlySelectedItem);
				}
			}
		}

		[DataSourceProperty]
		public bool IsDefaultCraftingMenuVisible
		{
			get
			{
				return this.m_IsDefaultCraftingMenuVisible;
			}
			set
			{
				if (this.m_IsDefaultCraftingMenuVisible != value)
				{
					this.m_IsDefaultCraftingMenuVisible = value;
					this.m_IsSavedWeaponsListVisible = !value;
					this.OnPropertyChanged("IsDefaultCraftingMenuVisible");
					WeaponDesignVM viewModel = base.ViewModel;
					if (viewModel != null)
					{
						viewModel.OnPropertyChanged("IsSavedWeaponsListVisible");
					}
					this.OnViewModeChanged();
				}
			}
		}

		[DataSourceProperty]
		public bool IsCategorySelectionEnabled
		{
			get
			{
				return this.m_IsCategorySelectionEnabled;
			}
			set
			{
				if (this.m_IsCategorySelectionEnabled != value)
				{
					this.m_IsCategorySelectionEnabled = value;
					WeaponDesignVM viewModel = base.ViewModel;
					if (viewModel != null)
					{
						viewModel.OnPropertyChangedWithValue(this.m_IsCategorySelectionEnabled, "IsCategorySelectionEnabled");
					}
					WeaponDesignVM viewModel2 = base.ViewModel;
					if (viewModel2 != null)
					{
						viewModel2.OnPropertyChangedWithValue(this.IsCategorySelectionDisabled, "IsCategorySelectionDisabled");
					}
					GauntletLayer gauntletLayer = this.m_ScreenSwitcher.GauntletCraftingScreen.GetGauntletLayer();
					if (gauntletLayer == null)
					{
						return;
					}
					gauntletLayer.UIContext.EventManager.PerformActionOnWidget(delegate(ListPanel _widget)
					{
						if (_widget.Id == "CraftingFilters")
						{
							Widget[] array = _widget.ParentWidget.AllChildren.ToArray<Widget>();
							int i = array.IndexOf(_widget);
							if (i >= 0)
							{
								while (i < array.Count<Widget>())
								{
									if (array[i].Id == "ModeSelection")
									{
										array[i].IsVisible = this.m_IsCategorySelectionEnabled;
										return true;
									}
									i++;
								}
							}
						}
						return false;
					});
				}
			}
		}

		[DataSourceProperty]
		public bool IsCategorySelectionDisabled
		{
			get
			{
				return !this.m_IsCategorySelectionEnabled;
			}
		}

		[DataSourceProperty]
		public bool IsSavedWeaponsListVisible
		{
			get
			{
				return this.m_IsSavedWeaponsListVisible;
			}
			set
			{
				if (this.m_IsSavedWeaponsListVisible != value)
				{
					this.m_IsSavedWeaponsListVisible = value;
					this.m_IsDefaultCraftingMenuVisible = !value;
					this.OnPropertyChanged("IsSavedWeaponsListVisible");
					WeaponDesignVM viewModel = base.ViewModel;
					if (viewModel != null)
					{
						viewModel.OnPropertyChanged("IsDefaultCraftingMenuVisible");
					}
					this.OnViewModeChanged();
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<CraftingItemTemplateVM> SavedItemList
		{
			get
			{
				if (this.m_SavedItemList == null)
				{
					this.UpdateSavedItemList();
				}
				return this.m_SavedItemList;
			}
			set
			{
				if (this.m_SavedItemList != value)
				{
					this.m_SavedItemList = value;
					WeaponDesignVM viewModel = base.ViewModel;
					if (viewModel == null)
					{
						return;
					}
					viewModel.OnPropertyChanged("SavedItemList");
				}
			}
		}

		public WeaponDesignVMMixin(WeaponDesignVM weaponDesign) : base(weaponDesign)
		{
			WeaponDesignVMMixin.Instance = this;
			this.m_WeaponSaveData = Instances.SettingsManager.GetSettings<WeaponSaveData>();
			this.m_ScreenSwitcher = Instances.ScreenSwitcher;
			this.m_WeaponSaveData.WeaponsListUpdated += this.OnWeaponsListUpdated;
			weaponDesign.PropertyChangedWithValue += this.OnWeaponDesignVMPropertyChanged;
			if (this.m_ScreenSwitcher.GauntletCraftingScreen != null)
			{
				this.OnGauntletCraftingScreenUpdated(null, this.m_ScreenSwitcher.GauntletCraftingScreen);
				return;
			}
			this.m_ScreenSwitcher.GauntletCraftingScreenUpdated += this.OnGauntletCraftingScreenUpdated;
		}

		public override void OnFinalize()
		{
			if (base.ViewModel != null)
			{
				base.ViewModel.PropertyChangedWithValue -= this.OnWeaponDesignVMPropertyChanged;
			}
			WeaponDesignVMMixin.Instance = null;
			this.m_WeaponSaveData.WeaponsListUpdated -= this.OnWeaponsListUpdated;
			if (this.m_ScreenSwitcher != null)
			{
				try
				{
					this.m_ScreenSwitcher.GauntletCraftingScreenUpdated -= this.OnGauntletCraftingScreenUpdated;
				}
				catch (Exception)
				{
				}
			}
			base.OnFinalize();
		}

		private void OnWeaponDesignVMPropertyChanged(object _sender, PropertyChangedWithValueEventArgs _propertyChangedWithValueEventArgs)
		{
			if (_propertyChangedWithValueEventArgs.PropertyName == "CurrentCraftedWeaponTemplateId")
			{
				this.UpdateSavedItemList((string)_propertyChangedWithValueEventArgs.Value);
			}
		}

		private void OnGauntletCraftingScreenUpdated(object _sender, GauntletCraftingScreen _e)
		{
			if (_e != null)
			{
				this.IsDefaultCraftingMenuVisible = !Instances.BetterSmithingUIContext.IsInNormalCraftingScreen;
				this.IsDefaultCraftingMenuVisible = Instances.BetterSmithingUIContext.IsInNormalCraftingScreen;
				this.IsCategorySelectionEnabled = true;
				this.OnCurrentlySelectedItemChanged(this.CurrentlySelectedItem);
			}
		}

		private void OnWeaponsListUpdated(object _sender, EventArgs _e)
		{
			this.UpdateSavedItemList();
		}

		private void UpdateSavedItemList()
		{
			string text;
			this.UpdateSavedItemList(MemberExtractor.GetPropertyValue(base.ViewModel, "CurrentCraftedWeaponTemplateId", out text));
		}

		private void UpdateSavedItemList(string weaponClassId)
		{
			IEnumerable<WeaponData> weapons = this.m_WeaponSaveData.Weapons;
			Func<WeaponData, bool> predicate = (WeaponData x) => x.Id == weaponClassId;
			MBBindingList<CraftingItemTemplateVM> mbbindingList = new MBBindingList<CraftingItemTemplateVM>();
			foreach (WeaponData weaponData in weapons.Where(predicate))
			{
				CraftingItemTemplateVM item = new CraftingItemTemplateVM(weaponData, new Action<CraftingItemTemplateVM>(this.SetCurrentItem));
				mbbindingList.Add(item);
			}
			this.SavedItemList = mbbindingList;
		}

		private void SetCurrentItem(CraftingItemTemplateVM _currentSelectedItem)
		{
			this.CurrentlySelectedItem = _currentSelectedItem;
		}

		private new void OnPropertyChanged([CallerMemberName] string _callerName = "")
		{
			WeaponDesignVM viewModel = base.ViewModel;
			if (viewModel == null)
			{
				return;
			}
			viewModel.OnPropertyChanged(_callerName);
		}

		private void OnCurrentlySelectedItemChanged(CraftingItemTemplateVM _e)
		{
			EventHandler<CraftingItemTemplateVM> currentlySelectedItemChanged = this.CurrentlySelectedItemChanged;
			if (currentlySelectedItemChanged == null)
			{
				return;
			}
			currentlySelectedItemChanged(this, _e);
		}

		private void OnViewModeChanged()
		{
			EventHandler viewModeChanged = this.ViewModeChanged;
			if (viewModeChanged == null)
			{
				return;
			}
			viewModeChanged(this, EventArgs.Empty);
		}

		private static void OnInstanceChanged(WeaponDesignVMMixin _e)
		{
			EventHandler<WeaponDesignVMMixin> instanceChanged = WeaponDesignVMMixin.InstanceChanged;
			if (instanceChanged == null)
			{
				return;
			}
			instanceChanged(null, _e);
		}

		private static WeaponDesignVMMixin m_Instance;

		private bool m_IsDefaultCraftingMenuVisible;

		private bool m_IsSavedWeaponsListVisible;

		private MBBindingList<CraftingItemTemplateVM> m_SavedItemList;

		private CraftingItemTemplateVM m_CurrentlySelectedItem;

		private WeaponSaveData m_WeaponSaveData;

		private bool m_IsCategorySelectionEnabled;

		private IScreenSwitcher m_ScreenSwitcher;
	}
}
