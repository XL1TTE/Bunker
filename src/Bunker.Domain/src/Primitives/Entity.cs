namespace Bunker.Domain.Primitives;

public abstract class Entity<TId>(TId id)
{
    public TId Id { get; init; } = id;

    public override bool Equals(object? obj) =>
        obj is Entity<TId> entity && EqualityComparer<TId>.Default.Equals(Id, entity.Id);

    public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id!);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        EqualityComparer<Entity<TId>>.Default.Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
        !(left == right);
}
