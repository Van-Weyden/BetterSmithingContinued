using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using BetterSmithingContinued.MainFrame.Patches;
using BetterSmithingContinued.MainFrame.Utilities;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using MCM.Abstractions.Base.Global;
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

namespace BetterSmithingContinued.MainFrame
{
	public sealed class CraftingRepeater : Module, ICraftingRepeater
	{
        public void OnWeaponCrafted(ItemObject weapon, ItemModifier weaponModifier)
        {
            if (GlobalSettings<MCMBetterSmithingSettings>.Instance?.GroupIdenticalCraftedWeapons ?? false)
            {
                weapon = MobileParty.MainParty.ItemRoster?.CompressIdenticalCraftedWeapons(weapon, weaponModifier);
            }
            Instances.SmithingManager.SmeltingItemRoster.ModifyItem(new EquipmentElement(weapon, weaponModifier), 1);

            ItemQuality weaponQuality = weaponModifier?.ItemQuality ?? ItemQuality.Common;
            if (!this.m_WeaponQualityCounts.ContainsKey(weaponQuality))
            {
                this.m_WeaponQualityCounts.Add(weaponQuality, 0);
            }
            this.m_WeaponQualityCounts[weaponQuality]++;
        }

        public void InitMultiCrafting(Hero hero, WeaponDesign weaponDesign)
        {
            m_hero = hero;
            m_weaponDesign = weaponDesign;
            m_desiredOperationCount = this.GetDesiredOperationCount();
        }

        public void DoMultiCrafting(ICraftingCampaignBehavior craftingBehavior)
        {
            if (m_hero == null || m_weaponDesign == null)
			{
				return;
			}

			CraftingState craftingState = GameStateManager.Current.ActiveState as CraftingState;
			if (craftingState == null)
			{
				return;
			}

            try
			{
				ItemObject craftedWeapon = craftingState.CraftingLogic.GetCurrentCraftedItemObject(false);
                ItemModifier craftedWeaponModifier = craftingBehavior.GetCurrentItemModifier();

                // DoMultiCrafting calls in the ExecuteFinalizeCrafting so we already crafted at least one weapon
				int crafted = 1;
                int total = Math.Min(m_desiredOperationCount, this.GetMaxCraftingCount(m_weaponDesign) + 1); // +1 because we spend material for 1st weapon already
                OnWeaponCrafted(craftedWeapon, craftedWeaponModifier); // Process that first crafted weapon

                if (ScreenManager.TopScreen != null)
                {
                    while (crafted < total && HaveEnergy(craftingBehavior, m_hero, craftedWeapon))
					{
						craftedWeaponModifier = Campaign.Current.Models.SmithingModel.GetCraftedWeaponModifier(m_weaponDesign, m_hero);
						craftingBehavior.SetCurrentItemModifier(craftedWeaponModifier);
						craftedWeapon = craftingBehavior.CreateCraftedWeaponInFreeBuildMode(m_hero, m_weaponDesign, craftedWeaponModifier);
                        OnWeaponCrafted(craftedWeapon, craftedWeaponModifier);
                        crafted++;
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

		public bool HaveEnergy(ICraftingCampaignBehavior craftingBehavior, Hero hero, ItemObject item)
		{
			return (
				this.m_SmithingManager.CraftingVM.SmartHaveEnergy() || 
				this.GetCraftingStaminaCost(hero, item) <= craftingBehavior.GetHeroCraftingStamina(hero)
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
        private Hero m_hero = null;
        private WeaponDesign m_weaponDesign = null;

        private int m_desiredOperationCount = 0;
    }
}
