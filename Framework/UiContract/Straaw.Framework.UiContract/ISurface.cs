namespace Straaw.Framework.UiContract
{
	public interface ISurface
	{
		ISurfaceRegistry GetSurfaceRegistry();		
		void SetDirector(IDirector director);
		
		void WillAttachToDirector();
		void DidAttachToDirector();
		void WillDetachFromDirector();
		void DidDetachFromDirector();
	}
}
