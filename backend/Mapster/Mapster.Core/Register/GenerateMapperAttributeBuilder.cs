using System.Reflection;

namespace Mapster;

public class GenerateMapperAttributeBuilder
{
    public GenerateMapperAttribute Attribute { get; }
    public HashSet<Type> Types { get; } = new HashSet<Type>();

    public GenerateMapperAttributeBuilder(GenerateMapperAttribute attribute)
    {
        Attribute = attribute;
    }

    public GenerateMapperAttributeBuilder ForTypes(params Type[] types)
    {
        Types.UnionWith(types);
        return this;
    }

    public GenerateMapperAttributeBuilder ForAllTypesInNamespace(Assembly assembly, string @namespace)
    {
        Types.UnionWith(assembly.GetTypes().Where(it =>
            (it.Namespace == @namespace || it.Namespace?.StartsWith(@namespace + '.') == true) && !it.Name.Contains('<')));
        return this;
    }

    public GenerateMapperAttributeBuilder ForType<T>()
    {
        Types.Add(typeof(T));
        return this;
    }

    public GenerateMapperAttributeBuilder ExcludeTypes(params Type[] types)
    {
        Types.ExceptWith(types);
        return this;
    }

    public GenerateMapperAttributeBuilder ExcludeTypes(Predicate<Type> predicate)
    {
        Types.RemoveWhere(predicate);
        return this;
    }
}