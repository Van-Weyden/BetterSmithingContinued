using System;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using BetterSmithingContinued.Settings;
using TaleWorlds.Core;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("SmeltingSettings")]
	public class SmeltingSettings : SettingsSection<SmeltingSettings>
	{
		[XmlElement("DisplayLockedWeapons")]
		[SettingDefaultValue(false, false)]
		public bool DisplayLockedWeapons
		{
			get
			{
				return this.m_DisplayLockedWeapons;
			}
			set
			{
				if (this.m_DisplayLockedWeapons != value)
				{
					this.m_DisplayLockedWeapons = value;
					this.OnPropertyChanged("DisplayLockedWeapons");
				}
			}
		}

		[XmlElement("DisplayPlayerCraftedItems")]
		[SettingDefaultValue(true, false)]
		public bool DisplayPlayerCraftedItems
		{
			get
			{
				return this.m_DisplayPlayerCraftedItems;
			}
			set
			{
				if (this.m_DisplayPlayerCraftedItems != value)
				{
					this.m_DisplayPlayerCraftedItems = value;
					this.OnPropertyChanged("DisplayPlayerCraftedItems");
				}
			}
		}

		[XmlElement("DisplayNonPlayerCraftedItems")]
		[SettingDefaultValue(true, false)]
		public bool DisplayNonPlayerCraftedItems
		{
			get
			{
				return this.m_DisplayNonPlayerCraftedItems;
			}
			set
			{
				if (this.m_DisplayNonPlayerCraftedItems != value)
				{
					this.m_DisplayNonPlayerCraftedItems = value;
					this.OnPropertyChanged("DisplayNonPlayerCraftedItems");
				}
			}
		}

		[XmlElement("FilteredCraftingMaterials")]
		[SettingDefaultValue("DisplayedMaterialsDefaultValues", true)]
		public DisplayedMaterialSetting[] DisplayedMaterials
		{
			get
			{
				return this.m_DisplayedMaterials;
			}
			set
			{
				if (this.m_DisplayedMaterials != null)
				{
					DisplayedMaterialSetting[] displayedMaterials = this.m_DisplayedMaterials;
					for (int i = 0; i < displayedMaterials.Length; i++)
					{
						displayedMaterials[i].PropertyChanged -= this.OnDisplayMaterialSettingPropertyChanged;
					}
				}
				if (value == null)
				{
					throw new NullReferenceException("DisplayedMaterials was set to null.");
				}
				if (this.m_DisplayedMaterials != value)
				{
					DisplayedMaterialSetting[] array = (from x in value
					where x.IsValidCraftingMaterial
					select x).ToArray<DisplayedMaterialSetting>();
					DisplayedMaterialSetting[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j].PropertyChanged += this.OnDisplayMaterialSettingPropertyChanged;
					}
					this.m_DisplayedMaterials = array;
					this.OnPropertyChanged("DisplayedMaterials");
				}
			}
		}

		public override string FileName
		{
			get
			{
				return "Config";
			}
		}

		public bool GetMaterialIsDisplayed(CraftingMaterials _craftingMaterial)
		{
			DisplayedMaterialSetting displayedMaterialSetting = this.DisplayedMaterials.FirstOrDefault((DisplayedMaterialSetting x) => x.Material == _craftingMaterial);
			return displayedMaterialSetting == null || displayedMaterialSetting.IsDisplayed;
		}

		public void SetMaterialIsDisplayed(CraftingMaterials _craftingMaterial, bool _isDisplayed)
		{
			DisplayedMaterialSetting displayedMaterialSetting = this.DisplayedMaterials.FirstOrDefault((DisplayedMaterialSetting x) => x.Material == _craftingMaterial);
			if (displayedMaterialSetting != null)
			{
				displayedMaterialSetting.IsDisplayed = _isDisplayed;
			}
		}

		private void OnDisplayMaterialSettingPropertyChanged(object _sender, PropertyChangedEventArgs _e)
		{
			base.NeedsSave = true;
		}

		private DisplayedMaterialSetting[] DisplayedMaterialsDefaultValues()
		{
			return (from x in Enum.GetNames(typeof(CraftingMaterials))
			select new DisplayedMaterialSetting
			{
				ResourceName = x,
				IsDisplayed = true
			}).ToArray<DisplayedMaterialSetting>();
		}

		private bool m_DisplayPlayerCraftedItems;

		private bool m_DisplayNonPlayerCraftedItems;

		private DisplayedMaterialSetting[] m_DisplayedMaterials;

		private bool m_DisplayLockedWeapons;
	}
}
