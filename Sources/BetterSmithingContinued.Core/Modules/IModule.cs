using System;

namespace BetterSmithingContinued.Core.Modules
{
	public interface IModule
	{
		void Create(IPublicContainer _publicContainer);

		void Load();

		void Unload();
	}
}
