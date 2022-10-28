using System;
using System.Diagnostics;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;

using MCM.Abstractions.Base.Global;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame
{
	public sealed class SmeltingRepeater : Module, ISmeltingRepeater
	{
		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.RegisterModule<ISmeltingRepeater>("");
		}

		public override void Load()
		{
			Instances.SmeltingRepeater = this;
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
		}

		public override void Unload()
		{
			Instances.SmeltingRepeater = null;
		}

		public void DoSmelting(ref CraftingCampaignBehavior _instance, Hero _hero, EquipmentElement _equipmentElement, int _startIndex)
		{
			int num = 1;
			try
			{
				MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
				if (Input.IsKeyDown((instance != null) ? instance.SmeltMultipleKey.SelectedValue.Key : InputKey.LeftControl))
				{
					num = this.SmeltMultiple(ref _instance, _hero, _equipmentElement, _startIndex);
				}
				else
				{
					MCMBetterSmithingSettings instance2 = GlobalSettings<MCMBetterSmithingSettings>.Instance;
					if (Input.IsKeyDown((instance2 != null) ? instance2.SmeltAllKey.SelectedValue.Key : InputKey.LeftShift))
					{
						num = this.SmeltAll(ref _instance, _hero, _equipmentElement, _startIndex);
					}
				}
			}
			catch (Exception ex)
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_EM_01}Error occurred while trying to do smelting operation. Exception: ", null) + ex.Message);
			}
			finally
			{
				Messaging.DisplayMessage(new TextObject("{=BSC_Msg_IS}Smelted {COUNT} {?IS_PLURAL}weapons{?}weapon{\\?}.", null).SetTextVariable("COUNT", num).SetTextVariable("IS_PLURAL", (num == 1) ? 0 : 1).ToString());
			}
		}

		private int SmeltAll(ref CraftingCampaignBehavior _instance, Hero _hero, EquipmentElement _equipmentElement, int _startIndex)
		{
			int num = 1;
			try
			{
				this.m_SmithingManager.SmeltingItemRoster.StoreCurrentSelectedItemIndex();
				MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
				bool flag = instance == null || instance.SmartSmeltingEnabled;
				bool isCraftedByPlayer = _equipmentElement.Item.IsCraftedByPlayer;
				EquipmentElement equipmentElement = _equipmentElement;
				int num3;
				int num2 = this.m_SmithingManager.SmeltingItemRoster.GetItemIndex(equipmentElement, out num3);
				if (num2 == -1)
				{
					num2 = _startIndex;
				}
				IL_FB:
				while (equipmentElement.Item != null)
				{
					for (int i = 0; i < num3; i++)
					{
						if (!this.m_SmithingManager.CraftingVM.SmartHaveEnergy() && this.GetSmeltingCost(_hero, _equipmentElement) > _instance.GetHeroCraftingStamina(_hero))
						{
							return num;
						}
						if (this.GetMaxSmeltCount() <= 0)
						{
							return num;
						}
						num++;
						_instance.DoSmelting(_hero, equipmentElement);
					}
					for (;;)
					{
						equipmentElement = this.m_SmithingManager.SmeltingItemRoster.GetItemAtIndex(num2, out num3);
						if (num3 < 0)
						{
							break;
						}
						if (equipmentElement.Item != null && (!flag || isCraftedByPlayer == equipmentElement.Item.IsCraftedByPlayer))
						{
							goto IL_FB;
						}
						num2++;
					}
					return num;
				}
			}
			catch (Exception value)
			{
				Trace.WriteLine(value);
			}
			finally
			{
				this.m_SmithingManager.SmeltingItemRoster.RestoreCurrentSelectedItemIndex();
			}
			return num;
		}

		private int SmeltMultiple(ref CraftingCampaignBehavior _instance, Hero _hero, EquipmentElement _equipmentElement, int _startIndex)
		{
			int num = 1;
			int num2;
			if (this.m_SmithingManager.SmeltingItemRoster.GetItemIndex(_equipmentElement, out num2) < 0)
			{
				return num;
			}
			MCMBetterSmithingSettings instance = GlobalSettings<MCMBetterSmithingSettings>.Instance;
			int num3 = (instance != null) ? instance.SmeltMultipleCount : 0;
			if (num3 == 0)
			{
				num3 = int.MaxValue;
			}
			int num4 = Math.Min(Math.Min(num3, num2 + 1), this.GetMaxSmeltCount() + 1);
			while (num < num4 && (this.m_SmithingManager.CraftingVM.SmartHaveEnergy() || this.GetSmeltingCost(_hero, _equipmentElement) <= _instance.GetHeroCraftingStamina(_hero)))
			{
				_instance.DoSmelting(_hero, _equipmentElement);
				num++;
			}
			return num;
		}

		private int GetSmeltingCost(Hero hero, EquipmentElement equipmentElement)
		{
			return Campaign.Current.Models.SmithingModel.GetEnergyCostForSmelting(equipmentElement.Item, hero);
		}

		private int GetMaxSmeltCount()
		{
			return MobileParty.MainParty.ItemRoster.GetItemNumber(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem(CraftingMaterials.Charcoal));
		}

		private ISmithingManager m_SmithingManager;
	}
}
