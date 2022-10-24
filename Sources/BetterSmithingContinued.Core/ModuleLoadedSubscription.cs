using System;
using System.Collections.Generic;

namespace BetterSmithingContinued.Core
{
	public class ModuleLoadedSubscription
	{
		public Action OnModulesLoaded { get; private set; }

		public Type Target { get; private set; }

		public CallSequence CallSequence { get; private set; }

		public Type SubscriberType { get; private set; }

		public ModuleLoadedSubscription(Type _subscriberType, Action _onModulesLoaded)
		{
			this.CallAfterList = new List<ModuleLoadedSubscription>();
			this.SubscriberType = _subscriberType;
			this.OnModulesLoaded = _onModulesLoaded;
		}

		public void CallModulesLoaded()
		{
			this.OnModulesLoaded();
			foreach (ModuleLoadedSubscription moduleLoadedSubscription in this.CallAfterList)
			{
				moduleLoadedSubscription.CallModulesLoaded();
			}
		}

		public bool HandlesSubscription(ModuleLoadedSubscription _otherSubscription)
		{
			if (_otherSubscription.Target == this.SubscriberType)
			{
				if (_otherSubscription.CallSequence == CallSequence.CallBefore)
				{
					this.SwapSubscriptions(this, _otherSubscription);
				}
				this.CallAfterList.Add(_otherSubscription);
				return true;
			}
			if (this.Target == _otherSubscription.SubscriberType)
			{
				if (this.CallSequence == CallSequence.CallAfter)
				{
					this.SwapSubscriptions(this, _otherSubscription);
				}
				this.CallAfterList.Add(_otherSubscription);
				return true;
			}
			using (List<ModuleLoadedSubscription>.Enumerator enumerator = this.CallAfterList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HandlesSubscription(_otherSubscription))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void SwapSubscriptions(ModuleLoadedSubscription x, ModuleLoadedSubscription y)
		{
			Action onModulesLoaded = x.OnModulesLoaded;
			Type target = x.Target;
			CallSequence callSequence = x.CallSequence;
			Type subscriberType = x.SubscriberType;
			List<ModuleLoadedSubscription> callAfterList = x.CallAfterList;
			x.OnModulesLoaded = y.OnModulesLoaded;
			x.Target = y.Target;
			x.CallSequence = y.CallSequence;
			x.SubscriberType = y.SubscriberType;
			x.CallAfterList = y.CallAfterList;
			y.OnModulesLoaded = onModulesLoaded;
			y.Target = target;
			y.CallSequence = callSequence;
			y.SubscriberType = subscriberType;
			y.CallAfterList = callAfterList;
		}

		public List<ModuleLoadedSubscription> CallAfterList;
	}
}
