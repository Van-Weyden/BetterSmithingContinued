using System;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace BetterSmithingContinued.MainFrame.UI.ViewModels.Templates
{
	public class ButtonToggleVM : ViewModel
	{
		public event EventHandler<bool> ToggleStateChanged;

		[DataSourceProperty]
		public HintViewModel ToggleHint
		{
			get
			{
				return this.m_ToggleHint;
			}
			set
			{
				if (value == this.m_ToggleHint)
				{
					return;
				}
				this.m_ToggleHint = value;
				base.OnPropertyChangedWithValue(value, "ToggleHint");
			}
		}

		[DataSourceProperty]
		public string SpriteAsStr
		{
			get
			{
				return this.m_SpriteAsStr;
			}
			set
			{
				if (value == this.m_SpriteAsStr)
				{
					return;
				}
				this.m_SpriteAsStr = value;
				base.OnPropertyChangedWithValue(value, "SpriteAsStr");
			}
		}

		[DataSourceProperty]
		public bool IsToggledOn
		{
			get
			{
				return this.m_IsToggledOn;
			}
			set
			{
				if (value == this.m_IsToggledOn)
				{
					return;
				}
				this.m_IsToggledOn = value;
				base.OnPropertyChangedWithValue(value, "IsToggledOn");
				this.OnToggleStateChanged(this.IsToggledOn);
				this.IsToggledOff = !this.m_IsToggledOn;
			}
		}

		[DataSourceProperty]
		public bool IsToggledOff
		{
			get
			{
				return this.m_IsToggledOff;
			}
			set
			{
				if (value == this.m_IsToggledOff)
				{
					return;
				}
				this.m_IsToggledOff = value;
				base.OnPropertyChangedWithValue(value, "IsToggledOff");
			}
		}

		[DataSourceProperty]
		public bool IsDisabled
		{
			get
			{
				return this.m_IsDisabled;
			}
			set
			{
				if (value == this.m_IsDisabled)
				{
					return;
				}
				this.m_IsDisabled = value;
				base.OnPropertyChangedWithValue(value, "IsDisabled");
			}
		}

		[DataSourceProperty]
		public bool IsVisible
		{
			get
			{
				return this.m_IsVisible;
			}
			set
			{
				if (value == this.m_IsVisible)
				{
					return;
				}
				this.m_IsVisible = value;
				base.OnPropertyChangedWithValue(value, "IsVisible");
			}
		}

		public ButtonToggleVM(bool _defaultIsToggledOn)
		{
			this.IsVisible = true;
			this.IsDisabled = false;
			this.m_IsToggledOn = !_defaultIsToggledOn;
			this.IsToggledOn = _defaultIsToggledOn;
		}

		public void OnButtonPressed()
		{
			this.IsToggledOn = !this.IsToggledOn;
		}

		protected virtual void OnToggleStateChanged(bool _isToggledOn)
		{
			EventHandler<bool> toggleStateChanged = this.ToggleStateChanged;
			if (toggleStateChanged == null)
			{
				return;
			}
			toggleStateChanged(this, _isToggledOn);
		}

		private HintViewModel m_ToggleHint;

		private bool m_IsToggledOn;

		private string m_SpriteAsStr;

		private bool m_IsToggledOff;

		private bool m_IsDisabled;

		private bool m_IsVisible;
	}
}
