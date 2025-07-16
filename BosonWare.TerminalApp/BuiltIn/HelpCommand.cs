using BosonWare.TUI;
using CommandInfo = (string CommandName, string Description, string[] Aliases);

namespace BosonWare.TerminalApp.BuiltIn;

[Command("help", Aliases = ["info", "-h", "?"], Description = "Displays help")]
public sealed class HelpCommand : ICommand
{
    public static List<CommandInfo> GetCommands()
    {
        List<CommandInfo> commands = [];
        
        commands.AddRange(CommandRegistry.Commands.Values.DistinctBy(x => x.Attribute.Name)
            .OrderBy(x => x.Attribute.Name)
            .Select(registryCommand => (registryCommand.Attribute.Name, registryCommand.Attribute.Description, registryCommand.Attribute.Aliases)));

        commands.AddRange(ConsoleApplication.Current.MinimalCommands
            .Select(minimalCommand => (CommandInfo)(minimalCommand.Name, minimalCommand.Description, [])));

        return commands;
    }
    
    public async Task Execute(string arguments)
    {
        var commands = GetCommands();
        
        SmartConsole.WriteLine(Application.PrettyName, ConsoleColor.Green);

        SmartConsole.WriteLine("\r\nCommands:");
        foreach (var (name, description, aliases) in commands)
        {
            var longestName = commands.Select(x => x.CommandName.Length).Max();

            var commandDescription = $"{(description.EndsWith('.') ? description : description + ".")}";

            var fullDescription = aliases.Length > 0
                ? $"{commandDescription} [Bright][[/][Cyan]Aliases[/]: {string.Join("[Dim],[/] ", aliases.Select(x => $"[Green]{x}[/]"))}[Bright]][/]"
                : commandDescription;

            var padding = "    ";
            
            if (name.Length < longestName)
            {
                for (var i = 0; i < longestName - name.Length; i++) padding += " ";
            }

            SmartConsole.WriteLine($"  [Green]{name}[/]{padding}{fullDescription}\r\n");

            await Task.Delay(100);
        }
    }
}
