using BosonWare.TUI;

namespace BosonWare.TerminalApp.BuiltIn;

[Command("version", Aliases = ["ver"], Description = "Display the current version")]
public sealed class VersionCommand : ICommand
{
    public Task Execute(string arguments)
    {
        SmartConsole.WriteLine(Application.PrettyName, ConsoleColor.Green);

        SmartConsole.WriteLine($"[Cyan]Version[/]: [Dim]{Application.Version}[/]");

        return Task.CompletedTask;
    }
}