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
			WeaponDesignVMExtensions.m_LazyRefreshCurrentHeroSkillLevel.Value(_weaponDesignVm);
		}

		private static readonly Lazy<Action<WeaponDesignVM>> m_LazyRefreshCurrentHeroSkillLevel = new Lazy<Action<WeaponDesignVM>>(delegate()
		{
			MethodInfo methodInfo = typeof(WeaponDesignVM).GetMethod("RefreshCurrentHeroSkillLevel", MemberExtractor.PrivateMemberFlags);
			return delegate(WeaponDesignVM _weaponDesignVM)
			{
				MethodInfo methodInfo_ = methodInfo;
				if (methodInfo_ == null)
				{
					return;
				}
                methodInfo_.Invoke(_weaponDesignVM, new object[0]);
			};
		});
	}
}
