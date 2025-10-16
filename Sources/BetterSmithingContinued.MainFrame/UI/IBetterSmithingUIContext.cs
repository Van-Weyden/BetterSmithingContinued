using System;

namespace BetterSmithingContinued.MainFrame.UI
{
	public interface IBetterSmithingUIContext
	{
		bool IsEditableTextWidgetFocused { get; set; }

		bool IsInNormalCraftingScreen { get; set; }

		void DeferAction(Action _action, int _ticks);
	}
}
