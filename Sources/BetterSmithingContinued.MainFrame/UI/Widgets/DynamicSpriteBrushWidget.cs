using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.TwoDimension;

namespace BetterSmithingContinued.MainFrame.UI.Widgets
{
	public class DynamicSpriteBrushWidget : BrushWidget
	{
		[Editor(false)]
		public new Sprite Sprite
		{
			get
			{
				return base.Brush.DefaultStyle.GetLayer("Default").Sprite;
			}
			set
			{
				foreach (StyleLayer styleLayer in 
					from _style in base.Brush.Styles
					from state in GetStates(this)
					select _style.GetLayer(state) into layer
					where layer != null
					select layer)
				{
					styleLayer.Sprite = value;
				}
			}
		}

		public DynamicSpriteBrushWidget(UIContext context) : base(context)
		{
		}

		private static List<string> GetStates(Widget _caller)
		{
			if (m_StatesFieldInfo == null)
			{
				m_StatesFieldInfo = MemberExtractor.GetPrivateFieldInfo<Widget>("_states");
			}
			List<string> result;
			if ((result = (List<string>)m_StatesFieldInfo?.GetValue(_caller)) == null)
			{
				(result = new List<string>()).Add("Default");
			}
			return result;
		}

		private static FieldInfo m_StatesFieldInfo;
	}
}
