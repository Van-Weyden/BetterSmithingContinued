using System;
using BetterSmithingContinued.Core.Modules;

namespace BetterSmithingContinued.Core
{
	public interface IPublicContainer
	{
		string Name { get; }

		IPublicContainer Parent { get; }

		Token RegisterModule<T>(IModule _module, string _moduleName = "") where T : class;

		void UnregisterModule(Token _token);

		T GetModule<T>(string _moduleName = "") where T : class;
	}
}
