using System;
using TaleWorlds.InputSystem;

namespace BetterSmithingContinued.Inputs.Code
{
	public abstract class HotKey
	{
		public event EventHandler<KeyState> KeyStateChanged;

		public KeyState CurrentKeyState
		{
			get
			{
				return this.m_CurrentKeyState;
			}
			set
			{
				if (this.m_CurrentKeyState != value)
				{
					this.m_CurrentKeyState = value;
					this.OnKeyStateChanged(this.m_CurrentKeyState);
				}
			}
		}

		public string Name { get; }

		public string Tooltip { get; }

		public int ID { get; set; }

		public string Category { get; }

		public InputKey DefaultKey { get; }

		public GameKey GameKey { get; set; }

		public Func<bool> IsEnabled { get; set; }

		protected HotKey(string _name, string _tooltip, string _category, InputKey _defaultKey)
		{
			this.Name = _name;
			this.Tooltip = _tooltip;
			this.Category = _category;
			this.DefaultKey = _defaultKey;
			this.m_CurrentKeyState = KeyState.None;
		}

		private void OnKeyStateChanged(KeyState _e)
		{
			EventHandler<KeyState> keyStateChanged = this.KeyStateChanged;
			if (keyStateChanged == null)
			{
				return;
			}
			keyStateChanged(this, _e);
		}

		private KeyState m_CurrentKeyState;
	}
}
