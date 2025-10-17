using System;

namespace BetterSmithingContinued.Core
{
	public static class HashCode
	{
		public static int Combine(object _object1, object _object2)
		{
			int hashCode = _object1.GetHashCode();
			int hashCode2 = _object2.GetHashCode();
			return (hashCode << 5) + hashCode ^ hashCode2;
		}

		public static int Combine(params object[] _objects)
		{
			if (_objects.Length < 2)
			{
				throw new ArgumentException("Tried to combine HashCodes of less than 2 objects.");
			}
			int num = HashCode.Combine(_objects[0], _objects[1]);
			for (int i = 2; i < _objects.Length; i++)
			{
				num = HashCode.Combine(num, _objects[i]);
			}
			return num;
		}
	}
}
