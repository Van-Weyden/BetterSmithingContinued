using System;
using BetterSmithingContinued.MainFrame.Persistence;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels
{
	public sealed class CraftingItemTemplateVM : ViewModel
	{
		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this.m_Visual;
			}
			set
			{
				if (this.m_Visual != value)
				{
					this.m_Visual = value;
					base.OnPropertyChangedWithValue(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this.m_Name;
			}
			set
			{
				if (this.m_Name != value)
				{
					this.m_Name = value;
					base.OnPropertyChangedWithValue(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this.m_IsSelected;
			}
			set
			{
				if (this.m_IsSelected != value)
				{
					this.m_IsSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		public WeaponData WeaponData { get; }

		public CraftingItemTemplateVM(WeaponData _weaponData, Action<CraftingItemTemplateVM> _onSelection)
		{
			this.WeaponData = _weaponData;
			this.m_OnSelection = _onSelection;
			this.Name = this.WeaponData.Name;
			this.Visual = new ImageIdentifierVM(this.WeaponData.ItemObject, "");
			this.RefreshValues();
		}

		private void ExecuteSelection()
		{
			this.m_OnSelection(this);
		}

		private readonly Action<CraftingItemTemplateVM> m_OnSelection;

		private ImageIdentifierVM m_Visual;

		private string m_Name;

		private bool m_IsSelected;
	}
}
