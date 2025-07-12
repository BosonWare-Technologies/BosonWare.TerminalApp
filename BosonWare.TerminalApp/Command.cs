using BosonWare.TUI;
using CommandLine;

namespace BosonWare.TerminalApp;

public abstract class Command<TOptions> : ICommand
{
    public async Task Execute(string arguments)
    {
        string[] parsedArgs = CommandLineParser.SplitCommandLine(arguments); // Tokenize the input string

        var options = Parser.Default.ParseArguments<TOptions>(parsedArgs);

        if (options.Errors.Any()) {
            if (!arguments.Contains("--help", StringComparison.InvariantCultureIgnoreCase)) {
                SmartConsole.LogError("Invalid command arguments. Please check the command syntax.");
            }

            return;
        }

        var parsedOptions = options.Value;

        await Execute(parsedOptions);
    }

    public abstract Task Execute(TOptions options);
}
