using System;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Core.Modules;
using HarmonyLib;

namespace BetterSmithingContinued.MainFrame.Patches
{
	public class CustomPatchManager : BetterSmithingContinued.Core.Modules.Module
	{
		public override void Load()
		{
			base.Load();
			this.m_SubModuleEventNotifier = base.PublicContainer.GetModule<ISubModuleEventNotifier>("");
			this.m_SubModuleEventNotifier.PerformManualPatches += this.OnPerformManualPatches;
		}

		private void OnPerformManualPatches(object _sender, Harmony _harmony)
		{
			Type[] array = (from _type in typeof(CustomPatchManager).Assembly.GetTypes()
			where !_type.IsAbstract && typeof(HarmonyCustomPatches).IsAssignableFrom(_type)
			select _type).ToArray<Type>();
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo method = array[i].GetMethod("RegisterCustomPatches", MemberExtractor.StaticPublicMemberFlags);
				if (method != null)
				{
					method.Invoke(null, new object[]
					{
						_harmony
					});
				}
			}
		}

		private ISubModuleEventNotifier m_SubModuleEventNotifier;
	}
}
