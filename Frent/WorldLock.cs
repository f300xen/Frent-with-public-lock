using System;
using System.Runtime.CompilerServices;

namespace Frent;

/// <summary>
/// A disposable struct that safely manages a lock on the World to allow for
/// manual structural changes. Guarantees that the world is properly unlocked.
/// </summary>
public readonly struct WorldLock : IDisposable
{
	private readonly World _world;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal WorldLock(World world)
	{
		_world = world;
		_world.EnterDisallowState();
	}

	/// <summary>
	/// Disposes the WorldLock.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Dispose()
	{
		_world.ExitDisallowState(null, _world.CurrentConfig.UpdateDeferredCreationEntities);
	}
}

public partial class World
{
	/// <summary>
	/// Acquires a temporary, exclusive lock on the World.
	/// </summary>
	/// <returns>An IDisposable WorldLock object.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WorldLock Lock()
	{
		return new WorldLock(this);
	}
}