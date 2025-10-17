using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels.Templates
{
	public class PerkPanelItemVM : ViewModel
	{
		[DataSourceProperty]
		public string SpriteAsStr
		{
			get
			{
				return this.m_SpriteAsStr;
			}
			set
			{
				if (value != this.m_SpriteAsStr)
				{
					this.m_SpriteAsStr = value;
					base.OnPropertyChangedWithValue(value, "SpriteAsStr");
				}
			}
		}

		[DataSourceProperty]
		public string PerkText
		{
			get
			{
				return this.m_PerkText;
			}
			set
			{
				if (value != this.m_PerkText)
				{
					this.m_PerkText = value;
					base.OnPropertyChangedWithValue(value, "PerkText");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel PerkHint
		{
			get
			{
				return this.m_PerkHint;
			}
			set
			{
				if (value != this.m_PerkHint)
				{
					this.m_PerkHint = value;
					base.OnPropertyChangedWithValue(value, "PerkHint");
				}
			}
		}

		public BasicTooltipViewModel PerkBasicHint
		{
			get
			{
				return this.m_PerkBasicHint;
			}
			set
			{
				if (this.m_PerkBasicHint != value)
				{
					this.m_PerkBasicHint = value;
					base.OnPropertyChangedWithValue(value, "PerkBasicHint");
				}
			}
		}

		private string m_SpriteAsStr;

		private string m_PerkText;

		private HintViewModel m_PerkHint;

		private BasicTooltipViewModel m_PerkBasicHint;
	}
}
