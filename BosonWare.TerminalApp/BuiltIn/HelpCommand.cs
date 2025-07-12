using BosonWare.TUI;

namespace BosonWare.TerminalApp.BuiltIn;

[Command("help", Aliases = ["info", "-h", "?"], Description = "Displays help")]
public sealed class HelpCommand : ICommand
{
    public async Task Execute(string arguments)
    {
        var commands = CommandRegistry.Commands.Values
            .DistinctBy(x => x.Attribute.Name)
            .OrderBy(x => x.Attribute.Name);

        SmartConsole.WriteLine(Application.PrettyName, ConsoleColor.Green);

        SmartConsole.WriteLine("\r\nCommands:");
        foreach (var (Command, Attribute) in commands)
        {
            int longestName = commands.Select(x => x.Attribute.Name.Length).Max();

            var commandDescription = $"{(Attribute.Description.EndsWith('.') ? Attribute.Description : Attribute.Description + ".")}";

            var fullDescription = Attribute.Aliases.Length > 0
                ? $"{commandDescription} [Bright][[/][Cyan]Aliases[/]: {string.Join("[Dim],[/] ", Attribute.Aliases.Select(x => $"[Green]{x}[/]"))}[Bright]][/]"
                : commandDescription;

            string padding = "    ";
            if (Attribute.Name.Length < longestName)
            {
                for (var i = 0; i < longestName - Attribute.Name.Length; i++) padding += " ";
            }

            SmartConsole.WriteLine($"  [Green]{Attribute.Name}[/]{padding}{fullDescription}\r\n");

            await Task.Delay(100);
        }
    }
}