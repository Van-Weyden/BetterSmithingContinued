using System;
using System.Xml.Serialization;
using BetterSmithingContinued.Settings;

namespace BetterSmithingContinued.MainFrame.Persistence
{
	[XmlType("RefinementSettings")]
	public sealed class RefiningSettings : SettingsSection<RefiningSettings>
	{
		[XmlElement("OnlyCycleHeroesWithCurrentRecipe")]
		[SettingDefaultValue(true, false)]
		public bool OnlyCycleHeroesWithCurrentRecipe
		{
			get
			{
				return this.m_OnlyCycleHeroesWithCurrentRecipe;
			}
			set
			{
				if (this.m_OnlyCycleHeroesWithCurrentRecipe != value)
				{
					this.m_OnlyCycleHeroesWithCurrentRecipe = value;
					this.OnPropertyChanged("OnlyCycleHeroesWithCurrentRecipe");
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

		private bool m_OnlyCycleHeroesWithCurrentRecipe;
	}
}
