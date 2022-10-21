using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace BetterSmithingContinued.Utilities
{
	public static class WidgetExtensions
	{
		public static void InvokeOnPropertyChanged(this Widget _widget, object _value, string _name)
		{
			WidgetExtensions.m_LazyOnPropertyChangedInvoker.Value(_widget, _value, _name);
		}

		private static readonly Lazy<Action<Widget, object, string>> m_LazyOnPropertyChangedInvoker = new Lazy<Action<Widget, object, string>>(delegate()
		{
			MethodInfo methodInfo = typeof(PropertyOwnerObject).GetMethod("OnPropertyChanged", MemberExtractor.PrivateMemberFlags);
			return delegate(Widget widget, object value, string name)
			{
				MethodInfo methodInfo_ = methodInfo;
				if (methodInfo_ == null)
				{
					return;
				}
                methodInfo_.Invoke(widget, new object[]
				{
					value,
					name
				});
			};
		});
	}
}
