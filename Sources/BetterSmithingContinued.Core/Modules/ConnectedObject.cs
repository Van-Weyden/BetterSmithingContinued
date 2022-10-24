using System;

namespace BetterSmithingContinued.Core.Modules
{
	public abstract class ConnectedObject : IConnectedObject
	{
		protected IPublicContainer PublicContainer { get; }

		protected ConnectedObject(IPublicContainer _publicContainer)
		{
			this.PublicContainer = _publicContainer;
		}

		public virtual void Load()
		{
		}

		public virtual void Unload()
		{
		}
	}
}
