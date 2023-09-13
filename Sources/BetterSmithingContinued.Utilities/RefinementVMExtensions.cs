using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Refinement;

namespace BetterSmithingContinued.Utilities
{
	public static class RefinementVMExtensions
	{
		public static void SmartOnSelectAction(this RefinementVM _refinementVM, RefinementActionItemVM _selectedAction)
		{
			OnSelectActionMethodInfo.Value?.Invoke(_refinementVM, new object[]
			{
				_selectedAction
			});
		}

		private static readonly Lazy<MethodInfo> OnSelectActionMethodInfo = new Lazy<MethodInfo>(() => MemberExtractor.GetPrivateMethodInfo<RefinementVM>("OnSelectAction"));
	}
}
