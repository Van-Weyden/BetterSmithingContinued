using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using BetterSmithingContinued.Annotations;
using TaleWorlds.Core;

namespace BetterSmithingContinued.Settings
{
	[XmlType("CraftingMaterial")]
	public class DisplayedMaterialSetting : INotifyPropertyChanged
	{
		[XmlElement("Name")]
		public string ResourceName
		{
			get
			{
				return this.m_ResourceName;
			}
			set
			{
				if (this.m_ResourceName != value)
				{
					this.m_ResourceName = value;
					CraftingMaterials material;
					if (Enum.TryParse<CraftingMaterials>(this.ResourceName, out material))
					{
						this.Material = material;
						this.IsValidCraftingMaterial = true;
					}
					else
					{
						this.IsValidCraftingMaterial = false;
					}
					this.OnPropertyChanged("ResourceName");
				}
			}
		}

		[XmlElement("IsDisplayed")]
		public bool IsDisplayed
		{
			get
			{
				return this.m_IsDisplayed;
			}
			set
			{
				if (this.m_IsDisplayed != value)
				{
					this.m_IsDisplayed = value;
					this.OnPropertyChanged("IsDisplayed");
				}
			}
		}

		[XmlIgnore]
		public CraftingMaterials Material { get; private set; }

		[XmlIgnore]
		public bool IsValidCraftingMaterial { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(_propertyName));
		}

		private string m_ResourceName;

		private bool m_IsDisplayed;
	}
}
