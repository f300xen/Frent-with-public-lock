using Frent.Core;
using Frent.Core.Structures;
using Frent.Updating;
using System;
using System.Runtime.CompilerServices;

namespace Frent;

/// <summary>
/// A short-lived, stack-only accessor for an entity's data.
/// It performs the expensive entity-to-location lookup only once upon creation,
/// making subsequent component operations on the same entity significantly faster.
/// </summary>
public readonly ref struct EntityAccessor
{
	/// <summary>
	/// Holds a direct, readonly reference to the entity's resolved data location.
	/// </summary>
	private readonly ref readonly EntityLocation _location;

	/// <summary>
	/// The constructor is internal to ensure it can only be created via a controlled method like `Entity.GetAccessor()`.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal EntityAccessor(in EntityLocation location)
	{
		_location = ref location;
	}

	/// <summary>
	/// Checks if the entity has a component of Type T.
	/// This operation is faster than `Entity.Has` because it skips the initial entity lookup.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Has<T>()
	{
		return _location.Archetype.GetComponentIndex<T>() != 0;
	}

	// --- NEW METHODS ADDED HERE ---

	/// <summary>
	/// Checks if the entity has a specific tag of type T.
	/// This operation is faster than `Entity.Tagged` because it skips the initial entity lookup.
	/// </summary>
	/// <typeparam name="T">The type of the tag to check for.</typeparam>
	/// <returns>true if the entity has the tag; otherwise, false.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Tagged<T>()
	{
		return Tagged(Tag<T>.ID);
	}

	/// <summary>
	/// Checks if the entity has a specific tag.
	/// This operation is faster than `Entity.Tagged` because it skips the initial entity lookup.
	/// </summary>
	/// <param name="tagID">The ID of the tag to check for.</param>
	/// <returns>true if the entity has the tag; otherwise, false.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Tagged(TagID tagID)
	{
		return _location.Archetype.HasTag(tagID);
	}

	/// <summary>
	/// Gets the entity's component of type T.
	/// This operation is faster than `Entity.Get` because it skips the initial entity lookup.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref T Get<T>()
	{
		int compIndex = _location.Archetype.GetComponentIndex<T>();
		ComponentStorageRecord storage = _location.Archetype.Components.UnsafeArrayIndex(compIndex);
		return ref storage.UnsafeIndex<T>(_location.Index);
	}

	/// <summary>
	/// Attempts to get a component from the entity.
	/// This operation is faster than `Entity.TryGet` because it skips the initial entity lookup.
	/// </summary>
	public bool TryGet<T>(out Ref<T> value)
	{
		int compIndex = GlobalWorldTables.ComponentIndex(_location.ArchetypeID, Component<T>.ID);

		if (compIndex == 0)
		{
			value = default;
			return false;
		}

		T[] storage = UnsafeExtensions.UnsafeCast<T[]>(
			_location.Archetype.Components.UnsafeArrayIndex(compIndex).Buffer);

		value = new Ref<T>(storage, _location.Index);
		return true;
	}
}


/// <summary>
/// Extends the Entity struct with the GetAccessor method.
/// </summary>
public partial struct Entity
{
	/// <summary>
	/// Performs the entity lookup once and returns a temporary, stack-only accessor
	/// for fast, repeated component operations on this entity.
	/// </summary>
	/// <returns>An <see cref="EntityAccessor"/> for this entity.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public EntityAccessor GetAccessor()
	{
		if (World.AllowStructualChanges)
			throw new InvalidOperationException(
				"Entity.GetAccessor() cannot be used when structural changes are allowed.");

		// Perform the expensive lookup once.
		ref var location = ref AssertIsAlive(out _);

		// Return a new accessor that holds the resolved location as a reference.
		return new EntityAccessor(in location);
	}
}