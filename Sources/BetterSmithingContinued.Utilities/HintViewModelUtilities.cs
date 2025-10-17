using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.Utilities
{
	public static class HintViewModelUtilities
	{
		public static HintViewModel CreateHintViewModel(string _text)
		{
			return HintViewModelUtilities.m_HintViewModelInstantiator.Value(_text);
		}

		private static Lazy<Func<string, HintViewModel>> m_HintViewModelInstantiator = new Lazy<Func<string, HintViewModel>>(delegate()
		{
			Activator<HintViewModel> activator = typeof(HintViewModel).GetActivator<HintViewModel>(new Type[] {
				typeof(TextObject),
				typeof(string)
			});
			return delegate(string _s)
			{
				Activator<HintViewModel> activator_ = activator;
				object[] array = new object[2];
				array[0] = new TextObject(_s, null);
				return activator_(array);
			};
		});
	}
}
