using System;
using System.Collections.Generic;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;

namespace BetterSmithingContinued.MainFrame.UI
{
	public class BetterSmithingUIContext : Module, IBetterSmithingUIContext
	{
		public bool IsInNormalCraftingScreen { get; set; }

		public bool IsEditableTextWidgetFocused { get; set; }

		public void DeferAction(Action _action, int _ticks)
		{
			this.m_DeferredActions.Add(new BetterSmithingUIContext.DeferredAction(_action, _ticks));
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.RegisterModule<IBetterSmithingUIContext>("");
			Instances.BetterSmithingUIContext = this;
			this.IsEditableTextWidgetFocused = false;
			this.IsInNormalCraftingScreen = true;
			this.m_DeferredActions = new List<BetterSmithingUIContext.DeferredAction>();
		}

		public override void Load()
		{
			base.Load();
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_SmithingManager.LeavingSmithingMenu += this.OnLeavingSmithingMenu;
			this.m_SubModuleEventNotifier = base.PublicContainer.GetModule<ISubModuleEventNotifier>("");
			this.m_SubModuleEventNotifier.GameTick += this.OnGameTick;
		}

		public override void Unload()
		{
			this.m_SmithingManager.LeavingSmithingMenu -= this.OnLeavingSmithingMenu;
			this.m_SubModuleEventNotifier.GameTick -= this.OnGameTick;
			base.Unload();
		}

		private void OnGameTick(object _sender, float _e)
		{
			List<BetterSmithingUIContext.DeferredAction> list = new List<BetterSmithingUIContext.DeferredAction>();
			foreach (BetterSmithingUIContext.DeferredAction deferredAction in this.m_DeferredActions)
			{
				if (deferredAction.Update())
				{
					list.Add(deferredAction);
				}
			}
			foreach (BetterSmithingUIContext.DeferredAction deferredAction2 in list)
			{
				deferredAction2.Action();
				this.m_DeferredActions.Remove(deferredAction2);
			}
		}

		private void OnLeavingSmithingMenu(object _sender, EventArgs _e)
		{
			this.IsEditableTextWidgetFocused = false;
			this.IsInNormalCraftingScreen = true;
		}

		private ISmithingManager m_SmithingManager;

		private ISubModuleEventNotifier m_SubModuleEventNotifier;

		private List<BetterSmithingUIContext.DeferredAction> m_DeferredActions;

		private class DeferredAction
		{
			public Action Action { get; }

			public int TicksRemaining { get; private set; }

			public DeferredAction(Action _action, int _ticksRemaining)
			{
				this.Action = _action;
				this.TicksRemaining = _ticksRemaining;
			}

			public bool Update()
			{
				int ticksRemaining = this.TicksRemaining;
				this.TicksRemaining = ticksRemaining - 1;
				return ticksRemaining <= 0;
			}
		}
	}
}
