using TaleWorlds.InputSystem;

using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using MCM.Common;

namespace BetterSmithingContinued.Settings
{
	public class MCMBetterSmithingSettings : AttributeGlobalSettings<MCMBetterSmithingSettings>
	{
		[SettingPropertyInteger("{=BSC_SPN_11}Ctrl Refine", 0, 1000, "0", RequireRestart = false, Order = 1, HintText = "{=BSC_SPH_11}The number of refining operations to perform when pressing \"Refine\" while holding the left Ctrl key. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_01}Refining", GroupOrder = 2)]
		public int ControlRefineOperationCount { get; set; } = 5;

		[SettingPropertyInteger("{=BSC_SPN_12}Shift Refine", 0, 1000, "0", RequireRestart = false, Order = 2, HintText = "{=BSC_SPH_12}The number of refining operations to perform when pressing \"Refine\" while holding the left Shift key. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_01}Refining")]
		public int ShiftRefineOperationCount { get; set; } = 50;

		[SettingPropertyInteger("{=BSC_SPN_13}Ctrl + Shift Refine", 0, 1000, "0", RequireRestart = false, Order = 3, HintText = "{=BSC_SPH_13}The number of refining operations to perform when pressing \"Refine\" while holding the left Ctrl + left Shift keys. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_01}Refining")]
		public int ControlShiftRefineOperationCount { get; set; }

		[SettingPropertyDropdown("{=BSC_SPN_21}Smelt Multiple Key", RequireRestart = false, Order = 1, HintText = "{=BSC_SPH_21}The key to hold when pressing the \"Smelt\" Button to perform a Smelt Multiple operation. Smelt Multiple will only smelt the currently selected equipment.")]
		[SettingPropertyGroup("{=BSC_SPG_02}Smelting", GroupOrder = 3)]
		public Dropdown<KeybindingDropdownOption> SmeltMultipleKey { get; set; } = KeybindingDropdown.GetKeybindingDropdownOptions(InputKey.LeftControl);

		[SettingPropertyInteger("{=BSC_SPN_22}Smelt Multiple Count", 0, 1000, "0", RequireRestart = false, Order = 2, HintText = "{=BSC_SPH_22}The number of smelting operations to perform when pressing \"Smelt\" while holding the key specified in the \"Smelt Multiple\" setting. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_02}Smelting")]
		public int SmeltMultipleCount { get; set; }

		[SettingPropertyDropdown("{=BSC_SPN_23}Smelt All Key", RequireRestart = false, Order = 3, HintText = "{=BSC_SPH_23}The key to hold when pressing the \"Smelt\" Button to perform a Smelt All operation. Smelt All will smelt all unlocked equipment starting from the top of the equipment list.")]
		[SettingPropertyGroup("{=BSC_SPG_02}Smelting")]
		public Dropdown<KeybindingDropdownOption> SmeltAllKey { get; set; } = KeybindingDropdown.GetKeybindingDropdownOptions(InputKey.LeftShift);

		[SettingPropertyBool("{=BSC_SPN_24}Enable Smart Smelting", RequireRestart = false, Order = 4, HintText = "{=BSC_SPH_24}If set to true, when doing a \"Smelt All\" operation, will only smelt either Player Crafted weapons, or Non-Player Crafted weapons, depending on the selected piece of equipment when the button is pressed.")]
		[SettingPropertyGroup("{=BSC_SPG_02}Smelting")]
		public bool SmartSmeltingEnabled { get; set; } = true;

		[SettingPropertyInteger("{=BSC_SPN_31}Ctrl Craft", 0, 1000, "0", RequireRestart = false, Order = 1, HintText = "{=BSC_SPH_31}The number of crafting operations to perform when pressing \"Craft\" while holding the left Ctrl key. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting", GroupOrder = 4)]
		public int ControlCraftOperationCount { get; set; } = 5;

