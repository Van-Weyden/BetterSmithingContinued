using System;
using System.Collections.Generic;
using BetterSmithingContinued.MainFrame.Patches;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.InputSystem;

namespace BetterSmithingContinued.MainFrame.UI.Widgets
{
	public class BetterSmithingContinuedEditableTextWidget : EditableTextWidget
	{
		public BetterSmithingContinuedEditableTextWidget(UIContext context) : base(context)
		{
		}

		public override void HandleInput(IReadOnlyList<int> lastKeysPressed)
		{
			if (Input.IsKeyPressed(InputKey.Enter))
			{
				base.Context.EventManager.ClearFocus();
				return;
			}
			base.HandleInput(lastKeysPressed);
		}

		protected override void OnMouseReleased()
		{
			base.OnMouseReleased();
		}

		protected override void OnGainFocus()
		{
			base.OnGainFocus();
			Instances.BetterSmithingUIContext.IsEditableTextWidgetFocused = true;
		}

		protected override void OnLoseFocus()
		{
			base.OnLoseFocus();
			Instances.BetterSmithingUIContext.IsEditableTextWidgetFocused = false;
		}
	}
}
