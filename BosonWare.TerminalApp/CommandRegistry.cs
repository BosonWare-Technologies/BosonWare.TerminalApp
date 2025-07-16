using BosonWare.Compares;
using BosonWare.TUI;
using System.Reflection;

namespace BosonWare.TerminalApp;

/// <summary>
/// Provides a registry for commands, allowing registration and retrieval of <see cref="ICommand"/> instances
/// by their names or aliases. Supports case-insensitive command name matching.
/// </summary>
public static class CommandRegistry
{
    public static Dictionary<string, (ICommand Command, CommandAttribute Attribute)> Commands { get; } 
        = new(new OrdinalIgnoreCaseEqualityComparer());

    /// <summary>
    /// Loads command types from the specified assemblies and registers them in the <c>Commands</c> dictionary.
    /// Each command type must be decorated with a <see cref="CommandAttribute"/> and implement <see cref="ICommand"/>.
    /// The method also registers command aliases defined in the <see cref="CommandAttribute"/>.
    /// </summary>
    /// <typeparam name="AssemblyMarker">
    /// A marker type used to identify the primary assembly to scan for command types.
    /// </typeparam>
    /// <param name="assemblyMarkers">
    /// An array of marker types whose assemblies will also be scanned for command types.
    /// </param>
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
