using System;
using System.Reflection;
using BetterSmithingContinued.Core;

namespace BetterSmithingContinued.Settings
{
	public abstract class SettingsSection<T> : SettingsSection where T : SettingsSection<T>
	{
		protected SettingsSection()
		{
			this.RestoreDefaults();
			base.NeedsSave = false;
		}

		public sealed override void RestoreDefaults()
		{
			SettingsSection<T>.m_RestoreDefaults.Value(this);
		}

		private static readonly Lazy<Action<SettingsSection<T>>> m_RestoreDefaults = new Lazy<Action<SettingsSection<T>>>(delegate()
		{
			Action<SettingsSection<T>> action = delegate(SettingsSection<T> _section)
			{
			};
			PropertyInfo[] properties = typeof(T).GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				SettingDefaultValueAttribute defaultValueAttribute = propertyInfo.GetCustomAttribute<SettingDefaultValueAttribute>();
				if (defaultValueAttribute != null)
				{
					if (defaultValueAttribute.IsMethodName)
					{
						MethodInfo defaultValueMethodInfo = typeof(T).GetMethod((string)defaultValueAttribute.DefaultValue, MemberExtractor.PrivateMemberFlags);
						action = (Action<SettingsSection<T>>)Delegate.Combine(action, new Action<SettingsSection<T>>(delegate(SettingsSection<T> _section)
						{
							propertyInfo.SetValue(_section, (defaultValueMethodInfo != null) ? defaultValueMethodInfo.Invoke(_section, new object[0]) : null);
						}));
					}
					else
					{
						action = (Action<SettingsSection<T>>)Delegate.Combine(action, new Action<SettingsSection<T>>(delegate(SettingsSection<T> _section)
						{
							propertyInfo.SetValue(_section, defaultValueAttribute.DefaultValue);
						}));
					}
				}
			}
			return action;
		});
	}
}
