using System;
using System.Collections.Generic;

namespace BetterSmithingContinued.Core.Modules
{
	public abstract class Module : IModule
	{
		public IPublicContainer PublicContainer { get; private set; }

		public virtual void Create(IPublicContainer publicContainer)
		{
			this.PublicContainer = publicContainer;
		}

		public virtual void Load()
		{
		}

		public virtual void Unload()
		{
		}

		public virtual void Destroy()
		{
			for (int i = 0; i < this.m_Tokens.Count; i++)
			{
				Token token = this.m_Tokens[i];
				if (token.IsValid())
				{
					this.PublicContainer.UnregisterModule(token);
					this.m_Tokens[i] = Token.CreateInvalid();
				}
			}
		}

		protected virtual void RegisterModule<TInterface>(IModule _instance, string _name = "") where TInterface : class
		{
			Token item = this.PublicContainer.RegisterModule<TInterface>(_instance, _name);
			this.m_Tokens.Add(item);
		}

		protected virtual void RegisterModule<TInterface>(string _name = "") where TInterface : class
		{
			this.RegisterModule<TInterface>(this, _name);
		}

		private readonly List<Token> m_Tokens = new List<Token>();
	}
}
