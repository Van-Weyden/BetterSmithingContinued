using System;
using BetterSmithingContinued.MainFrame.UI.ViewModels.Templates;
using BetterSmithingContinued.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public class CraftingResourceToggleVM : ButtonToggleVM
	{
		public CraftingMaterials ResourceMaterial { get; }

		[DataSourceProperty]
		public string ResourceName
		{
			get
			{
				return this.m_ResourceName;
			}
			set
			{
				if (value == this.m_ResourceName)
				{
					return;
				}
				this.m_ResourceName = value;
				base.OnPropertyChangedWithValue(value, "ResourceName");
			}
		}

		[DataSourceProperty]
		public string ResourceMaterialTypeAsStr
		{
			get
			{
				return this.m_ResourceMaterialTypeAsStr;
			}
			set
			{
				if (value == this.m_ResourceMaterialTypeAsStr)
				{
					return;
				}
				this.m_ResourceMaterialTypeAsStr = value;
				base.OnPropertyChangedWithValue(value, "ResourceMaterialTypeAsStr");
			}
		}

		[DataSourceProperty]
		public string ResourceItemStringId
		{
			get
			{
				return this.m_ResourceItemStringId;
			}
			set
			{
				if (value == this.m_ResourceItemStringId)
				{
					return;
				}
				this.m_ResourceItemStringId = value;
				base.OnPropertyChangedWithValue(value, "ResourceItemStringId");
			}
		}

		public CraftingResourceToggleVM(CraftingMaterials material) : base(true)
		{
			this.ResourceMaterial = material;
			Campaign campaign = Campaign.Current;
			ItemObject itemObject;
			if (campaign == null)
			{
				itemObject = null;
			}
			else
			{
				GameModels models = campaign.Models;
				if (models == null)
				{
					itemObject = null;
				}
				else
				{
					SmithingModel smithingModel = models.SmithingModel;
					itemObject = ((smithingModel != null) ? smithingModel.GetCraftingMaterialItem(material) : null);
				}
			}
			ItemObject itemObject2 = itemObject;
			string text;
			if (itemObject2 == null)
			{
				text = null;
			}
			else
			{
				TextObject name = itemObject2.Name;
				text = ((name != null) ? name.ToString() : null);
			}
			this.ResourceName = (text ?? "none");
			base.ToggleHint = HintViewModelUtilities.CreateHintViewModel(new TextObject("{=BSC_BH_IWGM}Toggle on to include weapons that give at least one ", null).ToString() + this.ResourceName + ".");
			this.ResourceItemStringId = (((itemObject2 != null) ? itemObject2.StringId : null) ?? "none");
			this.ResourceMaterialTypeAsStr = this.ResourceMaterial.ToString();
			base.SpriteAsStr = this.GetSpriteString();
		}

		private string GetSpriteString()
		{
			switch (this.ResourceMaterial)
			{
			case CraftingMaterials.IronOre:
			case CraftingMaterials.NumCraftingMats:
				return "Crafting\\crafting_materials_big_iron_ore";
			case CraftingMaterials.Iron1:
				return "Crafting\\crafting_materials_big_crudeiron";
			case CraftingMaterials.Iron2:
				return "Crafting\\crafting_materials_big_compositeiron";
			case CraftingMaterials.Iron3:
				return "Crafting\\crafting_materials_big_iron";
			case CraftingMaterials.Iron4:
				return "Crafting\\crafting_materials_big_steel";
			case CraftingMaterials.Iron5:
				return "Crafting\\crafting_materials_big_refinedsteel";
			case CraftingMaterials.Iron6:
				return "Crafting\\crafting_materials_big_calradiansteel";
			case CraftingMaterials.Wood:
				return "Crafting\\crafting_materials_big_hardwood";
			case CraftingMaterials.Charcoal:
				return "Crafting\\crafting_materials_big_charcoal";
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private string m_ResourceName;

		private string m_ResourceItemStringId;

		private string m_ResourceMaterialTypeAsStr;
	}
}
