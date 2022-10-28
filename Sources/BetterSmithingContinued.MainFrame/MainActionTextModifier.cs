using System;
using System.Runtime.CompilerServices;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame
{
	public class MainActionTextModifier : ConnectedObject
	{
		public MainActionTextModifier(IPublicContainer _publicContainer) : base(_publicContainer)
		{
		}

		public new void Load()
		{
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
			this.m_SubModuleEventNotifier = base.PublicContainer.GetModule<ISubModuleEventNotifier>("");
			this.m_SubModuleEventNotifier.GameTick += this.OnGameTick;
		}

		public new void Unload()
		{
			this.m_SubModuleEventNotifier.GameTick -= this.OnGameTick;
		}

		private void OnGameTick(object _sender, float _e)
		{
			string mainActionText = "";
			switch (this.m_SmithingManager.CurrentCraftingScreen)
			{
			case CraftingScreen.None:
				return;
			case CraftingScreen.Smelting:
				if (Input.IsKeyDown(GlobalSettings<MCMBetterSmithingSettings>.Instance.SmeltAllKey.SelectedValue.Key))
				{
					mainActionText = MainActionTextModifier.m_DefaultSmeltingMainActionString.Value + " " + new TextObject("{=BSC_Msg_All}All", null);
				}
				else if (Input.IsKeyDown(GlobalSettings<MCMBetterSmithingSettings>.Instance.SmeltMultipleKey.SelectedValue.Key))
				{
					mainActionText = MainActionTextModifier.m_DefaultSmeltingMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.SmeltMultipleCount);
				}
				else
				{
					mainActionText = MainActionTextModifier.m_DefaultSmeltingMainActionString.Value;
				}
				break;
			case CraftingScreen.Crafting:
				if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift))
				{
					mainActionText = MainActionTextModifier.m_DefaultCraftingMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ControlShiftCraftOperationCount);
				}
				else if (Input.IsKeyDown(InputKey.LeftControl))
				{
					mainActionText = MainActionTextModifier.m_DefaultCraftingMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ControlCraftOperationCount);
				}
				else if (Input.IsKeyDown(InputKey.LeftShift))
				{
					mainActionText = MainActionTextModifier.m_DefaultCraftingMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ShiftCraftOperationCount);
				}
				else
				{
					mainActionText = MainActionTextModifier.m_DefaultCraftingMainActionString.Value;
				}
				break;
			case CraftingScreen.Refining:
				if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift))
				{
					mainActionText = MainActionTextModifier.m_DefaultRefiningMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ControlShiftRefineOperationCount);
				}
				else if (Input.IsKeyDown(InputKey.LeftControl))
				{
					mainActionText = MainActionTextModifier.m_DefaultRefiningMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ControlRefineOperationCount);
				}
				else if (Input.IsKeyDown(InputKey.LeftShift))
				{
					mainActionText = MainActionTextModifier.m_DefaultRefiningMainActionString.Value + " " + MainActionTextModifier.GetStringFromCount(GlobalSettings<MCMBetterSmithingSettings>.Instance.ShiftRefineOperationCount);
				}
				else
				{
					mainActionText = MainActionTextModifier.m_DefaultRefiningMainActionString.Value;
				}
				break;
			}
			this.m_SmithingManager.CraftingVM.MainActionText = mainActionText;
		}

		[CompilerGenerated]
		internal static string GetStringFromCount(int _count)
		{
			if (_count != 0)
			{
				return _count.ToString();
			}
			return new TextObject("{=BSC_Msg_Max}Max", null).ToString();
		}

		private static readonly Lazy<string> m_DefaultCraftingMainActionString = new Lazy<string>(() => GameTexts.FindText("str_crafting_category_crafting", null).ToString());

		private static readonly Lazy<string> m_DefaultSmeltingMainActionString = new Lazy<string>(() => GameTexts.FindText("str_crafting_category_smelting", null).ToString());

		private static readonly Lazy<string> m_DefaultRefiningMainActionString = new Lazy<string>(() => GameTexts.FindText("str_crafting_category_refinement", null).ToString());

		private ISmithingManager m_SmithingManager;

		private ISubModuleEventNotifier m_SubModuleEventNotifier;
	}
}
