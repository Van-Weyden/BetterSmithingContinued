using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels.Templates
{
	public class TextButtonVM : ViewModel
	{
		public event EventHandler ButtonPressed;

		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this.m_IsVisible;
			}
			set
			{
				if (this.m_IsVisible != value)
				{
					this.m_IsVisible = value;
					base.OnPropertyChanged("IsVisible");
				}
			}
		}

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this.m_IsEnabled;
			}
			set
			{
				if (this.m_IsEnabled != value)
				{
					this.m_IsEnabled = value;
					base.OnPropertyChanged("IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public string TextValue
		{
			get
			{
				return this.m_TextValue;
			}
			set
			{
				if (this.m_TextValue != value)
				{
					this.m_TextValue = value;
					base.OnPropertyChanged("TextValue");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel TextButtonHint
		{
			get
			{
				return this.m_TextButtonHint;
			}
			set
			{
				if (this.m_TextButtonHint != value)
				{
					this.m_TextButtonHint = value;
					base.OnPropertyChanged("TextButtonHint");
				}
			}
		}

		[DataSourceProperty]
		public HintViewModel DisabledButtonHint
		{
			get
			{
				return this.m_DisabledButtonHint;
			}
			set
			{
				if (this.m_DisabledButtonHint != value)
				{
					this.m_DisabledButtonHint = value;
					base.OnPropertyChanged("DisabledButtonHint");
				}
			}
		}

		public override void RefreshValues()
		{
			this.TextButtonHint.RefreshValues();
			base.RefreshValues();
		}

		public void OnButtonPressed()
		{
			EventHandler buttonPressed = this.ButtonPressed;
			if (buttonPressed == null)
			{
				return;
			}
			buttonPressed(this, EventArgs.Empty);
		}

		private bool m_IsVisible;

		private string m_TextValue;

		private HintViewModel m_TextButtonHint;

		private bool m_IsEnabled;

		private HintViewModel m_DisabledButtonHint;
	}
}
