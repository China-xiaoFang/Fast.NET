using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Mapster.Utils;

internal static class DynamicTypeGenerator
{
    private const string DynamicAssemblyName = "Mapster.Dynamic";

    private static readonly AssemblyBuilder _assemblyBuilder =
#if NET40
            AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(DynamicAssemblyName), AssemblyBuilderAccess.Run);
#else
        AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(DynamicAssemblyName), AssemblyBuilderAccess.Run);
#endif
    private static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("Mapster.Dynamic");
    private static readonly ConcurrentDictionary<Type, Type> _generated = new ConcurrentDictionary<Type, Type>();
    private static int _generatedCounter;

    public static Type GetTypeForInterface(Type interfaceType, bool ignoreError)
    {
        try
        {
            return GetTypeForInterface(interfaceType);
        }
        catch (Exception)
        {
            if (ignoreError)
                return null;
            throw;
        }
    }

    public static Type GetTypeForInterface(Type interfaceType)
    {
        if (!interfaceType.GetTypeInfo().IsInterface)
        {
            const string msg = "Cannot create dynamic type for {0}, because it is not an interface.\n" +
                               "Target type full name: {1}";
            throw new InvalidOperationException(string.Format(msg, interfaceType.Name, interfaceType.FullName));
        }

        if (!interfaceType.GetTypeInfo().IsVisible)
        {
            const string msg = "Cannot adapt to interface {0}, because it is not accessible outside its assembly.\n" +
                               "Interface full name: {1}";
            throw new InvalidOperationException(string.Format(msg, interfaceType.Name, interfaceType.FullName));
        }

        return _generated.GetOrAdd(interfaceType, CreateTypeForInterface);
    }

    private static Type CreateTypeForInterface(Type interfaceType)
    {
        var builder = _moduleBuilder.DefineType("GeneratedType_" + Interlocked.Increment(ref _generatedCounter));

        var args = new List<FieldBuilder>();
        var propCount = 0;
        var hasReadonlyProps = false;

        foreach (var currentInterface in interfaceType.GetAllInterfaces())
        {
            builder.AddInterfaceImplementation(currentInterface);
            foreach (var prop in currentInterface.GetProperties())
            {
                propCount++;
                var propField = builder.DefineField("_" + MapsterHelper.CamelCase(prop.Name), prop.PropertyType,
                    FieldAttributes.Private);
                CreateProperty(currentInterface, builder, prop, propField);
                if (!prop.CanWrite)
                    hasReadonlyProps = true;
                args.Add(propField);
            }

            foreach (var method in currentInterface.GetMethods())
            {
                // MethodAttributes.SpecialName are methods for property getters and setters.
                if (!method.Attributes.HasFlag(MethodAttributes.SpecialName))
                {
                    CreateMethod(builder, method);
                }
            }
        }

        if (propCount == 0)
            throw new InvalidOperationException(
                $"No default constructor for type '{interfaceType.Name}', please use 'ConstructUsing' or 'MapWith'");

        if (hasReadonlyProps)
        {
            var ctorBuilder = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                args.Select(it => it.FieldType).ToArray());
            var ctorIl = ctorBuilder.GetILGenerator();
            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];
                ctorBuilder.DefineParameter(i + 1, ParameterAttributes.None, arg.Name.Substring(1));
                ctorIl.Emit(OpCodes.Ldarg_0);
                ctorIl.Emit(OpCodes.Ldarg_S, i + 1);
                ctorIl.Emit(OpCodes.Stfld, arg);
            }

            ctorIl.Emit(OpCodes.Ret);
        }

#if NETSTANDARD2_0 || NET6_0_OR_GREATER
        return builder.CreateTypeInfo()!;
#elif NETSTANDARD1_3
            return builder.CreateTypeInfo().AsType();
#else
            return builder.CreateType();
#endif
    }

    private static void CreateProperty(Type interfaceType, TypeBuilder builder, PropertyInfo prop, FieldBuilder propField)
    {
        const BindingFlags interfacePropMethodFlags = BindingFlags.Instance | BindingFlags.Public;
        // The property set and get methods require a special set of attributes.
        const MethodAttributes classPropMethodAttrs = MethodAttributes.Public | MethodAttributes.Virtual |
                                                      MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        var propBuilder = builder.DefineProperty(prop.Name, PropertyAttributes.None, prop.PropertyType, null);

        if (prop.CanRead)
        {
            // Define the "get" accessor method for property.
            var getMethodName = "get_" + prop.Name;
            var propGet = builder.DefineMethod(getMethodName, classPropMethodAttrs, prop.PropertyType, null);
            var propGetIl = propGet.GetILGenerator();
            propGetIl.Emit(OpCodes.Ldarg_0);
            propGetIl.Emit(OpCodes.Ldfld, propField);
            propGetIl.Emit(OpCodes.Ret);

            var interfaceGetMethod = interfaceType.GetMethod(getMethodName, interfacePropMethodFlags)!;
            builder.DefineMethodOverride(propGet, interfaceGetMethod);
            propBuilder.SetGetMethod(propGet);
        }

        if (prop.CanWrite)
        {
            // Define the "set" accessor method for property.
            var setMethodName = "set_" + prop.Name;
            var propSet = builder.DefineMethod(setMethodName, classPropMethodAttrs, null, new[] {prop.PropertyType});
            var propSetIl = propSet.GetILGenerator();
            propSetIl.Emit(OpCodes.Ldarg_0);
            propSetIl.Emit(OpCodes.Ldarg_1);
            propSetIl.Emit(OpCodes.Stfld, propField);
            propSetIl.Emit(OpCodes.Ret);

            var interfaceSetMethod = interfaceType.GetMethod(setMethodName, interfacePropMethodFlags)!;
            builder.DefineMethodOverride(propSet, interfaceSetMethod);
            propBuilder.SetSetMethod(propSet);
        }
    }

    private static void CreateMethod(TypeBuilder builder, MethodInfo interfaceMethod)
    {
        Type[] parameterTypes = null;
        var parameters = interfaceMethod.GetParameters();
        if (parameters.Length > 0)
        {
            parameterTypes = new Type[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                parameterTypes[i] = parameters[i].ParameterType;
            }
        }

        var classMethod = builder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public | MethodAttributes.Virtual,
            interfaceMethod.CallingConvention, interfaceMethod.ReturnType, parameterTypes);
        var classMethodIl = classMethod.GetILGenerator();
        classMethodIl.ThrowException(typeof(NotImplementedException));

        builder.DefineMethodOverride(classMethod, interfaceMethod);
    }
}