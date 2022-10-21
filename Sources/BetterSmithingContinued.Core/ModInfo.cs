using System;
using TaleWorlds.ModuleManager;

namespace BetterSmithingContinued.Core
{
	public class ModInfo
	{
		public const string Id = "BetterSmithingContinued";

		public const string LowerCaseId = "bettersmithingcontinued";

		public const string Name = "Better Smithing Continued";

		public static readonly string Version = ModuleHelper.GetModuleInfo("BetterSmithingContinued").Version.ToString();
	}
}
