using JetBrains.Annotations;

namespace BosonWare.TerminalApp;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class CommandAttribute(string name) : Attribute
{
    public string Name { get; } = name ?? throw new ArgumentNullException(nameof(name));

    public string Description { get; set; } = "";

    public string[] Aliases { get; set; } = [];
}
