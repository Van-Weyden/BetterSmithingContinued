using System.Collections.Generic;

using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

using BetterSmithingContinued.Utilities;

namespace BetterSmithingContinued.Inputs.Code
{
	public class HotKeyCategory : GameKeyContext
	{
		public HotKeyCategory(string _id, int _gameKeysCount, IEnumerable<HotKey> _hotKeys)
			: base(_id, _gameKeysCount, GameKeyContext.GameKeyContextType.Default)
		{
			GameText keyName = Module.CurrentModule.GlobalTextManager.AddGameText("str_key_name");
			GameText keyDesc = Module.CurrentModule.GlobalTextManager.AddGameText("str_key_description");
			foreach (HotKey hotKey in _hotKeys)
			{
				keyName.AddVariationWithId(string.Format("{0}_{1}", _id, hotKey.ID), TextObjectUtilities.CreateTextObject(hotKey.Name, null), new List<GameTextManager.ChoiceTag>());
				keyDesc.AddVariationWithId(string.Format("{0}_{1}", _id, hotKey.ID), TextObjectUtilities.CreateTextObject(hotKey.Tooltip, null), new List<GameTextManager.ChoiceTag>());
				hotKey.GameKey = new GameKey(hotKey.ID, hotKey.GetType().Name ?? "", _id, hotKey.DefaultKey, hotKey.Category);
				base.RegisterGameKey(hotKey.GameKey, true);
			}
		}
	}
}
