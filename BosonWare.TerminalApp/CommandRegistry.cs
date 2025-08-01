using System.Reflection;
using BosonWare.Compares;
using BosonWare.TUI;

namespace BosonWare.TerminalApp;

public record RegisteredCommand(
    ICommand Command,
    string Name,
    string Description,
    string[]? Aliases = null)
{
    public void Deconstruct(out ICommand command, out string name, out string description, out string[] aliases)
    {
        command = Command;
        name = Name;
        description = Description;
        aliases = Aliases ?? Array.Empty<string>();
    }
}

/// <summary>
///     Provides a registry for commands, allowing registration and retrieval of <see cref="ICommand" /> instances
///     by their names or aliases. Supports case-insensitive command name matching.
/// </summary>
public static class CommandRegistry
{
    public static Dictionary<string, RegisteredCommand> Commands { get; }
        = new(new OrdinalIgnoreCaseEqualityComparer());

    public static IEnumerable<RegisteredCommand> Groups
        => Commands.Values.Where(x => x.Command is CommandGroup);

    /// <summary>
    ///     Loads command types from the specified assemblies and registers them in the <c>Commands</c> dictionary.
    ///     Each command type must be decorated with a <see cref="CommandAttribute" /> and implement <see cref="ICommand" />.
    ///     The method also registers command aliases defined in the <see cref="CommandAttribute" />.
    /// </summary>
    /// <typeparam name="AssemblyMarker">
    ///     A marker type used to identify the primary assembly to scan for command types.
    /// </typeparam>
    /// <param name="assemblyMarkers">
    ///     An array of marker types whose assemblies will also be scanned for command types.
    /// </param>
    public static void LoadCommands<AssemblyMarker>(params Type[] assemblyMarkers)
    {
        var types = typeof(AssemblyMarker).Assembly
            .GetTypes()
            .Concat(assemblyMarkers.SelectMany(marker => marker.Assembly.GetTypes()));

        foreach (var type in types) {
            if (type.GetCustomAttribute<CommandAttribute>() is not { } commandAttribute)
                continue;

            try {
                HandleCommandType(type, commandAttribute);
            }
            catch (Exception ex) {
                SmartConsole.LogError($"[{ex.GetType()}] {ex.Message}");
            }
        }
    }

    private static void HandleCommandType(Type type, CommandAttribute commandAttribute)
    {
        var command = (ICommand)Activator.CreateInstance(type)!;

        var registeredCommand = new RegisteredCommand(
            command,
            commandAttribute.Name,
            commandAttribute.Description,
            commandAttribute.Aliases);

        if (type.GetCustomAttribute<GroupAttribute>() is { } commandGroup) {
            CommandGroup? group;
            if (!Commands.TryGetValue(commandGroup.Name, out var groupInfo)) {
                group = new CommandGroup();

                groupInfo = new RegisteredCommand(
                    group,
                    commandGroup.Name,
                    commandGroup.Description);

                Commands.Add(commandGroup.Name, groupInfo);
            }
            else {
                group = (CommandGroup)groupInfo.Command;

                if (group is null) {
                    throw new Exception($"Group {commandGroup.Name} has already been registered");
                }
            }

            group.Commands.Add(registeredCommand.Name, registeredCommand);

            return;
        }

        Commands[commandAttribute.Name] = registeredCommand;

        foreach (var alias in commandAttribute.Aliases) {
            Commands[alias] = registeredCommand;
        }
    }

    public static ICommand? GetCommand(string? commandName)
    {
        if (commandName is null) return null;

        return Commands.TryGetValue(commandName, out var data)
            ? data.Command
            : null;

    }
}
