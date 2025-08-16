using System.Reflection;

namespace BosonWare.TerminalApp.Internal;

internal static class ReflectionUtility
{
    internal static T? GetAttribute<T>(this Type type) where T : Attribute
    {
        var attribute = type.GetCustomAttribute<T>(inherit: true);

        // If not found on the type or its base classes, check the interfaces
        if (attribute is not null)
            return attribute;

        foreach (var interfaceType in type.GetInterfaces()) {
            attribute = interfaceType.GetCustomAttribute<T>(inherit: true);
            if (attribute != null) {
                break; // Found the attribute on an interface
            }
        }

        return attribute;
    }
}
