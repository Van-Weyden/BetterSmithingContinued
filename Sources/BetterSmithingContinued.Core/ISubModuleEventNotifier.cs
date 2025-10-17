using System;
using HarmonyLib;

namespace BetterSmithingContinued.Core
{
	public interface ISubModuleEventNotifier
	{
		event EventHandler BeforeInitialModuleScreenSetAsRoot;

		event EventHandler<float> GameTick;

		event EventHandler<Harmony> PerformManualPatches;

		void SubscribeToModulesLoaded(ModuleLoadedSubscription _subscription);
	}
}
