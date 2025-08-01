using BosonWare;
using BosonWare.TerminalApp;
using BosonWare.TerminalApp.BuiltIn;

Application.Initialize<Program>();

CommandRegistry.LoadCommands<Program>(typeof(IBuiltInAssemblyMarker));

var app = new ConsoleApplication("[Crimson]Boson Terminal[/] > ") {
    History = CommandHistory.CreateAsync("command_history.json").GetAwaiter().GetResult()
};

await app.RunAsync();
