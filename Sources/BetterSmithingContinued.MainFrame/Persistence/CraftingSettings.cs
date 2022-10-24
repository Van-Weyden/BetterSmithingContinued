using System;
using System.Xml.Serialization;
using BetterSmithingContinued.Settings;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("CraftingSettings")]
	public class CraftingSettings : SettingsSection<CraftingSettings>
	{
		[XmlElement("SkipWeaponFinalizationPopup")]
		[SettingDefaultValue(true, false)]
		public bool SkipWeaponFinalizationPopup
		{
			get
			{
				return this.m_SkipWeaponFinalizationPopup;
			}
			set
			{
				if (this.m_SkipWeaponFinalizationPopup != value)
				{
					this.m_SkipWeaponFinalizationPopup = value;
					this.OnPropertyChanged("SkipWeaponFinalizationPopup");
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

		private bool m_SkipWeaponFinalizationPopup;
	}
}
