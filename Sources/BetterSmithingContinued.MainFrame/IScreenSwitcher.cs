using System;
using BetterSmithingContinued.MainFrame.Utilities;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame
{
	public interface IScreenSwitcher
	{
		GauntletCraftingScreen GauntletCraftingScreen { get; set; }

		void UpdateCurrentCraftingSubVM(CraftingScreen _currentCraftingScreen);

		event EventHandler<GauntletCraftingScreen> GauntletCraftingScreenUpdated;
	}
}
