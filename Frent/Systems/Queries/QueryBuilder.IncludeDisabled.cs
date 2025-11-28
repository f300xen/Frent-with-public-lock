using System.Diagnostics;

namespace Frent.Systems.Queries;

/// <inheritdoc cref="IQueryBuilder"/>
public static class QueryBuilderExtensions{
	/// <inheritdoc cref="IQueryBuilder"/>
	public static QueryIncludeDisabled<T> IncludeDisabled<T>(this T builder)
		where T : struct, IQueryBuilder
		=> new QueryIncludeDisabled<T>(builder.World);
}

/// <inheritdoc cref="IQueryBuilder"/>
public readonly struct QueryIncludeDisabled<TRest>(World world) : IQueryBuilder
	where TRest : struct, IQueryBuilder
{
	/// <inheritdoc cref="IQueryBuilder"/>
	public World World { get; init; } = world;

	/// <inheritdoc cref="IQueryBuilder"/>
	public void AddRules(List<Rule> rules)
	{
		rules.Add(Rule.IncludeDisabledRule);
		default(TRest).AddRules(rules);
	}

	/// <summary>
	/// Excludes entities with the tag <typeparamref name="N"/> from this query.
	/// </summary>
	public readonly QueryUntagged<N, QueryIncludeDisabled<TRest>> Untagged<N>() => new(World);

	/// <summary>
	/// Includes entities with the tag <typeparamref name="N"/> in this query.
	/// </summary>
	public readonly QueryTagged<N, QueryIncludeDisabled<TRest>> Tagged<N>() => new(World);

	/// <summary>
	/// Includes entities with the component <typeparamref name="N"/> in this query.
	/// </summary>
	public readonly QueryWith<N, QueryIncludeDisabled<TRest>> With<N>() => new(World);

	/// <summary>
	/// Excludes entities with the component <typeparamref name="N"/> from this query.
	/// </summary>
	public readonly QueryWithout<N, QueryIncludeDisabled<TRest>> Without<N>() => new(World);

	/// <inheritdoc cref="IQueryBuilder"/>
	public readonly Query Build() => World.BuildQuery<QueryIncludeDisabled<TRest>>();
}