using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("CraftingCategory", "descendant::Widget[@Id='CraftingCategoryParent']/Children/ListPanel/Children/Widget[@Id='ModeSelection']")]
	public class CraftingCategoryDropdownReplacementPatch : PrefabExtensionInsertPatch
	{
		public override InsertType Type
		{
			get
			{
				return InsertType.Append;
			}
		}

		[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(true)]
		public string PatchContent
		{
			get
			{
				return "CraftingCategoryDropdownReplacement";
			}
		}
	}
}
