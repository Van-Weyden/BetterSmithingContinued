using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("CraftingHeroPopup", "descendant::ListPanel[@Id='Content']/Children/ListPanel/Children/ListPanel/Children/Widget")]
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
