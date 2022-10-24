using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("Crafting", "descendant::NavigatableGridWidget[@Id='InnerList']/ItemTemplate/Widget/Children/ListPanel/Children/Widget")]
	public class HeroSmithSkillLevelIconColorPatch : PrefabExtensionSetAttributePatch
	{
		public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
		{
			get
			{
				return new List<PrefabExtensionSetAttributePatch.Attribute>
				{
					new PrefabExtensionSetAttributePatch.Attribute("Color", "@HeroSmithSkillColor")
				};
			}
		}
	}
}
