using BosonWare.TUI;
using CommandLine;

namespace BosonWare.TerminalApp;

/// <summary>
/// Represents a minimal command with a name, description, and an execution action.
/// </summary>
/// <param name="name">The name of the command.</param>
/// <param name="description">The description of the command.</param>
/// <param name="execute">The action to execute when the command is invoked, accepting a string argument.</param>
public sealed class MinimalCommand<TOptions>(
        string name,
        string description,
        Func<TOptions, Task> execute) : IMinimalCommand
{
    public string Name { get; init; } = name ?? throw new ArgumentNullException(nameof(name));

    public string Description { get; private set; } = description ?? throw new ArgumentNullException(nameof(description));

    public Func<TOptions, Task> Execute { get; init; } = execute ?? throw new ArgumentNullException(nameof(execute));

    /// <summary>
    /// Sets the description for the <see cref="MinimalCommand"/> instance.
    /// </summary>
    /// <param name="description">
    /// The description text to assign. Cannot be <c>null</c>.
    /// </param>
    /// <returns>
    /// The current <see cref="MinimalCommand"/> instance with the updated description.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="description"/> is <c>null</c>.
    /// </exception>
    public MinimalCommand<TOptions> AddDescription(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));

        return this;
    }

    Task ICommand.Execute(string arguments)
    {
        var args = CommandLineParser.SplitCommandLine(arguments);

        var options = Parser.Default.ParseArguments<TOptions>(args);

        if (options.Errors.Any()) {
            if (!arguments.Contains("--help", StringComparison.InvariantCultureIgnoreCase)) {
                SmartConsole.LogError("Invalid command arguments. Please check the command syntax.");
            }

            return Task.CompletedTask;
        }

        var parsedOptions = options.Value;

        return Execute(parsedOptions);
    }
}
