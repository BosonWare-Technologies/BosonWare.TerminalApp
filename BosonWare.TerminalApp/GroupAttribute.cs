namespace BosonWare.TerminalApp;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public sealed class GroupAttribute(string name) : Attribute
{
    public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));

    public string Description { get; set; } = string.Empty;
}
