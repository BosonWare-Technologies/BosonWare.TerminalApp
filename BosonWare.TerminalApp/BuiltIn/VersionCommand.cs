using BosonWare.TUI;

namespace BosonWare.TerminalApp.BuiltIn;

/// <summary>
/// Represents a command that displays the current application version in the terminal.
/// </summary>
/// <remarks>
/// This command can be invoked using "version" or its alias "ver".
/// </remarks>
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