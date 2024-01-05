using System.Runtime.CompilerServices;

namespace Mapster.Utils;

public readonly struct ReferenceTuple : IEquatable<ReferenceTuple>
{
    public object Reference { get; }
    public Type DestinationType { get; }

    public ReferenceTuple(object reference, Type destinationType)
    {
        Reference = reference;
        DestinationType = destinationType;
    }

    public override bool Equals(object obj)
    {
        return obj is ReferenceTuple other && Equals(other);
    }

    public bool Equals(ReferenceTuple other)
    {
        return ReferenceEquals(Reference, other.Reference) && DestinationType == other.DestinationType;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (RuntimeHelpers.GetHashCode(Reference) * 397) ^ DestinationType.GetHashCode();
        }
    }

    public static bool operator ==(ReferenceTuple left, ReferenceTuple right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ReferenceTuple left, ReferenceTuple right)
    {
        return !(left == right);
    }
}