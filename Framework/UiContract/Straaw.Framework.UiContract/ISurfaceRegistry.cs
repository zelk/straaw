namespace Straaw.Framework.UiContract
{
	public interface ISurfaceRegistry
	{
		void AttachToDirector(ISurface surface, long directorId);
		void DetachFromDirector(ISurface surface, long directorId);
	}
}
