using System;

namespace Straaw.Framework.Model
{
	public interface IMutableModel : IModel
	{
		IImmutableModel ToIImmutableModel();
		Type ImmutableModelType();
		Type MutableModelType();
	}
}
