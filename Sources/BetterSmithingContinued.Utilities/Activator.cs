using System;

namespace BetterSmithingContinued.Utilities
{
	public delegate T Activator<out T>(params object[] _arguments);
}
