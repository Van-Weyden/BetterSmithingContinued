using System;
using System.Runtime.CompilerServices;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

using MCM.Abstractions.Base.Global;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame
{
	public sealed class RefiningRepeater : Module, IRefiningRepeater
	{
		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.RegisterModule<IRefiningRepeater>("");
		}

		public override void Load()
		{
			Instances.RefiningRepeater = this;
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
		}

		public override void Unload()
		{
			Instances.RefiningRepeater = null;
		}

		public void DoRefinement(ref CraftingCampaignBehavior __instance, Hero hero, Crafting.RefiningFormula refineFormula)
		{
			int num = Math.Min(this.GetDesiredOperationCount(), this.GetMaxRefiningCount(refineFormula) + 1);
			if (num == 0)
			{
				return;
			}
			int num2 = 1;
			if (ScreenManager.TopScreen == null)
			{
				return;
			}
			try
			{
				while (num2 < num && (this.m_SmithingManager.CraftingVM.SmartHaveEnergy() || this.GetRefinementCost(hero, refineFormula) <= __instance.GetHeroCraftingStamina(hero)))
				{
					__instance.DoRefinement(hero, refineFormula);
					num2++;
				}
			}
			catch (Exception ex)
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_EM_03}Error occurred while trying to do multiple refine operations. Exception: ", null) + ex.Message);
			}
			finally
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_Msg_IR}Refined {COUNT} {?IS_PLURAL}times{?}time{\\?}.", null).SetTextVariable("COUNT", num2).SetTextVariable("IS_PLURAL", (num2 == 1) ? 0 : 1).ToString());
			}
		}

		private int GetDesiredOperationCount()
		{
			int num = -1;
			if (Input.IsKeyDown(InputKey.LeftControl) || Input.IsKeyDown(InputKey.LeftShift))
			{
				MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
				if (instance != null)
				{
					if (Input.IsKeyDown(InputKey.LeftControl) && Input.IsKeyDown(InputKey.LeftShift))
					{
						num = instance.ControlShiftRefineOperationCount;
					}
					else if (Input.IsKeyDown(InputKey.LeftShift))
					{
						num = instance.ShiftRefineOperationCount;
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num = instance.ControlRefineOperationCount;
					}

					if (num == 0)
					{
						num = int.MaxValue;
					}
				}
				else
				{
					num = 1;
				}
			}
			return num;
		}

		private int GetRefinementCost(Hero hero, Crafting.RefiningFormula refineFormula)
		{
			return Campaign.Current.Models.SmithingModel.GetEnergyCostForRefining(ref refineFormula, hero);
		}

		private int GetMaxRefiningCount(Crafting.RefiningFormula refineFormula)
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			int num = int.MaxValue;
			if (refineFormula.Input1Count > 0)
			{
				int val = RefiningRepeater.MaxForInput(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Input1), refineFormula.Input1Count, ref itemRoster);
				num = Math.Min(num, val);
			}
			if (refineFormula.Input2Count > 0)
			{
				int val2 = RefiningRepeater.MaxForInput(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(refineFormula.Input2), refineFormula.Input2Count, ref itemRoster);
				return Math.Min(num, val2);
			}
			return num;
		}

		[CompilerGenerated]
		internal static int MaxForInput(ItemObject _inputItem, int _inputCount, ref ItemRoster itemRoster)
		{
			int itemNumber = itemRoster.GetItemNumber(_inputItem);
			if (itemNumber <= 0)
			{
				return 0;
			}
			return itemNumber / Math.Abs(_inputCount);
		}

		private ISmithingManager m_SmithingManager;
	}
}
