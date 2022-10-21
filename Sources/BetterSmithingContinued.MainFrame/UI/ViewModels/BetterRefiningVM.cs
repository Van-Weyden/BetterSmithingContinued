using System;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public class BetterRefiningVM : ConnectedViewModel
	{
		public BetterRefiningVM(IPublicContainer _publicContainer, GauntletCraftingScreen _parentScreen) : base(_publicContainer)
		{
			this.m_ParentScreen = _parentScreen;
		}

		private readonly GauntletCraftingScreen m_ParentScreen;
	}
}
