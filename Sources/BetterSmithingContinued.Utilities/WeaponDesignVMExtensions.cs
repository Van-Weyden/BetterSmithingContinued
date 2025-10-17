using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;

namespace BetterSmithingContinued.Utilities
{
	public static class WeaponDesignVMExtensions
	{
		public static void SafeRefreshCurrentHeroSkillLevel(this WeaponDesignVM _weaponDesignVm)
		{
			m_LazyRefreshCurrentHeroSkillLevel.Value(_weaponDesignVm);
		}

		private static readonly Lazy<Action<WeaponDesignVM>> m_LazyRefreshCurrentHeroSkillLevel = new Lazy<Action<WeaponDesignVM>>(delegate()
		{
			MethodInfo methodInfo = MemberExtractor.GetPrivateMethodInfo<WeaponDesignVM>("RefreshCurrentHeroSkillLevel");
			return delegate(WeaponDesignVM _weaponDesignVM) {
				methodInfo?.Invoke(_weaponDesignVM, new object[0]);
			};
		});
	}
}
