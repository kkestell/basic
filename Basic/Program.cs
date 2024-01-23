namespace Basic;

public static class Program
{
    public static async Task Main()
    {
        // var lexer = new Lexer(@"
        //     x = 1 + 2 * 3
        //     y = x * 10
        //     z = 0.7
        //     print(y)
        //     if z > 1 then
        //         print(""blah"")
        //     else if z < 0.5 then
        //         print(""whoa"")
        //     else
        //         if y > 10 then
        //             print(""yay"")
        //         else
        //             print(""meh"")
        //         end if
        //     end if
        //     for i = 1 to 5 then
        //         print(i)
        //     end for
        // ");
        var lexer = new Lexer("""
          # arithmetic

          x = 1 + 2 - 3 * 4 / 5
          y = 1 + (2 - (3 * (4 / 5)))
          print(x)
          print(y)

          # logic

          IF x > 1 THEN
              print("blah")
          ELSE IF x < 0.5 THEN
              print("whoa")
          ELSE
              print("meh")
          END IF

          FOR i = 1 TO 5 THEN
              print(i)
          END FOR

          x = 0
          WHILE x < 10 THEN
              x = x + 1
              print(x)
          END WHILE

          name = "Alice"
          print("Hello, {name}!")
          
          x = 10
          WHILE TRUE THEN
              print(x)
              x = x - 1
              IF x < 0 THEN
                  BREAK
              END IF
          END WHILE

        """);
        var parser = new Parser(lexer);
        var interpreter = new Interpreter();
        while (true)
        {
            var statement = parser.ParseStatement();

            if (statement is null)
                break;

            await interpreter.ExecuteStatement(statement);
        }
        return;
    }
}