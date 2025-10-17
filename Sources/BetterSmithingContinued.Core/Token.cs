using System;

namespace BetterSmithingContinued.Core
{
	public readonly struct Token
	{
		public static bool operator ==(Token _token1, Token _token2)
		{
			return _token1.m_Value == _token2.m_Value;
		}

		public static bool operator !=(Token _token1, Token _token2)
		{
			return _token1.m_Value != _token2.m_Value;
		}

		public static Token Create()
		{
			return new Token(Token.m_NextValue++);
		}

		public static Token Create(int _value)
		{
			return new Token(_value);
		}

		public static Token CreateInvalid()
		{
			return Token.Create(0);
		}

		private Token(int _value)
		{
			this.m_Value = _value;
		}

		public override bool Equals(object _obj)
		{
			if (_obj is Token)
			{
				Token other = (Token)_obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.m_Value;
		}

		public bool Equals(Token _other)
		{
			return this.m_Value == _other.m_Value;
		}

		public bool IsValid()
		{
			return this.m_Value != 0;
		}

		private static int m_NextValue = 1;

		private readonly int m_Value;
	}
}
