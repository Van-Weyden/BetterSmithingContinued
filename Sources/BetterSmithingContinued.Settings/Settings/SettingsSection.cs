using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using BetterSmithingContinued.Annotations;

namespace BetterSmithingContinued.Settings
{
	public abstract class SettingsSection : INotifyPropertyChanged
	{
		public abstract string FileName { get; }

		[XmlIgnore]
		public bool NeedsSave { get; set; }

		protected SettingsSection()
		{
			this.NeedsSave = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public abstract void RestoreDefaults();

		public virtual void OnDeserialized()
		{
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string _propertyName = null)
		{
			this.NeedsSave = true;
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, new PropertyChangedEventArgs(_propertyName));
		}
	}
}
