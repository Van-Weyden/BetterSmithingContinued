using System;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Utilities
{
	public static class Messaging
	{
		public static void DisplayMessage(string _message)
		{
			Messaging.DisplayMessage(new InformationMessage(_message));
		}

		public static void DisplayMessage(InformationMessage _informationMessage)
		{
			InformationManager.DisplayMessage(_informationMessage);
		}
	}
}
