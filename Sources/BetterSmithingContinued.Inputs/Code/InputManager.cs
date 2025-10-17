using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using TaleWorlds.InputSystem;
using TaleWorlds.Library;

using BetterSmithingContinued.Core;

namespace BetterSmithingContinued.Inputs.Code
{
	public class InputManager : Core.Modules.Module, IInputManager
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
			this.m_CurrentHotkeyId = HotKeyManager.GetAllCategories().
				SelectMany(
					(GameKeyContext context) => (context?.RegisteredGameKeys) ?? new MBReadOnlyList<GameKey>(new List<GameKey>())
				).Max(delegate(GameKey gameKey) {
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
			List<GameKeyContext> contexts = HotKeyManager.GetAllCategories().ToList();
			contexts.AppendList(m_Hotkeys.Select(delegate (KeyValuePair<string, List<HotKey>> category) {
				string id = "BetterSmithingContinued";
				int gameKeysCount = m_CurrentHotkeyId + 1;
				KeyValuePair<string, List<HotKey>> keyValuePair = category;
				GameKeyContext context = new HotKeyCategory(id, gameKeysCount, keyValuePair.Value);
				return context;
			}).ToList());
			HotKeyManager.RegisterInitialContexts(contexts, true);
		}

		private void OnGameTick(object _sender, float _e)
		{
			foreach (HotKey hotKey in this.m_Hotkeys.SelectMany((KeyValuePair<string, List<HotKey>> category) => category.Value))
			{
				KeyState keyState = GetHotkeyState(hotKey);
				if (hotKey.CurrentKeyState != keyState)
				{
					if (keyState == KeyState.None || (hotKey.IsEnabled != null && hotKey.IsEnabled()))
					{
						hotKey.CurrentKeyState = keyState;
					}
				}
			}
		}

		private static KeyState GetHotkeyState(HotKey _hotKey)
		{
			KeyState keyState = KeyState.None;
			if (_hotKey.GameKey != null)
			{
				Key key = KeyProperty.Value?.GetValue(_hotKey.GameKey) as Key;
				if (key != null)
				{
					if (key.InputKey.IsPressed())
					{
						keyState = KeyState.KeyPressed;
					}
					else if (key.InputKey.IsReleased())
					{
						keyState = KeyState.KeyReleased;
					}
					else if (key.InputKey.IsDown())
					{
						keyState = KeyState.KeyDown;
					}
				}
			}
			return keyState;
		}

		private static readonly Lazy<PropertyInfo> KeyProperty = new Lazy<PropertyInfo>(() => {
			return MemberExtractor.GetPropertyInfo<GameKey>("PrimaryKey") ?? MemberExtractor.GetPropertyInfo<GameKey>("KeyboardKey");
		});

		private int m_CurrentHotkeyId;
		private Dictionary<string, List<HotKey>> m_Hotkeys;
		private ISubModuleEventNotifier m_SubModuleEventNotifier;
	}
}
