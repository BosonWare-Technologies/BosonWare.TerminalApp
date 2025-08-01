using BosonWare.TerminalApp;
using BosonWare.TUI;
using CommandLine;

namespace TerminalApp;

[Command("Hello", Description = "Simple hello command.")] [Group("welcome", Description = "The welcome group")]
public sealed class HelloCommand : Command<HelloCommand.Options>
{
    public override Task Execute(Options options)
    {
        TUIConsole.WriteLine($"[Bright][Green]Hello[/], [Magenta]{options.User}[/]!");

        return Task.CompletedTask;
    }

    public sealed class Options
    {
        [Option('u', "user", Required = true)]
        public string User { get; set; } = "";
    }
}
