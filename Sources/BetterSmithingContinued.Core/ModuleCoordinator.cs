using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BetterSmithingContinued.Core.Modules;

namespace BetterSmithingContinued.Core
{
	public class ModuleCoordinator
	{
		public IPublicContainer PublicContainer
		{
			get
			{
				return this.m_PublicContainer;
			}
		}

		public ModuleCoordinator(List<Assembly> _startupAssemblies)
		{
			this.m_StartupAssemblies = _startupAssemblies;
			this.m_Modules = new List<IModule>();
			this.m_PublicContainer = new PublicContainer("Main", null);
		}

		public void Start()
		{
			this.Initialize();
			foreach (IModule module in this.m_Modules)
			{
				module.Create(this.m_PublicContainer);
			}
			foreach (IModule module2 in this.m_Modules)
			{
				module2.Load();
			}
		}

		public void Stop()
		{
			foreach (IModule module in this.m_Modules)
			{
				module.Unload();
			}
		}

		public void Initialize()
		{
			Type[] source = (from _type in this.m_StartupAssemblies.SelectMany((Assembly assembly) => assembly.GetTypes())
			where !_type.IsAbstract && !_type.IsInterface && typeof(IModule).IsAssignableFrom(_type)
			select _type).ToArray<Type>();
			this.m_Modules.AddRange(this.InstantiateAndGetBaseModules(from _type in source
			where typeof(BetterSmithingContinued.Core.Modules.Module).IsAssignableFrom(_type)
			select _type));
		}

		private IEnumerable<IModule> InstantiateAndGetBaseModules(IEnumerable<Type> _baseModuleTypes)
		{
			List<IModule> list = new List<IModule>();
			foreach (Type type in _baseModuleTypes)
			{
				list.Add((IModule)Activator.CreateInstance(type));
			}
			return list;
		}

		private readonly List<Assembly> m_StartupAssemblies;

		private readonly PublicContainer m_PublicContainer;

		private readonly List<IModule> m_Modules;
	}
}
