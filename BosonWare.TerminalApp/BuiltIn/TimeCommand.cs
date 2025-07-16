using BosonWare.TUI;
using CommandLine;

namespace BosonWare.TerminalApp.BuiltIn;

/// <summary>
/// Represents a command that displays the current local or UTC time in the terminal.
/// </summary>
/// <remarks>
/// The <c>TimeCommand</c> provides an option to display the time in UTC format.
/// </remarks>
[Command("time", Description = "Displays the current local time.")]
public sealed class TimeCommand : Command<TimeCommand.Options>
{
    public class Options
    {
        [Option('u', "utc", Required = false, HelpText = "Display UTC time.")]
        public bool Utc { get; set; } = false;
    }

    public override Task Execute(Options options)
    {
        var now = options.Utc ? DateTime.UtcNow : DateTime.Now;

        SmartConsole.WriteLine($"[Green]Time[Dim]:[/] {now.ToLongDateString()} {now.ToLongTimeString()}{(options.Utc ? " UTC" : "")}");

        return Task.CompletedTask;
    }
}