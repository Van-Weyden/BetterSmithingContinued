using System;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using SandBox.GauntletUI;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public class BetterSmithingSettingsVM : ConnectedViewModel
	{
		public BetterSmithingSettingsVM(IPublicContainer _publicContainer, CraftingGauntletScreen _parent, Action _closeSettingsScreen) : base(_publicContainer)
		{
			this.m_Parent = _parent;
			this.m_CloseSettingsScreen = _closeSettingsScreen;
		}

		private readonly CraftingGauntletScreen m_Parent;

		private readonly Action m_CloseSettingsScreen;
	}
}
