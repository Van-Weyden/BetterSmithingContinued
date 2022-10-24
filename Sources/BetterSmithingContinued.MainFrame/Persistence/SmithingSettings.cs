using System;
using System.Xml.Serialization;
using BetterSmithingContinued.Settings;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("GlobalSmithingSettings")]
	public sealed class SmithingSettings : SettingsSection<SmithingSettings>
	{
		[XmlElement("OnlyCycleHeroesWithStamina")]
		[SettingDefaultValue(true, false)]
		public bool OnlyCycleHeroesWithStamina
		{
			get
			{
				return this.m_OnlyCycleHeroesWithStamina;
			}
			set
			{
				if (this.m_OnlyCycleHeroesWithStamina != value)
				{
					this.m_OnlyCycleHeroesWithStamina = value;
					this.OnPropertyChanged("OnlyCycleHeroesWithStamina");
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

		private bool m_OnlyCycleHeroesWithStamina = true;
	}
}
