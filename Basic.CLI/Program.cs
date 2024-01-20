namespace Basic.CLI;

public static class Program
{
    public static async Task Main()
    {
        var interpreter = new Interpreter();
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
                
                var task = interpreter.ExecuteStatement(statement, stoppingToken.Token);
                
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