		[SettingPropertyInteger("{=BSC_SPN_32}Shift Craft", 0, 1000, "0", RequireRestart = false, Order = 2, HintText = "{=BSC_SPH_32}The number of crafting operations to perform when pressing \"Craft\" while holding the left Shift key. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting")]
		public int ShiftCraftOperationCount { get; set; } = 50;

		[SettingPropertyInteger("{=BSC_SPN_33}Ctrl + Shift Craft", 0, 1000, "0", RequireRestart = false, Order = 3, HintText = "{=BSC_SPH_33}The number of crafting operations to perform when pressing \"Craft\" while holding the left Ctrl + left Shift keys. Set to 0 for Maximum.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting")]
		public int ControlShiftCraftOperationCount { get; set; }

		[SettingPropertyBool("{=BSC_SPN_34}Show Only Unlocked Pieces by Default", RequireRestart = false, Order = 4, HintText = "{=BSC_SPH_34}If Enabled, toggle button \"Show only unlocked pieces\" will be enabled by default.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting")]
		public bool ShowUnlockedPiecesByDefault { get; set; }

		[SettingPropertyBool("{=BSC_SPN_35}Group Identical Crafted Weapons", RequireRestart = false, Order = 5, HintText = "{=BSC_SPH_35}If Enabled, will group all identical crafted weapons (Weapons with the same name, same type, same stats, etc) together.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting")]
		public bool GroupIdenticalCraftedWeapons { get; set; } = true;

		[SettingPropertyBool("{=BSC_SPN_36}Add Weapon Tier Prefixes", RequireRestart = false, Order = 6, HintText = "{=BSC_SPH_36}If Enabled, weapon names will be prefixed with their appropriate tiers (Legendary(3), Masterwork(2), Fine(1), Crude(-1), Rusty(-2), Broken(-3) ). 0 Tier weapons don't get a prefix.")]
		[SettingPropertyGroup("{=BSC_SPG_03}Crafting")]
		public bool AddWeaponTierPrefixes { get; set; } = true;

		[SettingPropertyBool("{=BSC_SPN_41}Use Crafting Stamina", IsToggle = true, RequireRestart = false, Order = 1, HintText = "{=BSC_SPH_41}Disable this option for unlimited crafting stamina.")]
		[SettingPropertyGroup("{=BSC_SPG_04}Crafting stamina", GroupOrder = 5)]
		public bool CraftingStaminaEnabled { get; set; } = true;

		[SettingPropertyFloatingInteger("{=BSC_SPN_42}Recovery rate inside towns", 0f, 10f, "#0%", Order = 2, RequireRestart = false, HintText = "{=BSC_SPH_42}Crafting stamina recovery rate multipler when party rest inside towns. 100% means the same as vanilla recovery rate in towns; 0% means that recovery inside towns is disabled.")]
		[SettingPropertyGroup("{=BSC_SPG_04}Crafting stamina")]
		public float CraftingStaminaRecoveryRateInsideTowns { get; set; } = 1f;

		[SettingPropertyFloatingInteger("{=BSC_SPN_43}Recovery rate outside towns", 0f, 10f, "#0%", Order = 3, RequireRestart = false, HintText = "{=BSC_SPH_43}Crafting stamina recovery rate multipler when party is outside towns. 100% means the same as vanilla recovery rate in towns; 0% means that recovery outside towns is disabled.")]
		[SettingPropertyGroup("{=BSC_SPG_04}Crafting stamina")]
		public float CraftingStaminaRecoveryRateOutsideTowns { get; set; }

		public override string Id
		{
			get
			{
				return "BetterSmithingContinued";
			}
		}

		public override string DisplayName
		{
			get
			{
				return "Better Smithing Continued";
			}
		}

		public override string FolderName
		{
			get
			{
				return "BetterSmithingContinued";
			}
		}

		public override string FormatType
		{
			get
			{
				return "xml";
			}
		}
	}
}
