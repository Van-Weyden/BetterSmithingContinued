using System;
using System.Collections.Generic;
using BetterSmithingContinued.Utilities;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade;

namespace BetterSmithingContinued.Inputs.Code
{
	public class HotKeyCategory : GameKeyContext
	{
		public HotKeyCategory(string _id, int _gameKeysCount, IEnumerable<HotKey> _hotKeys) : base(_id, _gameKeysCount, GameKeyContext.GameKeyContextType.Default)
		{
			GameText gameText = Module.CurrentModule.GlobalTextManager.AddGameText("str_key_name");
			GameText gameText2 = Module.CurrentModule.GlobalTextManager.AddGameText("str_key_description");
			foreach (HotKey hotKey in _hotKeys)
			{
				gameText.AddVariationWithId(string.Format("{0}_{1}", _id, hotKey.ID), TextObjectUtilities.CreateTextObject<string>(hotKey.Name, null), new List<GameTextManager.ChoiceTag>());
				gameText2.AddVariationWithId(string.Format("{0}_{1}", _id, hotKey.ID), TextObjectUtilities.CreateTextObject<string>(hotKey.Tooltip, null), new List<GameTextManager.ChoiceTag>());
				hotKey.GameKey = new GameKey(hotKey.ID, hotKey.GetType().Name ?? "", _id, hotKey.DefaultKey, hotKey.Category);
				base.RegisterGameKey(hotKey.GameKey, true);
			}
		}
	}
}
