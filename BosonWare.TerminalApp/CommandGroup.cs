using BosonWare.Compares;
using BosonWare.TerminalApp.BuiltIn;

namespace BosonWare.TerminalApp;

public class CommandGroup : ICommand
{
    public Dictionary<string, RegisteredCommand> Commands { get; }
        = new(new OrdinalIgnoreCaseEqualityComparer());

    /// <inheritdoc />
    public Task Execute(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments)) {
            HelpCommand.PrintCommands(Commands.Values);

            return Task.CompletedTask;
        }

        var (name, args) = CommandLineParser.Parse(arguments);

        if (Commands.TryGetValue(name, out var command)) {
            return command.Command.Execute(args);
        }

        throw new ArgumentException($"Unknown command: {name}");
    }
}
