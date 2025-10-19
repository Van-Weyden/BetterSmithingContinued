using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ScreenSystem;

using MCM.Abstractions.Base.Global;

using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.MainFrame
{
	public sealed class CraftingRepeater : Module, ICraftingRepeater
	{
		public void AddWeaponTierType()
		{
			ItemQuality weaponQuality = this.m_SmithingManager.LastSmithedWeaponModifier?.ItemQuality ?? ItemQuality.Common;
			if (!this.m_WeaponQualityCounts.ContainsKey(weaponQuality))
			{
				this.m_WeaponQualityCounts.Add(weaponQuality, 0);
			}
			this.m_WeaponQualityCounts[weaponQuality]++;
		}

		public void DoMultiCrafting(ref CraftingCampaignBehavior __instance, Hero hero, WeaponDesign weaponDesign)
		{
			try
			{
				int desiredOperationCount = this.GetDesiredOperationCount();
				CraftingState craftingState;
				if (desiredOperationCount > 0 && (craftingState = (GameStateManager.Current.ActiveState as CraftingState)) != null)
				{
					ItemObject currentCraftedItemObject = craftingState.CraftingLogic.GetCurrentCraftedItemObject(false);
					int total = Math.Min(desiredOperationCount, this.GetMaxCraftingCount(weaponDesign));
					int crafted = 1; // DoMultiCrafting calls in the CreateCraftedWeaponInFreeBuildModePostfix so we already crafted at least one weapon
                    if (ScreenManager.TopScreen != null)
					{
						while (crafted < total && HaveEnergy(ref __instance, hero, currentCraftedItemObject))
						{
							ItemModifier craftedWeaponModifier = Campaign.Current.Models.SmithingModel.GetCraftedWeaponModifier(weaponDesign, hero);
							__instance.SetCurrentItemModifier(craftedWeaponModifier);
							__instance.CreateCraftedWeaponInFreeBuildMode(hero, weaponDesign, craftedWeaponModifier);
							crafted++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_EM_02}Error occurred while trying to do multiple crafting operations. Exception: ", null) + ex.Message));
			}
			finally
			{
				this.DisplayCraftingMessage();
			}
		}

		public bool HaveEnergy(ref CraftingCampaignBehavior __instance, Hero hero, ItemObject item)
		{
			return (
				this.m_SmithingManager.CraftingVM.SmartHaveEnergy() || 
				this.GetCraftingStaminaCost(hero, item) <= __instance.GetHeroCraftingStamina(hero)
			);
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.m_WeaponQualityCounts = new Dictionary<ItemQuality, int>();
			this.RegisterModule<ICraftingRepeater>("");
		}

		public override void Load()
		{
			Instances.CraftingRepeater = this;
			this.m_SmithingManager = base.PublicContainer.GetModule<ISmithingManager>("");
		}

		public override void Unload()
		{
			Instances.CraftingRepeater = null;
		}

		private void DisplayCraftingMessage()
		{
			int num = this.m_WeaponQualityCounts.Sum((KeyValuePair<ItemQuality, int> x) => x.Value);
			StringBuilder stringBuilder = new StringBuilder(new TextObject("{=BSC_Msg_CW}Crafted {COUNT} weapons", null).SetTextVariable("COUNT", num).ToString());
			bool flag = true;
			foreach (ItemQuality weaponTier in WeaponTierUtils.GetWeaponTierOrderedList())
			{
				if (this.m_WeaponQualityCounts.TryGetValue(weaponTier, out num) && num > 0)
				{
					stringBuilder.Append(flag ? ":" : ",");
					flag = false;
					stringBuilder.Append(string.Format(" {0} {1}", num, WeaponTierUtils.GetWeaponTierPrefix(weaponTier, true)));
				}
			}
			InformationManager.DisplayMessage(new InformationMessage(stringBuilder.ToString()));
			this.m_WeaponQualityCounts.Clear();
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
						num = instance.ControlShiftCraftOperationCount;
					}
					else if (Input.IsKeyDown(InputKey.LeftShift))
					{
						num = instance.ShiftCraftOperationCount;
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num = instance.ControlCraftOperationCount;
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

		private int GetMaxCraftingCount(WeaponDesign weaponDesign)
		{
			ItemRoster itemRoster = MobileParty.MainParty.ItemRoster;
			int num = int.MaxValue;
			int[] smithingCostsForWeaponDesign = Campaign.Current.Models.SmithingModel.GetSmithingCostsForWeaponDesign(weaponDesign);
			for (int i = 0; i < smithingCostsForWeaponDesign.Length; i++)
			{
				if (smithingCostsForWeaponDesign[i] < 0)
				{
					int val = 0;
					int itemNumber = itemRoster.GetItemNumber(Campaign.Current.Models.SmithingModel.GetCraftingMaterialItem((CraftingMaterials)i));
					if (itemNumber > 0)
					{
						val = itemNumber / Math.Abs(smithingCostsForWeaponDesign[i]);
					}
					num = Math.Min(num, val);
				}
			}
			return num;
		}

		private int GetCraftingStaminaCost(Hero hero, ItemObject currentCraftedItemObject)
		{
			return Campaign.Current.Models.SmithingModel.GetEnergyCostForSmithing(currentCraftedItemObject, hero);
		}

		private Dictionary<TaleWorlds.Core.ItemQuality, int> m_WeaponQualityCounts;

		private ISmithingManager m_SmithingManager;
	}
}
