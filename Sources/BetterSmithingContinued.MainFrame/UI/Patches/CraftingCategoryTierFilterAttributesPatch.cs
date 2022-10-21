using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("CraftingCategory", "descendant::Widget[@Id='CraftingCategoryParent']/Children/ListPanel/Children/ListPanel[@Id='CraftingFilters']")]
	public class CraftingCategoryTierFilterAttributesPatch : PrefabExtensionSetAttributePatch
	{
		public override List<PrefabExtensionSetAttributePatch.Attribute> Attributes
		{
			get
			{
				return new List<PrefabExtensionSetAttributePatch.Attribute>
				{
					new PrefabExtensionSetAttributePatch.Attribute("IsVisible", "@IsDefaultCraftingMenuVisible")
				};
			}
		}
	}
}
