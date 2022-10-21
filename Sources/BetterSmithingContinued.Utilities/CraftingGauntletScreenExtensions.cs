using System;
using System.Reflection;
using BetterSmithingContinued.Core;
using SandBox.GauntletUI;
using TaleWorlds.Engine.GauntletUI;

namespace BetterSmithingContinued.Utilities
{
	public static class GauntletCraftingScreenExtensions
	{
		public static GauntletLayer GetGauntletLayer(this GauntletCraftingScreen _instance)
		{
			if (_instance != null)
			{
				return GauntletCraftingScreenExtensions.m_LazyGauntletLayerAccessor.Value(_instance);
			}
			return null;
		}

		private static readonly Lazy<Func<GauntletCraftingScreen, GauntletLayer>> m_LazyGauntletLayerAccessor = new Lazy<Func<GauntletCraftingScreen, GauntletLayer>>(delegate()
		{
			FieldInfo fieldInfo = typeof(GauntletCraftingScreen).GetField("_gauntletLayer", MemberExtractor.PrivateMemberFlags);
			return delegate(GauntletCraftingScreen _GauntletCraftingScreen)
			{
				FieldInfo fieldInfo_ = fieldInfo;
				return ((fieldInfo_ != null) ? fieldInfo_.GetValue(_GauntletCraftingScreen) : null) as GauntletLayer;
			};
		});
	}
}
