using System;
using BetterSmithingContinued.MainFrame.Utilities;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame
{
	public interface IScreenSwitcher
	{
		CraftingGauntletScreen CraftingGauntletScreen { get; set; }

		void UpdateCurrentCraftingSubVM(CraftingScreen _currentCraftingScreen);

		event EventHandler<CraftingGauntletScreen> CraftingGauntletScreenUpdated;
	}
}
