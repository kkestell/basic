namespace Basic;

public static class Program
{
    public static int Main()
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
//          var lexer = new Lexer(@"
//          # arithmetic
//
//          x = 1 + 2 - 3 * 4 / 5
//          y = 1 + (2 - (3 * (4 / 5)))
//
//          # logic
//
//          if x > 1 then
//              print(""blah"")
//          else if x < 0.5 then
//              print(""whoa"")
//          else
//              print(""meh"")
//          end if
//
//          # loops
//
//          for i = 1 to 5
//              print(i)
//              if i >= 2 then
//                  break
//              end if
//          end for
//
//          people = [""Alice"", ""Bob"", ""Carol""]
//          for person in people
//              print(person)
//          end for
//
//          j = 0
//          while j <= 10
//              j = j + 1
//          end while
//
//          k = 0
//          while true
//              k = k + 1
//              if k >= 100 then
//                  break
//              end if
//          end while
//          ");
        // var parser = new Parser(lexer);
        // var interpreter = new Interpreter();
        // while (true)
        // {
        //     var statement = parser.ParseStatement();
        //     
        //     if (statement is null)
        //         break;
        //
        //     interpreter.ExecuteStatement(statement);
        // }
        return 0;
    }
}