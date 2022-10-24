﻿using System;
using System.Collections.Generic;
using System.Reflection;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.UI.ViewModels;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using MCM.Abstractions.Settings.Base.Global;
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace BetterSmithingContinued.MainFrame
{
	public sealed class ScreenSwitcher : BetterSmithingContinued.Core.Modules.Module, IScreenSwitcher
	{
		public event EventHandler<CraftingGauntletScreen> CraftingGauntletScreenUpdated;

		public CraftingGauntletScreen CraftingGauntletScreen
		{
			get
			{
				return this.m_CraftingGauntletScreen;
			}
			set
			{
				if (this.m_CraftingGauntletScreen != value)
				{
					if (value == null)
					{
						this.ExitSmithingScreen();
						this.m_CraftingGauntletScreen = null;
					}
					else
					{
						this.m_CraftingGauntletScreen = value;
						this.EnterSmithingScreen();
					}
					this.OnCraftingGauntletScreenUpdated(this.m_CraftingGauntletScreen);
				}
			}
		}

		public void UpdateCurrentCraftingSubVM(CraftingScreen _currentCraftingScreen)
		{
			if (this.CraftingGauntletScreen == null)
			{
				this.m_CurrentCraftingScreen = _currentCraftingScreen;
				return;
			}
			if (this.m_CurrentCraftingScreen == _currentCraftingScreen && this.m_CurrentScreenLayer != null)
			{
				return;
			}
			this.m_CurrentCraftingScreen = _currentCraftingScreen;
			if (this.m_CurrentScreenLayer != null)
			{
				if (this.m_CurrentMovie != null)
				{
					this.m_CurrentScreenLayer.ReleaseMovie(this.m_CurrentMovie);
				}
				CraftingGauntletScreen craftingGauntletScreen = this.CraftingGauntletScreen;
				if (craftingGauntletScreen != null)
				{
					craftingGauntletScreen.RemoveLayer(this.m_CurrentScreenLayer);
				}
			}
			this.m_CurrentScreenLayer = this.UpdateScreen(_currentCraftingScreen);
			CraftingGauntletScreen craftingGauntletScreen2 = this.CraftingGauntletScreen;
			if (craftingGauntletScreen2 != null)
			{
				craftingGauntletScreen2.AddLayer(this.m_CurrentScreenLayer);
			}
			CraftingVM craftingVM = this.m_SmithingManager.CraftingVM;
			if (craftingVM != null)
			{
				craftingVM.SmartRefreshEnabledMainAction();
			}
			this.m_SmithingManager.CurrentCraftingScreen = this.m_CurrentCraftingScreen;
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.RegisterModule<IScreenSwitcher>("");
		}

		public override void Load()
		{
			Instances.ScreenSwitcher = this;
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_SubModuleEventNotifier = base.PublicContainer.GetModule<ISubModuleEventNotifier>("");
		}

		public override void Unload()
		{
			Instances.ScreenSwitcher = null;
		}

		private void OnGameTick(object _sender, float _e)
		{
			if (this.m_WeaponPreviewSceneLayer != null)
			{
				bool mouseVisibility = !this.m_WeaponPreviewSceneLayer.Input.IsHotKeyDown("Rotate") && !this.m_WeaponPreviewSceneLayer.Input.IsHotKeyDown("Zoom");
				this.m_BetterSmithingScreenLayer.InputRestrictions.SetMouseVisibility(mouseVisibility);
				this.m_CurrentScreenLayer.InputRestrictions.SetMouseVisibility(mouseVisibility);
			}
		}

		private void EnterSmithingScreen()
		{
			this.m_CharacterDeveloperSpriteCategory = UIResourceManager.SpriteData.SpriteCategories["ui_characterdeveloper"];
			this.m_CharacterDeveloperSpriteCategory.Load(UIResourceManager.ResourceContext, UIResourceManager.UIResourceDepot);
			this.UpdateCurrentCraftingSubVM(this.m_CurrentCraftingScreen);
			this.m_BetterSmithingViewModel = new BetterSmithingVM(base.PublicContainer, this.CraftingGauntletScreen);
			this.m_BetterSmithingViewModel.Load();
			this.m_BetterSmithingScreenLayer = new GauntletLayer(50, "GauntletLayer", false);
			this.m_BetterSmithingMovie = this.m_BetterSmithingScreenLayer.LoadMovie("BetterSmithingScreen", this.m_BetterSmithingViewModel);
			this.m_BetterSmithingScreenLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			this.CraftingGauntletScreen.AddLayer(this.m_BetterSmithingScreenLayer);
			FieldInfo value = ScreenSwitcher.m_LazySceneLayerFieldInfo.Value;
			this.m_WeaponPreviewSceneLayer = (SceneLayer)((value != null) ? value.GetValue(this.CraftingGauntletScreen) : null);
			this.m_SubModuleEventNotifier.GameTick += this.OnGameTick;
			ISmithingManager smithingManager = this.m_BetterSmithingViewModel.SmithingManager();
			WeaponDesignVM weaponDesignVM;
			if (smithingManager == null)
			{
				weaponDesignVM = null;
			}
			else
			{
				CraftingVM craftingVM = smithingManager.CraftingVM;
				weaponDesignVM = ((craftingVM != null) ? craftingVM.WeaponDesign : null);
			}
			WeaponDesignVM weaponDesignVM2 = weaponDesignVM;
			MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
			if (weaponDesignVM2 != null && instance != null && instance.ShowUnlockedPiecesByDefault && !weaponDesignVM2.ShowOnlyUnlockedPieces)
			{
				weaponDesignVM2.ExecuteToggleShowOnlyUnlockedPieces();
			}
			ISmithingManager smithingManager2 = this.m_BetterSmithingViewModel.SmithingManager();
			if (smithingManager2 == null)
			{
				return;
			}
			CraftingAvailableHeroItemVM currentCraftingHero = smithingManager2.CurrentCraftingHero;
			if (currentCraftingHero == null)
			{
				return;
			}
			currentCraftingHero.ExecuteSelection();
		}

		private void ExitSmithingScreen()
		{
			foreach (KeyValuePair<CraftingScreen, ConnectedViewModel> keyValuePair in this.m_ViewModels)
			{
				ConnectedViewModel value = keyValuePair.Value;
				if (value != null)
				{
					value.OnFinalize();
				}
			}
			if (this.m_CurrentScreenLayer != null)
			{
				if (this.m_CurrentMovie != null)
				{
					this.m_CurrentScreenLayer.ReleaseMovie(this.m_CurrentMovie);
				}
				this.CraftingGauntletScreen.RemoveLayer(this.m_CurrentScreenLayer);
			}
			this.m_BetterSmithingScreenLayer.ReleaseMovie(this.m_BetterSmithingMovie);
			BetterSmithingVM betterSmithingViewModel = this.m_BetterSmithingViewModel;
			if (betterSmithingViewModel != null)
			{
				betterSmithingViewModel.OnFinalize();
			}
			this.CraftingGauntletScreen.RemoveLayer(this.m_BetterSmithingScreenLayer);
			this.m_BetterSmithingScreenLayer = null;
			this.m_BetterSmithingViewModel = null;
			this.m_BetterSmithingMovie = null;
			this.m_CharacterDeveloperSpriteCategory.Unload();
			this.m_CharacterDeveloperSpriteCategory = null;
			this.m_ViewModels.Clear();
			this.m_CurrentMovie = null;
			this.m_CurrentScreenLayer = null;
			this.m_SubModuleEventNotifier.GameTick -= this.OnGameTick;
			this.m_WeaponPreviewSceneLayer = null;
			this.m_CurrentCraftingScreen = CraftingScreen.None;
		}

		private GauntletLayer UpdateScreen(CraftingScreen _currentCraftingScreen)
		{
			ConnectedViewModel connectedViewModel;
			if (!this.m_ViewModels.TryGetValue(_currentCraftingScreen, out connectedViewModel))
			{
				ConnectedViewModel connectedViewModel2;
				switch (_currentCraftingScreen)
				{
				case CraftingScreen.Smelting:
					connectedViewModel2 = new BetterSmeltingVM(base.PublicContainer, this.CraftingGauntletScreen);
					break;
				case CraftingScreen.Crafting:
					connectedViewModel2 = new BetterCraftingVM(base.PublicContainer, this.CraftingGauntletScreen);
					break;
				case CraftingScreen.Refining:
					connectedViewModel2 = new BetterRefiningVM(base.PublicContainer, this.CraftingGauntletScreen);
					break;
				default:
					connectedViewModel2 = null;
					break;
				}
				connectedViewModel = connectedViewModel2;
				if (connectedViewModel != null)
				{
					connectedViewModel.Load();
				}
				this.m_ViewModels.Add(_currentCraftingScreen, connectedViewModel);
			}
			GauntletLayer gauntletLayer = new GauntletLayer(51, "GauntletLayer", false);
			this.m_CurrentMovie = gauntletLayer.LoadMovie("Better" + Enum.GetName(typeof(CraftingScreen), _currentCraftingScreen) + "Screen", connectedViewModel);
			gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			return gauntletLayer;
		}

		private void OnCraftingGauntletScreenUpdated(CraftingGauntletScreen _e)
		{
			EventHandler<CraftingGauntletScreen> craftingGauntletScreenUpdated = this.CraftingGauntletScreenUpdated;
			if (craftingGauntletScreenUpdated == null)
			{
				return;
			}
			craftingGauntletScreenUpdated(this, _e);
		}

		private static readonly Lazy<FieldInfo> m_LazySceneLayerFieldInfo = new Lazy<FieldInfo>(() => typeof(CraftingGauntletScreen).GetField("_sceneLayer", MemberExtractor.PrivateMemberFlags));

		private readonly Dictionary<CraftingScreen, ConnectedViewModel> m_ViewModels = new Dictionary<CraftingScreen, ConnectedViewModel>();

		private GauntletLayer m_CurrentScreenLayer;

		private CraftingGauntletScreen m_CraftingGauntletScreen;

		private IGauntletMovie m_CurrentMovie;

		private CraftingScreen m_CurrentCraftingScreen;

		private BetterSmithingVM m_BetterSmithingViewModel;

		private GauntletLayer m_BetterSmithingScreenLayer;

		private IGauntletMovie m_BetterSmithingMovie;

		private ISmithingManager m_SmithingManager;

		private ISubModuleEventNotifier m_SubModuleEventNotifier;

		private SceneLayer m_WeaponPreviewSceneLayer;

		private SpriteCategory m_CharacterDeveloperSpriteCategory;
	}
}
