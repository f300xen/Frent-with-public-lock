namespace Frent;

public partial class World
{
	/// <summary>
	/// Manually locks the world. Structural changes (entity creation/deletion, component addition/removal)
	/// occurring within the scope will be deferred until the returned object is disposed.
	/// </summary>
	/// <returns>An disposable object that releases the lock.</returns>
	public WorldLock Lock() => new(this);
	/// <summary>
	/// A disposable structure that maintains a lock on the World.
	/// </summary>
	public readonly struct WorldLock : IDisposable
	{
		private readonly World _world;
		internal WorldLock(World world)
		{
			_world = world;
			_world.EnterDisallowState();
		}
		/// <inheritdoc/>
		public void Dispose()
		{
			_world.ExitDisallowState(null, updateDeferredEntities: false);
		}
	}
}