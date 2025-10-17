using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace BetterSmithingContinued.Inputs.Code
{
	public class InputManager : BetterSmithingContinued.Core.Modules.Module, IInputManager
	{
		public void AddHotkey(HotKey _hotKey)
		{
			List<HotKey> list;
			if (!this.m_Hotkeys.TryGetValue(_hotKey.Category, out list))
			{
				list = new List<HotKey>();
				this.m_Hotkeys.Add(_hotKey.Category, list);
			}
			_hotKey.ID = this.m_CurrentHotkeyId;
			this.m_CurrentHotkeyId++;
			list.Add(_hotKey);
		}

		public override void Create(IPublicContainer _publicContainer)
		{
			base.Create(_publicContainer);
			this.m_Hotkeys = new Dictionary<string, List<HotKey>>();
			this.m_CurrentHotkeyId = HotKeyManager.GetAllCategories().SelectMany((GameKeyContext context) => ((context != null) ? context.RegisteredGameKeys : null) ?? new MBReadOnlyList<GameKey>(new List<GameKey>())).Max(delegate(GameKey gameKey)
			{
				if (gameKey == null)
				{
					return 0;
				}
				return gameKey.Id;
			}) + 50;
			this.RegisterModule<IInputManager>("");
		}

		public override void Load()
		{
			this.m_SubModuleEventNotifier = base.PublicContainer.GetModule<ISubModuleEventNotifier>("");
			this.m_SubModuleEventNotifier.GameTick += this.OnGameTick;
			this.m_SubModuleEventNotifier.SubscribeToModulesLoaded(new ModuleLoadedSubscription(typeof(ISubModuleEventNotifier), new Action(this.RegisterHotKeys)));
		}

		public override void Unload()
		{
			if (this.m_SubModuleEventNotifier != null)
			{
				this.m_SubModuleEventNotifier.GameTick -= this.OnGameTick;
			}
		}

		private void RegisterHotKeys()
		{
			HotKeyManager.RegisterInitialContexts(this.m_Hotkeys.Select(delegate(KeyValuePair<string, List<HotKey>> category)
			{
				string id = "BetterSmithingContinued";
				int gameKeysCount = this.m_CurrentHotkeyId + 1;
				KeyValuePair<string, List<HotKey>> keyValuePair = category;
				return new HotKeyCategory(id, gameKeysCount, keyValuePair.Value);
			}), true);
		}

		private void OnGameTick(object _sender, float _e)
		{
			foreach (HotKey hotKey in this.m_Hotkeys.SelectMany((KeyValuePair<string, List<HotKey>> category) => category.Value))
			{
				KeyState keyState = KeyState.None;
				if (InputManager.GetHotkeyState(hotKey, (InputKey inputKey) => inputKey.IsPressed()))
				{
					keyState = KeyState.KeyPressed;
				}
				else if (InputManager.GetHotkeyState(hotKey, (InputKey inputKey) => inputKey.IsReleased()))
				{
					keyState = KeyState.KeyReleased;
				}
				else if (InputManager.GetHotkeyState(hotKey, (InputKey inputKey) => inputKey.IsDown()))
				{
					keyState = KeyState.KeyDown;
				}
				if (hotKey.CurrentKeyState != keyState)
				{
					Func<bool> isEnabled = hotKey.IsEnabled;
					if (isEnabled != null && isEnabled())
					{
						hotKey.CurrentKeyState = keyState;
						continue;
					}
				}
				if (keyState == KeyState.None)
				{
					hotKey.CurrentKeyState = KeyState.None;
				}
			}
		}

		private static bool GetHotkeyState(HotKey _hotKey, Func<InputKey, bool> _getState)
		{
			Key key = null;
			if (_hotKey.GameKey != null)
			{
				PropertyInfo value = InputManager.KeyProperty.Value;
				key = (((value != null) ? value.GetValue(_hotKey.GameKey) : null) as Key);
			}
			return key != null && _getState != null && _getState(key.InputKey);
		}

		private static readonly Lazy<PropertyInfo> KeyProperty = new Lazy<PropertyInfo>(() => typeof(GameKey).GetProperty("PrimaryKey", MemberExtractor.PublicMemberFlags) ?? typeof(GameKey).GetProperty("KeyboardKey", MemberExtractor.PublicMemberFlags));

		private int m_CurrentHotkeyId;

		private Dictionary<string, List<HotKey>> m_Hotkeys;

		private ISubModuleEventNotifier m_SubModuleEventNotifier;
	}
}
