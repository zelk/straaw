using System;

namespace Straaw.Framework.UiContract
{
	public interface IDirector
	{
		long DirectorId { get; set; }

		event Action<bool>OnDismissed;

		void UserDismiss(bool animated);
	}
}
