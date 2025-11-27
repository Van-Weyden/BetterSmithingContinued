using System;
using System.Collections.Generic;
using System.Reflection;
using Bannerlord.UIExtenderEx;
using BetterSmithingContinued.Core;
using BetterSmithingContinued.Inputs.Code;
using BetterSmithingContinued.MainFrame;
using BetterSmithingContinued.Settings;
using BetterSmithingContinued.Utilities;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.WeaponCrafting.Smelting;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace BetterSmithingContinued.Main
{
	public class Main : MBSubModuleBase
	{
		protected override void OnSubModuleLoad()
		{
			List<Assembly> list = new List<Assembly>
			{
				Assembly.Load(typeof(PublicContainer).Assembly.FullName),
				Assembly.Load(typeof(InputManager).Assembly.FullName),
				Assembly.Load(typeof(SmithingManager).Assembly.FullName),
				Assembly.Load(typeof(SettingsManager).Assembly.FullName),
				Assembly.Load(typeof(Messaging).Assembly.FullName)
			};
			this.m_ModuleCoordinator = new ModuleCoordinator(list);
			this.m_ModuleCoordinator.Start();
			this.m_SubModuleEventNotifier = (SubModuleEventNotifier)this.m_ModuleCoordinator.PublicContainer.GetModule<ISubModuleEventNotifier>("");
			this.m_SubModuleEventNotifier.OnModulesLoaded();
			Harmony harmony = new Harmony("Bannerlord.BetterSmithingContinued");
			foreach (Assembly assembly in list)
			{
				harmony.PatchAll(assembly);
			}
			this.PerformManualPatches(harmony);
			this.m_UIExtender = UIExtender.Create("BetterSmithingContinued");
			this.m_UIExtender.Register(typeof(MainFrameSubModule).Assembly);
			this.m_UIExtender.Enable();
			base.OnSubModuleLoad();
		}

		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			Harmony harmony = new Harmony("Bannerlord.BetterSmithingContinued");
			MethodInfo method = MemberExtractor.GetMethodInfo<SmeltingVM>("RefreshList");
			Patches patchInfo = Harmony.GetPatchInfo(method);
			if (patchInfo != null)
			{
				foreach (Patch patch in patchInfo.Postfixes)
				{
					if (string.Equals(patch.owner, "mod.bannerlord.tweaks", StringComparison.InvariantCultureIgnoreCase))
					{
						Type declaringType = patch.PatchMethod.DeclaringType;
						if (string.Equals((declaringType != null) ? declaringType.Name : null, "RefreshListPatch"))
						{
							harmony.Unpatch(method, patch.PatchMethod);
							Messaging.DisplayMessage("BetterSmithingContinued - Unpatched BannelordTweaks 'PreventSmeltingLockedItems' patch.");
						}
					}
				}
			}
			base.OnGameStart(game, gameStarterObject);
		}

		protected override void OnSubModuleUnloaded()
		{
			this.m_ModuleCoordinator.Stop();
			base.OnSubModuleUnloaded();
		}

		protected override void OnBeforeInitialModuleScreenSetAsRoot()
		{
			InformationManager.DisplayMessage(new InformationMessage(new TextObject("{=BSC_Loaded}Loaded", null) + " Better Smithing Continued " + ModInfo.Version));
			this.m_SubModuleEventNotifier.OnBeforeInitialModuleScreenSetAsRoot();
		}

		protected override void OnApplicationTick(float _deltaTime)
		{
			this.OnGameTick(_deltaTime);
			base.OnApplicationTick(_deltaTime);
		}

		private void PerformManualPatches(Harmony _harmony)
		{
			this.m_SubModuleEventNotifier.OnPerformManualPatches(_harmony);
		}

		private void OnGameTick(float _deltaTime)
		{
			this.m_SubModuleEventNotifier.OnGameTick(_deltaTime);
		}

		private const string HarmonyName = "Bannerlord.BetterSmithingContinued";

		private ModuleCoordinator m_ModuleCoordinator;

		private SubModuleEventNotifier m_SubModuleEventNotifier;

		private UIExtender m_UIExtender;
	}
}
