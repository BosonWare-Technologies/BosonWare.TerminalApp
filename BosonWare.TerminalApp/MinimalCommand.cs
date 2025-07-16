namespace BosonWare.TerminalApp;

public sealed class MinimalCommand(string name, string description, Action<string> execute) : ICommand
{
    public string Name { get; init; } = name ?? throw new ArgumentNullException(nameof(name));

    public string Description { get; private set; } = description ?? throw new ArgumentNullException(nameof(description));

    public Action<string> Execute { get; init; } = execute ?? throw new ArgumentNullException(nameof(execute));

    public MinimalCommand AddDescription(string description)
    {
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        return this;
    }

    Task ICommand.Execute(string arguments)
    {
        return Task.Run(() => Execute(arguments));
    }
}
