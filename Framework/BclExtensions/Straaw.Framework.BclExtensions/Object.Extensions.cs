using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ObjectExtensions
{
	public static bool IsDefault<TValue>(this TValue self)
	{
		if (ReferenceEquals(self, null))
		{
			return true;
		}
		return self.Equals(default(TValue));
	}
}
