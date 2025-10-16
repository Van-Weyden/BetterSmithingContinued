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
			MethodInfo value = SmeltingVMExtensions.m_OnItemSelectionMethodInfo.Value;
			if (value == null)
			{
				return;
			}
			value.Invoke(_smeltingVM, new object[]
			{
				_newItem
			});
		}

		public static ItemRoster GetPlayerItemRoster(this SmeltingVM _smeltingVM)
		{
			FieldInfo value = SmeltingVMExtensions.m_PlayerItemRosterFieldInfo.Value;
			return (ItemRoster)((value != null) ? value.GetValue(_smeltingVM) : null);
		}

		private static readonly Lazy<MethodInfo> m_OnItemSelectionMethodInfo = new Lazy<MethodInfo>(() => typeof(SmeltingVM).GetMethod("OnItemSelection", MemberExtractor.PrivateMemberFlags));

		private static readonly Lazy<FieldInfo> m_PlayerItemRosterFieldInfo = new Lazy<FieldInfo>(() => typeof(SmeltingVM).GetField("_playerItemRoster", MemberExtractor.PrivateMemberFlags));
	}
}
