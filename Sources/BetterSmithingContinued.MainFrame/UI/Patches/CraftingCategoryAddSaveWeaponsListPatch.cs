using System;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace BetterSmithingContinued.MainFrame.UI.Patches
{
	[PrefabExtension("CraftingCategory", "descendant::Widget[@Id='CraftingCategoryParent']/Children/Widget[@Id='InnerPanel']/Children/ListPanel[@Id='PieceListParent']/Children/TabControl")]
	public class CraftingCategoryAddSaveWeaponsListPatch : PrefabExtensionInsertPatch
	{
		public override InsertType Type
		{
			get
			{
				return InsertType.Prepend;
			}
		}

		[PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
		public string GetPrefabExtension()
		{
			return "SavedWeaponsList";
		}
	}
}
