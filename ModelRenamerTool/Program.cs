using System.Text.RegularExpressions;

namespace ModelRenamerTool;

public partial class Program {

    [GeneratedRegex(@"aeg\d+_\d+")]
    private static partial Regex NameRegex();

    private string _oName, _nName;

    private static string[] _args;

    public static void Main(string[] args) {
        Print("Make sure to extract everything before renaming!");
        _args = args;
        if(_args.Length <= 0) {
            Print("No directories supplied. Press any key to exit...");
            Console.ReadKey();
            return;
        }
        _ = new Program();
    }

    public Program() {
        var match = NameRegex().Match(_args[0]);
        if (match.Success) {
            _oName = match.Value;
        } else {
            Console.WriteLine("Failed to extract name. Input name:");
            _oName = Console.ReadLine();
        }

        Print($"Current name: {_oName} - Is this correct? (y,n)");

        if(Console.ReadKey().Key != ConsoleKey.Y) {
            ResolveName();
        }

        var newNameset = false;
        while (!newNameset) {
            Print("Input new name:");
            _nName = Console.ReadLine();
            Print($"New name: {_nName} - Is this correct? (y,n)");
            if (Console.ReadKey().Key == ConsoleKey.Y) newNameset = true;
        }

        Begin();

        Print("All done! Press any key to exit...");
        Console.ReadKey();

    }

    private bool ResolveName() {
        Print("Input correct name:");
        _oName = Console.ReadLine();
        Print($"Current name: {_oName} - Is this correct? (y,n)");

        if (Console.ReadKey().Key != ConsoleKey.Y) {
            return ResolveName();
        }

        return true;
    }

    private void Begin() {
        try {
            var witchies = new List<string>();

            Print("Renaming files");

            foreach (var dir in _args) {
                foreach(var f in Directory.GetFiles(dir, "*", SearchOption.AllDirectories)) {
                    if(Path.GetFileName(f) == "_witchy-bnd4.xml") witchies.Add(f);
                    if (!f.Contains(_oName.ToUpper())) continue;
                    var curPath = f;
                    var newPath = f.Replace(_oName.ToUpper(), _nName.ToUpper());
                    File.Move(curPath, newPath);
                    Print([curPath, "=>", newPath, ""]);
                }
            }
            var s = _oName.Split('_');
            var s2 = _nName.Split("_");
            foreach (var w in witchies) {
                Print($"Modifying {w}");
                var c = File.ReadAllText(w);

                var n = Regex.Replace(c, _oName, _nName);
                n = Regex.Replace(n, _oName.ToUpper(), _nName.ToUpper());
                n = Regex.Replace(n, s[0], s2[0]);
                n = Regex.Replace(n, s[0].ToUpper(), s2[0].ToUpper());

                File.WriteAllText(w, n);
            }

            Print("Renaming directories");
            foreach(var d in _args) {
                var newPath = d.Replace(_oName, _nName);
                Directory.Move(d, newPath);
                Print([d, "=>", newPath, ""]);
            }

        } catch(Exception e) {
            Print(e.Message);
        }
    }



    private static void Print(string message) => Console.WriteLine(message);
    private static void Print(string[] messages) => Console.WriteLine(string.Join("\n", messages));
}