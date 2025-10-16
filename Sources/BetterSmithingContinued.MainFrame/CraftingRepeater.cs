using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using MCM.Abstractions.Base.Global;
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

namespace BetterSmithingContinued.MainFrame
{
	public sealed class CraftingRepeater : Module, ICraftingRepeater
	{
		public void AddWeaponTierType()
		{
			WeaponTier weaponTier = WeaponTierUtils.GetWeaponTier(this.m_SmithingManager.LastSmithedWeaponTier);
			if (!this.m_WeaponTierCounts.ContainsKey(weaponTier))
			{
				this.m_WeaponTierCounts.Add(weaponTier, 0);
			}
			Dictionary<WeaponTier, int> weaponTierCounts = this.m_WeaponTierCounts;
			WeaponTier weaponTier2 = weaponTier;
			WeaponTier key = weaponTier2;
			int num = weaponTierCounts[key];
			weaponTierCounts[key] = num + 1;
		}

		public void DoMultiCrafting(ref CraftingCampaignBehavior __instance, Hero hero, WeaponDesign weaponDesign)
		{
			try
			{
				int desiredOperationCount = this.GetDesiredOperationCount();
				CraftingState craftingState;
				if (desiredOperationCount != -1 && (craftingState = (GameStateManager.Current.ActiveState as CraftingState)) != null)
				{
					ItemObject currentCraftedItemObject = craftingState.CraftingLogic.GetCurrentCraftedItemObject(false, null);
					int num = Math.Min(desiredOperationCount, this.GetMaxCraftingCount(weaponDesign) + 1);
					if (num != 0)
					{
						int num2 = Math.Min(desiredOperationCount, num);
						int num3 = 1;
						if (ScreenManager.TopScreen != null)
						{
							while (num3 < num2 && (this.m_SmithingManager.CraftingVM.SmartHaveEnergy() || this.GetCraftingStaminaCost(hero, currentCraftedItemObject) <= __instance.GetHeroCraftingStamina(hero)))
							{
								int modifierTierForSmithedWeapon = Campaign.Current.Models.SmithingModel.GetModifierTierForSmithedWeapon(weaponDesign, hero);
								Crafting.OverrideData modifierChanges = Campaign.Current.Models.SmithingModel.GetModifierChanges(modifierTierForSmithedWeapon, hero, currentCraftedItemObject.GetWeaponWithUsageIndex(0));
								__instance.CreateCraftedWeaponInFreeBuildMode(hero, weaponDesign, modifierTierForSmithedWeapon, modifierChanges);
								num3++;
							}
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

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.m_WeaponTierCounts = new Dictionary<WeaponTier, int>();
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
			int num = this.m_WeaponTierCounts.Sum((KeyValuePair<WeaponTier, int> x) => x.Value);
			StringBuilder stringBuilder = new StringBuilder(new TextObject("{=BSC_Msg_CW}Crafted {COUNT} weapons", null).SetTextVariable("COUNT", num).ToString());
			bool flag = true;
			foreach (WeaponTier weaponTier in WeaponTierUtils.GetWeaponTierOrderedList())
			{
				if (this.m_WeaponTierCounts.TryGetValue(weaponTier, out num) && num > 0)
				{
					stringBuilder.Append(flag ? ":" : ",");
					flag = false;
					stringBuilder.Append(string.Format(" {0} {1}", num, WeaponTierUtils.GetWeaponTierPrefix(weaponTier, true)));
				}
			}
			InformationManager.DisplayMessage(new InformationMessage(stringBuilder.ToString()));
			this.m_WeaponTierCounts.Clear();
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
						if (num == 0)
						{
							num = int.MaxValue;
						}
					}
					else if (Input.IsKeyDown(InputKey.LeftShift))
					{
						num = instance.ShiftCraftOperationCount;
					}
					else if (Input.IsKeyDown(InputKey.LeftControl))
					{
						num = instance.ControlCraftOperationCount;
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

		private Dictionary<WeaponTier, int> m_WeaponTierCounts;

		private ISmithingManager m_SmithingManager;
	}
}
