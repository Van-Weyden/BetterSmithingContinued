using System;
using System.Collections.Generic;
using TaleWorlds.Localization;

namespace BetterSmithingContinued.Utilities
{
	public static class TextObjectUtilities
	{
		public static TextObject CreateTextObject<T>(T value, Dictionary<string, TextObject> attributes = null)
		{
			return (TextObject)Activator.CreateInstance(typeof(TextObject), new object[]
			{
				value,
				attributes
			});
		}
	}
}
