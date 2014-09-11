using System;

namespace Straaw.Framework.Model
{
	public interface IImmutableModel : IModel
	{
		IMutableModel ToIMutableModel();
		Type ImmutableModelType();
		Type MutableModelType();
	}
}
