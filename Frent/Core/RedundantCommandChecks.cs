using Frent.Core.Archetypes;

namespace Frent.Core;
internal static class RedundantCommandChecks
{
	public static bool AlreadyHasComponent(World world, int entityId, 
		Archetype archetype, ComponentID componentId)
	{
		if (componentId.IsSparseComponent)
			return world.WorldSparseSetTable[componentId.SparseIndex].Has(entityId);
		return archetype.ID.HasComponent(componentId);
	}

	public static bool AlreadyHasTag(Archetype archetype, TagID tagId) 
		=> archetype.ID.HasTag(tagId);
}
