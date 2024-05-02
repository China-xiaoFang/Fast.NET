namespace Mapster;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AdaptIgnoreAttribute : Attribute
{
    public MemberSide? Side { get; set; }

    public AdaptIgnoreAttribute()
    {
    }

    public AdaptIgnoreAttribute(MemberSide side)
    {
        Side = side;
    }
}