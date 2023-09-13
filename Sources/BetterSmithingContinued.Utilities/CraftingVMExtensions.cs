using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingVMExtensions
	{
		public static CraftingCampaignBehavior GetCraftingCampaignBehavior(this CraftingVM _craftingVm)
		{
			return (CraftingCampaignBehavior) CraftingCampaignBehavior?.GetValue(_craftingVm);
		}

		public static bool SmartHaveEnergy(this CraftingVM _craftingVM)
		{
			return (bool)(HaveEnergyMethodInfo?.Invoke(_craftingVM, new object[0]) ?? false);
		}

		public static bool SmartRefreshEnabledMainAction(this CraftingVM _craftingVM)
		{
			bool result = (bool)(RefreshEnableMainActionMethodInfo?.Invoke(_craftingVM, new object[0]) ?? false);
			_craftingVM.WeaponDesign?.SafeRefreshCurrentHeroSkillLevel();
			return result;
		}

		private static FieldInfo CraftingCampaignBehavior
		{
			get
			{
				if (m_CraftingCampaignBehavior == null)
				{
					m_CraftingCampaignBehavior = MemberExtractor.GetPrivateFieldInfo<CraftingVM>("_craftingBehavior");
				}
				return m_CraftingCampaignBehavior;
			}
		}

		private static MethodInfo HaveEnergyMethodInfo
		{
			get
			{
				if (m_HaveEnergyMethodInfo == null)
				{
					m_HaveEnergyMethodInfo = MemberExtractor.GetPrivateMethodInfo<CraftingVM>("HaveEnergy");
				}
				return m_HaveEnergyMethodInfo;
			}
		}

		private static MethodInfo RefreshEnableMainActionMethodInfo
		{
			get
			{
				if (m_RefreshEnableMainActionMethodInfo == null)
				{
					m_RefreshEnableMainActionMethodInfo = MemberExtractor.GetPrivateMethodInfo<CraftingVM>("RefreshEnableMainAction");
				}
				return m_RefreshEnableMainActionMethodInfo;
			}
		}

		private static FieldInfo m_CraftingCampaignBehavior;
		private static MethodInfo m_HaveEnergyMethodInfo;
		private static MethodInfo m_RefreshEnableMainActionMethodInfo;
	}
}
