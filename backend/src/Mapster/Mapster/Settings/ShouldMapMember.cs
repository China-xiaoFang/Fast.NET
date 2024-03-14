using System.Reflection;
using System.Runtime.CompilerServices;
using Mapster.Models;

namespace Mapster;

public static class ShouldMapMember
{
    public static readonly Func<IMemberModel, MemberSide, bool?> AllowNonPublic = (model, _) =>
        (model.AccessModifier & AccessModifier.NonPublic) == 0
            ? null
            : !(model.Info is FieldInfo) || !model.HasCustomAttribute<CompilerGeneratedAttribute>();

    public static readonly Func<IMemberModel, MemberSide, bool?> AllowPublic = (model, _) =>
        model.AccessModifier == AccessModifier.Public ? true : null;

    public static readonly Func<IMemberModel, MemberSide, bool?> IgnoreAdaptIgnore = (model, side) =>
    {
        var ignoreAttr = model.GetCustomAttributeFromData<AdaptIgnoreAttribute>();
        if (ignoreAttr == null)
            return null;
        return ignoreAttr.Side == null || ignoreAttr.Side == side ? false : null;
    };

    public static readonly Func<IMemberModel, MemberSide, bool?> AllowAdaptMember = (model, side) =>
    {
        var memberAttr = model.GetCustomAttributeFromData<AdaptMemberAttribute>();
        if (memberAttr == null)
            return null;
        return memberAttr.Side == null || memberAttr.Side == side ? true : null;
    };
}