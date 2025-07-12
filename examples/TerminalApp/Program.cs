using BosonWare.TerminalApp;
using BosonWare.TerminalApp.BuiltIn;

CommandRegistry.LoadCommands<Program>(typeof(IBuiltInAssemblyMarker));

var app = new ConsoleApplication() {
    Prompt = "[Crimson]Boson Terminal[/] > ",
    History = CommandHistory.CreateAsync("command_history.json").GetAwaiter().GetResult()
};

await app.RunAsync();
