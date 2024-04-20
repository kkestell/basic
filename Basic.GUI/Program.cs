using System.Threading.Tasks;
using Raylib_CsLo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Raylib.InitWindow(1280, 720, "Hello World with MultiTextBox and Status Bar");
        RayGui.GuiLoadStyleDefault();
        Raylib.SetTargetFPS(60);

        string labelText = "Click me...";
        string multiTextBoxText = "";
        string editedText = "";
        
        while (!Raylib.WindowShouldClose())
        {
            var multiTextBoxRect = new Rectangle(0, 0, 1280, 720); // Reserve 50 pixels for status bar
            if (RayGui.GuiTextBoxMulti(multiTextBoxRect, editedText, 1024, true))
            {
                multiTextBoxText = editedText;
            }
            else
            {
                editedText = multiTextBoxText;
            }
            
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Raylib.SKYBLUE);
            Raylib.DrawFPS(1280 - 100, 720 - 40);
            
            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }
}



// using Raylib_cs;
//
// namespace Basic.Gui;
//
// public class BackgroundInterpreter
// {
//     private readonly Thread _thread;
//
//     public BackgroundInterpreter(Window window)
//     {
//         _thread = new Thread(Run);
//     }
//
//     public void Start()
//     {
//         _thread.Start();   
//     }
//
//     private void Run()
//     {
//         using var window = new Window(640, 480);
//
//         var lexer = new Lexer(@"
//             WHILE TRUE THEN
//                 clear()
//                 present()
//             END WHILE
//         ");
//
//         var parser = new Parser(lexer);
//
//         var interpreter = new Interpreter(window);
//
//         while (!Raylib.WindowShouldClose())
//         {
//             try
//             {
//                 var statement = parser.ParseStatement();
//                 
//                 if (statement is null)
//                     break;
//
//                 interpreter.ExecuteStatement(statement);
//             }
//             catch (SyntaxError e)
//             {
//                 Console.WriteLine(e.Message);
//                 break;
//             }
//         }
//     }
// }
//
// public static class Program
// {
//     public static async Task Main()
//     {
//         using var window = new Window(640, 480);
//
//         var lexer = new Lexer(@"
// vx = 1
// vy = 1
// px = 320
// py = 240
// sprite = 1
//
// WHILE TRUE THEN
//     clear()
//
//     px = px + vx
//     py = py + vy
//
//     bounce_sound = random(30, 34)
//
//     FOR x = 0 TO 20 THEN
//         FOR y = 0 TO 15 THEN
//             drawSprite(""assets/sprites/sprite-371.png"", x*32, y*32)
//         END FOR
//     END FOR
//
//     IF px < 0 THEN
//         px = 0
//         vx = -vx
//         playSound(""assets/sounds/bounce-{bounce_sound}.ogg"")
//         sprite = sprite + 1
//     END IF
//
//     IF px > 640 - 32 THEN
//         px = 640 - 32
//         vx = -vx
//         playSound(""assets/sounds/bounce-{bounce_sound}.ogg"")
//         sprite = sprite + 1
//     END IF
//
//     IF py < 0 THEN
//         py = 0
//         vy = -vy
//         playSound(""assets/sounds/bounce-{bounce_sound}.ogg"")
//         sprite = sprite + 1
//     END IF
//
//     IF py > 480 - 32 THEN
//         py = 480 - 32
//         vy = -vy
//         playSound(""assets/sounds/bounce-{bounce_sound}.ogg"")
//         sprite = sprite + 1
//     END IF
//
//     IF sprite > 250 THEN
//         sprite = 1
//     END IF
//
//     drawSprite(""assets/sprites/sprite-{sprite}.png"", px, py)
//     # drawCircle(px+16, py+16, 16)
//     drawText(px-4, py+32, ""{px}x{py}"")
//     present()
// END WHILE
//         ");
//         var parser = new Parser(lexer);
//         var interpreter = new Interpreter(window);
//         
//         while (!Raylib.WindowShouldClose())
//         {
//             try
//             {
//                 var statement = parser.ParseStatement();
//                 
//                 if (statement is null)
//                     break;
//
//                 interpreter.ExecuteStatement(statement);
//             }
//             catch (SyntaxError e)
//             {
//                 Console.WriteLine(e.Message);
//                 break;
//             }
//         }
//     }
// }