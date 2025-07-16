namespace BosonWare.TerminalApp;

/// <summary>
/// Represents a minimal command with a name and description.
/// Inherits from <see cref="ICommand"/>.
/// </summary>
/// <remarks>
/// Implement this interface to define a command with basic metadata.
/// </remarks>
/// <property name="Name">
/// Gets the name of the command.
/// </property>
/// <property name="Description">
/// Gets the description of the command.
/// </property>
public interface IMinimalCommand : ICommand
{
    public string Name { get; }

    public string Description { get; }
}
