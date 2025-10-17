using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.WeaponDesign;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingVMExtensions
	{
		public static CraftingCampaignBehavior GetCraftingCampaignBehavior(this CraftingVM _craftingVm)
		{
			FieldInfo craftingCampaignBehavior = CraftingVMExtensions.CraftingCampaignBehavior;
			return (CraftingCampaignBehavior)((craftingCampaignBehavior != null) ? craftingCampaignBehavior.GetValue(_craftingVm) : null);
		}

		public static bool SmartHaveEnergy(this CraftingVM _craftingVM)
		{
			MethodInfo haveEnergyMethodInfo = CraftingVMExtensions.HaveEnergyMethodInfo;
			return (bool)(((haveEnergyMethodInfo != null) ? haveEnergyMethodInfo.Invoke(_craftingVM, new object[0]) : null) ?? false);
		}

		public static bool SmartRefreshEnabledMainAction(this CraftingVM _craftingVM)
		{
			MethodInfo refreshEnableMainActionMethodInfo = CraftingVMExtensions.RefreshEnableMainActionMethodInfo;
			bool result = (bool)(((refreshEnableMainActionMethodInfo != null) ? refreshEnableMainActionMethodInfo.Invoke(_craftingVM, new object[0]) : null) ?? false);
			WeaponDesignVM weaponDesign = _craftingVM.WeaponDesign;
			if (weaponDesign != null)
			{
				weaponDesign.SafeRefreshCurrentHeroSkillLevel();
			}
			return result;
		}

		private static FieldInfo CraftingCampaignBehavior
		{
			get
			{
				FieldInfo fieldInfo = CraftingVMExtensions.m_CraftingCampaignBehavior;
				if (fieldInfo == null)
				{
					fieldInfo = (CraftingVMExtensions.m_CraftingCampaignBehavior = typeof(CraftingVM).GetField("_craftingBehavior", MemberExtractor.PrivateMemberFlags));
				}
				return fieldInfo;
			}
		}

		private static MethodInfo HaveEnergyMethodInfo
		{
			get
			{
				MethodInfo methodInfo = CraftingVMExtensions.m_HaveEnergyMethodInfo;
				if (methodInfo == null)
				{
					methodInfo = (CraftingVMExtensions.m_HaveEnergyMethodInfo = typeof(CraftingVM).GetMethod("HaveEnergy", MemberExtractor.PrivateMemberFlags));
				}
				return methodInfo;
			}
		}

		private static MethodInfo RefreshEnableMainActionMethodInfo
		{
			get
			{
				MethodInfo methodInfo = CraftingVMExtensions.m_RefreshEnableMainActionMethodInfo;
				if (methodInfo == null)
				{
					methodInfo = (CraftingVMExtensions.m_RefreshEnableMainActionMethodInfo = typeof(CraftingVM).GetMethod("RefreshEnableMainAction", MemberExtractor.PrivateMemberFlags));
				}
				return methodInfo;
			}
		}

		private static FieldInfo m_CraftingCampaignBehavior;

		private static MethodInfo m_HaveEnergyMethodInfo;

		private static MethodInfo m_RefreshEnableMainActionMethodInfo;
	}
}
