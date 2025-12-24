using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using BetterSmithingContinued.Core;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.Library;

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
			return (EventManager eventManager) => eventManager.Context.InputContext.GetMousePosition();
		});

		private static readonly Lazy<Func<EventManager, MBReadOnlyList<Widget>>>
			m_LazyGetCurrentListInvoker = new Lazy<Func<EventManager, MBReadOnlyList<Widget>>>(delegate()
		{
			FieldInfo widgetContainersInfo = MemberExtractor.GetPrivateFieldInfo<EventManager>("_widgetContainers");
            Type widgetContainerType = widgetContainersInfo.FieldType.GetElementType();
            MethodInfo methodInfo = widgetContainerType.GetMethod("GetActiveList", MemberExtractor.PublicMemberFlags);

			return delegate(EventManager eventManager) {
				Array widgetContainers = widgetContainersInfo.GetValue(eventManager) as Array;
				return methodInfo.Invoke(widgetContainers.GetValue(0), null) as MBReadOnlyList<Widget>;
			};
		});
	}
}
