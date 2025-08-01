using BosonWare.TUI;

namespace BosonWare.TerminalApp.BuiltIn;

[Command("help", Aliases = ["info", "-h", "?"], Description = "Displays help")]
public sealed class HelpCommand : ICommand
{
    /// <inheritdoc />
    public async Task Execute(string arguments)
    {
        var commands = GetCommands(arguments);

        await PrintCommands(commands);
    }

    public static ICollection<RegisteredCommand> GetCommands(string groupName)
    {
        if (string.IsNullOrEmpty(groupName)) {
            List<RegisteredCommand> commands = [];

            commands.AddRange(CommandRegistry.Commands.Values.DistinctBy(x => x.Name)
                .OrderBy(x => x.Name));

            commands.AddRange(ConsoleApplication.Current.MinimalCommands
                .Select(minimalCommand => new RegisteredCommand(minimalCommand, minimalCommand.Name, minimalCommand.Description)));

            return commands;
        }

        var group = CommandRegistry.Groups
            .FirstOrDefault(x => x.Name.Equals(groupName, StringComparison.InvariantCultureIgnoreCase));

        if (group is null) {
            throw new ArgumentException($"Unknown group: {groupName}");
        }

        return ((CommandGroup)group.Command).Commands.Values;
    }

    public static async Task PrintCommands(ICollection<RegisteredCommand> commands)
    {
        SmartConsole.WriteLine("\r\nCommands:");
        foreach (var (_, name, description, aliases) in commands) {
            var longestName = commands.Select(x => x.Name.Length).Max();

            var commandDescription = $"{(description.EndsWith('.') ? description : description + ".")}";

            var fullDescription = aliases.Length > 0
                ? $"{commandDescription} [Bright][[/][Cyan]Aliases[/]: {string.Join("[Dim],[/] ", aliases.Select(x => $"[Green]{x}[/]"))}[Bright]][/]"
                : commandDescription;

            var padding = "    ";

            if (name.Length < longestName) {
                for (var i = 0; i < longestName - name.Length; i++) padding += " ";
            }

            SmartConsole.WriteLine($"  [Green]{name}[/]{padding}{fullDescription}\r\n");

            await Task.Delay(100);
        }
    }
}
