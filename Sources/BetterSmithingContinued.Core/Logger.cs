using System;
using HarmonyLib;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Core
{
	public class Logger
	{
        public static void Add(string message)
		{
			Add(message, Logger.LogType.HarmonyLogFile);
            Add(message, Logger.LogType.Chat);
        }

        public static void Add(string message, Logger.LogType logType)
		{
			if (!Logger.isChatLogEnabled && logType == Logger.LogType.Chat)
			{
				return;
			}
			else if (!Logger.isHarmonyLogEnabled && logType == Logger.LogType.HarmonyLogFile)
			{
				return;
			}

			DateTime now = DateTime.Now;
			string text = string.Concat(new string[] {
				"[",
					now.Hour.ToString(), ":",
					now.Minute.ToString(), ":",
					now.Second.ToString(), ".",
					now.Millisecond.ToString(),
				"] BetterSmithingContinued: ", message
			});
			if (logType == Logger.LogType.Chat)
			{
				InformationManager.DisplayMessage(new InformationMessage(text));
				return;
			}
			if (logType != Logger.LogType.HarmonyLogFile)
			{
				return;
			}
			FileLog.Log(text);
		}

		public static void Add(object obj, Logger.LogType logType)
		{
			Logger.Add(obj.ToString(), logType);
        }

        public static void Add(object obj)
        {
            Add(obj.ToString());
        }

        public static bool isChatLogEnabled = true;

		public static bool isHarmonyLogEnabled = false;

		public enum LogType
		{
			Chat,
			HarmonyLogFile
		}
	}
}
