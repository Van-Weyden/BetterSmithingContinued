using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using SandBox.GauntletUI;
using TaleWorlds.Engine.GauntletUI;

namespace BetterSmithingContinued.Utilities
{
	public static class CraftingGauntletScreenExtensions
	{
		public static GauntletLayer GetGauntletLayer(this CraftingGauntletScreen _instance)
		{
			if (_instance != null)
			{
				return CraftingGauntletScreenExtensions.m_LazyGauntletLayerAccessor.Value(_instance);
			}
			return null;
		}

		private static readonly Lazy<Func<CraftingGauntletScreen, GauntletLayer>> m_LazyGauntletLayerAccessor = new Lazy<Func<CraftingGauntletScreen, GauntletLayer>>(delegate()
		{
			FieldInfo fieldInfo = typeof(CraftingGauntletScreen).GetField("_gauntletLayer", MemberExtractor.PrivateMemberFlags);
			return delegate(CraftingGauntletScreen _craftingGauntletScreen)
			{
				FieldInfo fieldInfo_ = fieldInfo;
				return ((fieldInfo_ != null) ? fieldInfo_.GetValue(_craftingGauntletScreen) : null) as GauntletLayer;
			};
		});
	}
}
