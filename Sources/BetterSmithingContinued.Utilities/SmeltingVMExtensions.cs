using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;

namespace BetterSmithingContinued.Utilities
{
	public static class SmeltingVMExtensions
	{
		public static void SmartSelectItem(this SmeltingVM _smeltingVM, SmeltingItemVM _newItem)
		{
			m_OnItemSelectionMethodInfo.Value?.Invoke(_smeltingVM, new object[] {
				_newItem
			});
		}

		public static ItemRoster GetPlayerItemRoster(this SmeltingVM _smeltingVM)
		{
			return (ItemRoster) m_PlayerItemRosterFieldInfo.Value?.GetValue(_smeltingVM);
		}

		private static readonly Lazy<MethodInfo> m_OnItemSelectionMethodInfo = new Lazy<MethodInfo>(() => MemberExtractor.GetPrivateMethodInfo<SmeltingVM>("OnItemSelection"));
		private static readonly Lazy<FieldInfo> m_PlayerItemRosterFieldInfo = new Lazy<FieldInfo>(() => MemberExtractor.GetPrivateFieldInfo<SmeltingVM>("_playerItemRoster"));
	}
}
