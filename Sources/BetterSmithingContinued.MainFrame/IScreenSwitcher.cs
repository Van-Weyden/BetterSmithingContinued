using System;

using SandBox.GauntletUI;

using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Utilities;

namespace BetterSmithingContinued.MainFrame
{
	public interface IScreenSwitcher
	{
		GauntletCraftingScreen GauntletCraftingScreen { get; set; }
		ConnectedViewModel ConnectedViewModel(CraftingScreen screen);

        void UpdateCurrentCraftingSubVM(CraftingScreen _currentCraftingScreen);

		event EventHandler<GauntletCraftingScreen> GauntletCraftingScreenUpdated;
	}
}
