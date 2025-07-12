using BosonWare.TUI;
using System.Reflection;

namespace BosonWare.TerminalApp;

public static class CommandRegistry
{
    private sealed class EqualityComparer : IEqualityComparer<string>
    {
        bool IEqualityComparer<string>.Equals(string? x, string? y)
        {
            if (x is null && y is null) return true;

            if (x is null || y is null) return false;

            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        int IEqualityComparer<string>.GetHashCode(string obj) => obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public static Dictionary<string, (ICommand Command, CommandAttribute Attribute)> Commands { get; } 
        = new(new EqualityComparer());

    public static void LoadCommands<AssemblyMarker>(params Type[] assemblyMarkers)
    {
        var types = typeof(AssemblyMarker).Assembly
            .GetTypes()
            .Concat(assemblyMarkers.SelectMany(marker => marker.Assembly.GetTypes()));

        foreach (var type in types) {
            var commandAttribute = type.GetCustomAttribute<CommandAttribute>();

            if (commandAttribute is not null) {
                try {
                    var command = (ICommand)Activator.CreateInstance(type)!;

                    Commands[commandAttribute.Name] = (command, commandAttribute);

                    foreach (var alias in commandAttribute.Aliases) {
                        Commands[alias] = (command, commandAttribute);
                    }
                }
                catch (Exception ex) {
                    SmartConsole.LogError($"[{ex.GetType()}] {ex.Message}");
                }
            }
        }
    }

    public static ICommand? GetCommand(string? commandName)
    {
        if (commandName is null) return null;

        if (Commands.TryGetValue(commandName, out var data)) {
            return data.Command;
        }

        return null;
    }
}
