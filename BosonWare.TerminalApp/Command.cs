using BosonWare.TUI;
using CommandLine;
using JetBrains.Annotations;

namespace BosonWare.TerminalApp;

/// <summary>
///     Represents an abstract base class for commands with typed options.
/// </summary>
/// <typeparam name="TOptions">The type of options to be parsed and passed to the command.</typeparam>
[PublicAPI]
public abstract class Command<TOptions> : ICommand
{
    /// <summary>
    ///     Executes the command with the specified argument string.
    ///     Parses the arguments, validates them, and invokes the command logic asynchronously.
    ///     Logs an error if the arguments are invalid and help is not requested.
    /// </summary>
    /// <param name="arguments">A string containing the command-line arguments to be parsed and executed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Execute(string arguments)
    {
        var parsedArgs = CommandLineParser.SplitCommandLine(arguments); // Tokenize the input string

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
