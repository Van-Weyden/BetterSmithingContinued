using System;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Core.Modules
{
	public abstract class ConnectedViewModel : ViewModel, IConnectedObject
	{
		protected IPublicContainer PublicContainer { get; }

		protected ConnectedViewModel(IPublicContainer _publicContainer)
		{
			this.PublicContainer = _publicContainer;
		}

		public virtual void Load()
		{
		}

		public virtual void Unload()
		{
		}

		public override void OnFinalize()
		{
			base.OnFinalize();
			this.Unload();
		}
	}
}
