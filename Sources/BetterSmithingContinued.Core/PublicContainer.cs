using System;
using System.Collections.Generic;
using System.Linq;
using BetterSmithingContinued.Core.Modules;

namespace BetterSmithingContinued.Core
{
	public sealed class PublicContainer : IPublicContainer
	{
		public PublicContainer(string _name, PublicContainer _parent = null)
		{
			this.m_Parent = _parent;
			this.Name = _name;
			this.m_Modules = new Dictionary<string, Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>>();
			this.m_ModulesByToken = new Dictionary<Token, PublicContainer.ModuleItemKey>();
		}

		public string Name { get; }

		public IPublicContainer Parent
		{
			get
			{
				return this.m_Parent;
			}
		}

		public Token RegisterModule<T>(IModule _module, string _moduleName) where T : class
		{
			Type typeFromHandle = typeof(T);
			if (_moduleName == null)
			{
				throw new ArgumentNullException("_moduleName");
			}
			if (_module == null)
			{
				throw new ArgumentNullException("_module");
			}
			if (!typeFromHandle.IsInterface)
			{
				throw new Exception(typeFromHandle.Name + " needs to be an interface.");
			}
			Dictionary<string, Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>> modules = this.m_Modules;
			Token token;
			lock (modules)
			{
				PublicContainer.ModuleItemKey moduleItemKey = new PublicContainer.ModuleItemKey(_moduleName, typeFromHandle);
				Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem> dictionary;
				if (!this.m_Modules.TryGetValue(moduleItemKey.InterfaceFullName, out dictionary))
				{
					dictionary = new Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>();
					this.m_Modules.Add(moduleItemKey.InterfaceFullName, dictionary);
				}
				PublicContainer.ModuleItem moduleItem;
				if (dictionary.TryGetValue(moduleItemKey, out moduleItem))
				{
					throw new Exception(string.Concat(new string[]
					{
						"A component with the name [",
						_moduleName,
						"] of type [",
						typeFromHandle.Name,
						"] is already registered in the public container."
					}));
				}
				if (!typeFromHandle.IsInstanceOfType(_module))
				{
					throw new Exception("The object " + _module.GetType().FullName + " is not a " + typeFromHandle.FullName);
				}
				moduleItem = new PublicContainer.ModuleItem(_module, Token.Create());
				dictionary.Add(moduleItemKey, moduleItem);
				this.m_ModulesByToken.Add(moduleItem.Token, moduleItemKey);
				token = moduleItem.Token;
			}
			return token;
		}

		public void UnregisterModule(Token _token)
		{
			if (!_token.IsValid())
			{
				throw new ArgumentException("Token was invalid.");
			}
			Dictionary<string, Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>> modules = this.m_Modules;
			lock (modules)
			{
				PublicContainer.ModuleItemKey key;
				if (this.m_ModulesByToken.TryGetValue(_token, out key))
				{
					this.m_ModulesByToken.Remove(_token);
					Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem> dictionary;
					if (!this.m_Modules.TryGetValue(key.InterfaceFullName, out dictionary) || !dictionary.Remove(key))
					{
						throw new Exception("ModuleItem was not found.");
					}
					if (dictionary.Count == 0)
					{
						this.m_Modules.Remove(key.InterfaceFullName);
					}
				}
				else
				{
					if (this.Parent == null)
					{
						throw new Exception("ModuleItem was not found.");
					}
					this.Parent.UnregisterModule(_token);
				}
			}
		}

		public T GetModule<T>(string _moduleName = "") where T : class
		{
			Type typeFromHandle = typeof(T);
			if (!typeFromHandle.IsInterface)
			{
				throw new Exception(typeFromHandle.Name + " needs to be an interface.");
			}
			Dictionary<string, Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>> modules = this.m_Modules;
			T result;
			lock (modules)
			{
				if (string.IsNullOrWhiteSpace(_moduleName))
				{
					result = this.GetModuleFromInterfaceFullName<T>(typeFromHandle.FullName);
				}
				else
				{
					result = this.GetModuleFromKey<T>(new PublicContainer.ModuleItemKey(_moduleName, typeFromHandle));
				}
			}
			return result;
		}

		private T GetModuleFromInterfaceFullName<T>(string _interfaceFullName) where T : class
		{
			Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem> source;
			if (!this.m_Modules.TryGetValue(_interfaceFullName, out source))
			{
				PublicContainer parent = this.m_Parent;
				if (parent == null)
				{
					return default(T);
				}
				return parent.GetModuleFromInterfaceFullName<T>(_interfaceFullName);
			}
			else
			{
				PublicContainer.ModuleItem value = source.FirstOrDefault<KeyValuePair<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>>().Value;
				T t = ((value != null) ? value.Module : null) as T;
				if (t == null)
				{
					throw new Exception(string.Concat(new string[]
					{
						"A component with the given type [",
						_interfaceFullName,
						"] does not exist in the public container: Container: [",
						this.Name,
						"]."
					}));
				}
				return t;
			}
		}

		private T GetModuleFromKey<T>(PublicContainer.ModuleItemKey _key) where T : class
		{
			Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem> dictionary;
			PublicContainer.ModuleItem moduleItem;
			if (!this.m_Modules.TryGetValue(_key.InterfaceFullName, out dictionary) || !dictionary.TryGetValue(_key, out moduleItem))
			{
				PublicContainer parent = this.m_Parent;
				if (parent == null)
				{
					return default(T);
				}
				return parent.GetModuleFromKey<T>(_key);
			}
			else
			{
				T t = moduleItem.Module as T;
				if (t == null)
				{
					throw new Exception(string.Concat(new string[]
					{
						"A component with the given name [",
						_key.Name,
						"] and type [",
						_key.InterfaceFullName,
						"] does not exist in the public container: Container: [",
						this.Name,
						"]."
					}));
				}
				return t;
			}
		}

		private readonly Dictionary<string, Dictionary<PublicContainer.ModuleItemKey, PublicContainer.ModuleItem>> m_Modules;

		private readonly Dictionary<Token, PublicContainer.ModuleItemKey> m_ModulesByToken;

		private readonly PublicContainer m_Parent;

		private class ModuleItem
		{
			public object Module { get; }

			public Token Token { get; }

			public ModuleItem(object _module, Token _token)
			{
				this.Module = _module;
				this.Token = _token;
			}
		}

		private readonly struct ModuleItemKey
		{
			public string Name { get; }

			public string InterfaceFullName { get; }

			public ModuleItemKey(string _name, Type _interface)
			{
				this.Name = _name;
				this.InterfaceFullName = _interface.FullName;
				this.m_HashCode = HashCode.Combine(this.Name, this.InterfaceFullName);
			}

			public override int GetHashCode()
			{
				return this.m_HashCode;
			}

			public override string ToString()
			{
				return "Name:" + this.Name + " Type:" + this.InterfaceFullName;
			}

			private readonly int m_HashCode;
		}
	}
}
