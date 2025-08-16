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
            return HelpCommand.PrintCommands(Commands.Values);
        }

        var (name, args) = CommandLineParser.Parse(arguments);

        if (Commands.TryGetValue(name, out var command)) {
            return command.Command.Execute(args);
        }

        throw new ArgumentException($"Unknown command: {name}");
    }
}
