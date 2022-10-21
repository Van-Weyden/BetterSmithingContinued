using System;
using HarmonyLib;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Core
{
	public class Logger
	{
		public static void Add(string message, Logger.LogType logType = Logger.LogType.HarmonyLogFile)
		{
			if (!Logger.isChatLogEnabled && logType == Logger.LogType.Chat)
			{
				return;
			}
			if (!Logger.isHarmonyLogEnabled && logType == Logger.LogType.HarmonyLogFile)
			{
				return;
			}
			DateTime now = DateTime.Now;
			string text = string.Concat(new string[]
			{
				"[",
				now.Hour.ToString(),
				":",
				now.Minute.ToString(),
				":",
				now.Second.ToString(),
				".",
				now.Millisecond.ToString(),
				"] BetterSmithingContinued: ",
				message
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

		public static void Add(object obj, Logger.LogType logType = Logger.LogType.Chat)
		{
			Logger.Add(obj.ToString(), logType);
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
