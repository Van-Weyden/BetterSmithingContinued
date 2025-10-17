using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("Crafting", "descendant::ButtonWidget[@Id='CurrentCraftingHeroToggleWidget']/Children/ListPanel/Children/Widget")]
	public class CurrentHeroSmithSkillLevelIconColorPatch : PrefabExtensionSetAttributePatch
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
