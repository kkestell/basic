namespace Basic.CLI;

public class Repl
{
    private readonly Interpreter _interpreter;
    
    public Repl()
    {
        _interpreter = new Interpreter();
    }
    
    public void Run()
    {
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrEmpty(input))
                break;
            
            var lexer = new Lexer(input);
            var parser = new Parser(lexer);                

            try
            {
                var statement = parser.ParseStatement();
                if (statement is null)
                    break;
                
                var stoppingToken = new CancellationTokenSource();
                
                var task = _interpreter.ExecuteStatement(statement, stoppingToken.Token);
                
                while (!task.IsCompleted)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.F12)
                    {
                        stoppingToken.Cancel();
                        Console.WriteLine("BREAK");
                        break;
                    }
                }
            }
            catch (SyntaxError e)
            {
                Console.WriteLine(e.Message);
                break;
            }
        }
    }
}

public static class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0)
        {
            var input = await File.ReadAllTextAsync(args[0]);
            var lexer = new Lexer(input, args[0]);
            var parser = new Parser(lexer);
            var interpreter = new Interpreter();

            try
            {
                while (true)
                {
                    var statement = parser.ParseStatement();
                    if (statement is null)
                        break;

                    await interpreter.ExecuteStatement(statement);
                }
            }
            catch (SyntaxError e)
            {
                Console.WriteLine(e.Message);
            }
            
            return;
        }
        
        var repl = new Repl();
        repl.Run();
    }
}