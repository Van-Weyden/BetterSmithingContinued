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
				foreach (StyleLayer styleLayer in from _style in base.Brush.Styles
				from state in DynamicSpriteBrushWidget.GetStates(this)
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
			if (DynamicSpriteBrushWidget.m_StatesFieldInfo == null)
			{
				DynamicSpriteBrushWidget.m_StatesFieldInfo = typeof(Widget).GetField("_states", MemberExtractor.PrivateMemberFlags);
			}
			FieldInfo statesFieldInfo = DynamicSpriteBrushWidget.m_StatesFieldInfo;
			List<string> result;
			if ((result = (List<string>)((statesFieldInfo != null) ? statesFieldInfo.GetValue(_caller) : null)) == null)
			{
				(result = new List<string>()).Add("Default");
			}
			return result;
		}

		private static FieldInfo m_StatesFieldInfo;
	}
}
