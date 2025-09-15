namespace lox
{
    internal class Program
    {
        static bool HadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                Environment.Exit(64);
            }

            if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            Run(File.ReadAllText(path));
            if (HadError) Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? line = Console.ReadLine();
                if (line == null) break;
                Run(line);
                HadError = false;
            }
        }

        private static void Run(string src)
        {
            // For now, just print the tokens.
            Scanner.Scanner scanner = new Scanner.Scanner(src);
            var tokens = scanner.ScanTokens();

            foreach (Token tok in tokens)
            {
                Console.WriteLine(tok.ToString());
            }
        }

        internal static void Error(int line, string msg)
        {
            Report(line, "", msg);
            HadError = true;
        }

        internal static void Report(int line, string where, string msg)
        {
            Console.WriteLine($"[line {line}] Error{where}: {msg}");
        }
    }
}