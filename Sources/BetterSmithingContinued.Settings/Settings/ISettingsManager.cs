using System;

namespace BetterSmithingContinued.Settings
{
	public interface ISettingsManager
	{
		event EventHandler<SettingsSection> SettingsSectionChanged;

		void RestoreDefaults();

		T GetSettings<T>() where T : SettingsSection;

		void Save();

		void LoadSettings();
	}
}
