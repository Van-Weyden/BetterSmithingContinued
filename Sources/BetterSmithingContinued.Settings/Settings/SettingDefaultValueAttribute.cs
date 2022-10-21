using System;

namespace BetterSmithingContinued.Settings
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingDefaultValueAttribute : Attribute
	{
		public bool IsMethodName { get; }

		public object DefaultValue { get; }

		public SettingDefaultValueAttribute(object _defaultValue, bool _isMethodName = false)
		{
			this.DefaultValue = _defaultValue;
			this.IsMethodName = _isMethodName;
		}
	}
}
