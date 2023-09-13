using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace BetterSmithingContinued.Utilities
{
	public static class EventManagerExtensions
	{
		public static void PerformActionOnWidget<T>(this EventManager _instance, Func<T, bool> _action) where T : Widget
		{
			foreach (T arg in EventManagerExtensions.m_LazyGetCurrentListInvoker.Value(_instance).OfType<T>())
			{
				if (_action(arg))
				{
					break;
				}
			}
		}

		public static Vector2 GetMousePosition(this EventManager _instance)
		{
			return EventManagerExtensions.m_LazyMousePosition.Value(_instance);
		}

		private static readonly Lazy<Func<EventManager, Vector2>> m_LazyMousePosition = new Lazy<Func<EventManager, Vector2>>(delegate()
		{
			PropertyInfo mousePositionProperty = MemberExtractor.GetPropertyInfo<EventManager>("MousePosition");
			if ((mousePositionProperty?.GetMethod) != null)
			{
				return (EventManager eventManager) => (Vector2)mousePositionProperty.GetMethod.Invoke(eventManager, null);
			}
			return (EventManager eventManager) => new Vector2((float)eventManager.InputContext.GetPointerX(), (float)eventManager.InputContext.GetPointerY());
		});

		private static readonly Lazy<Func<EventManager, List<Widget>>> m_LazyGetCurrentListInvoker = new Lazy<Func<EventManager, List<Widget>>>(delegate()
		{
			FieldInfo widgetContainer = MemberExtractor.GetPrivateFieldInfo<EventManager>("_widgetsWithUpdateContainer");
			MethodInfo methodInfo = widgetContainer?.FieldType.GetMethod("GetCurrentList", MemberExtractor.PrivateMemberFlags);
			return delegate(EventManager eventManager) {
				object obj = methodInfo?.Invoke(widgetContainer?.GetValue(eventManager), null);
				return obj as List<Widget>;
			};
		});
	}
}
