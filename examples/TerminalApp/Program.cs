using BosonWare.TerminalApp;
using BosonWare.TerminalApp.BuiltIn;
using BosonWare.TUI;
using CommandLine;

CommandRegistry.LoadCommands<Program>(typeof(IBuiltInAssemblyMarker));

var app = new ConsoleApplication("[Crimson]Boson Terminal[/] > ") {
    History = CommandHistory.CreateAsync("command_history.json").GetAwaiter().GetResult()
};

app.AddCommand("hello", (args) => {
   Console.WriteLine("Hello World!"); 
}).AddDescription("Print a simple hello world!");

app.AddCommand<Options>("welcome", (args) => {
    SmartConsole.WriteLine($"[Green]Welcome[/], [Cyan]{args.User}[/]!");
}).AddDescription("Print a welcome $User");

await app.RunAsync();

file record Options
{
    [Option('u', "user", Required = true)]
    public string User { get; set; } = "";
}
