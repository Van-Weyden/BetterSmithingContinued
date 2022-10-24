using System;
using System.Collections.Generic;
using System.Linq;
using BetterSmithingContinued.Core.Modules;
using HarmonyLib;

namespace BetterSmithingContinued.Core
{
	public class SubModuleEventNotifier : Module, ISubModuleEventNotifier
	{
		public event EventHandler BeforeInitialModuleScreenSetAsRoot;

		public event EventHandler<float> GameTick;

		public event EventHandler<Harmony> PerformManualPatches;

		public void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription)
		{
			if (this.m_OnModulesLoadedSubscribers.Any((ModuleLoadedSubscription moduleLoadedSubscription) => moduleLoadedSubscription.HandlesSubscription(_subscription)))
			{
				return;
			}
			this.m_OnModulesLoadedSubscribers.Add(_subscription);
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.m_OnModulesLoadedSubscribers = new List<ModuleLoadedSubscription>();
			this.RegisterModule<ISubModuleEventNotifier>("");
		}

		public void OnBeforeInitialModuleScreenSetAsRoot()
		{
			EventHandler beforeInitialModuleScreenSetAsRoot = this.BeforeInitialModuleScreenSetAsRoot;
			if (beforeInitialModuleScreenSetAsRoot == null)
			{
				return;
			}
			beforeInitialModuleScreenSetAsRoot(this, EventArgs.Empty);
		}

		public void OnGameTick(float _e)
		{
			EventHandler<float> gameTick = this.GameTick;
			if (gameTick == null)
			{
				return;
			}
			gameTick(this, _e);
		}

		public void OnPerformManualPatches(Harmony _e)
		{
			EventHandler<Harmony> performManualPatches = this.PerformManualPatches;
			if (performManualPatches == null)
			{
				return;
			}
			performManualPatches(this, _e);
		}

		public void OnModulesLoaded()
		{
			foreach (ModuleLoadedSubscription moduleLoadedSubscription in this.m_OnModulesLoadedSubscribers)
			{
				moduleLoadedSubscription.CallModulesLoaded();
			}
		}

		private List<ModuleLoadedSubscription> m_OnModulesLoadedSubscribers;
	}
}
