// using System.Numerics;
// using Raylib_cs;
//
// namespace Basic.Gui;
//
// public class Window : IWindow, IDisposable
// {
//     private readonly int _width;
//     private readonly int _height;
//     private readonly RenderTexture2D _target;
//     private readonly Texture2D _texture;
//     private readonly Font _font;
//     private readonly SpriteManager _spriteManager = new();
//     private readonly SoundManager _soundManager = new();
//
//     public Window(int width, int height)
//     {
//         _width = width;
//         _height = height;
//
//         Raylib.InitWindow(width * 2, height * 2, "BASIC");
//         Raylib.SetTargetFPS(60);
//         
//         Raylib.InitAudioDevice();
//         
//         _target = Raylib.LoadRenderTexture(width, height);
//         _texture = _target.texture;
//         Raylib.SetTextureFilter(_texture, TextureFilter.TEXTURE_FILTER_POINT);
//         
//         _font = Raylib.LoadFontEx("assets/fonts/pixel-operator.ttf", 12, null, 0);
//     }
//     
//     public void Clear()
//     {
//         Raylib.BeginTextureMode(_target);
//         Raylib.ClearBackground(Color.BLACK);
//         Raylib.EndTextureMode();
//     }
//
//     public void Present()
//     {
//         Raylib.BeginDrawing();
//         Raylib.DrawTexturePro(
//             _texture,
//             new Rectangle(0, _height, _width, -_height),
//             new Rectangle(0, 0, _width * 2, _height * 2),
//             new Vector2(0, 0),
//             0,
//             Color.WHITE
//         );
//         Raylib.EndDrawing();
//     }
//
//     public void DrawCircle(int x, int y, int r)
//     {
//         Raylib.BeginTextureMode(_target);
//         Raylib.DrawCircleLines(x, y, r, Color.WHITE);
//         Raylib.EndTextureMode();
//     }
//     
//     public void DrawText(int x, int y, string text)
//     {
//         Raylib.BeginTextureMode(_target);
//         Raylib.DrawTextEx(_font, text, new Vector2(x, y), _font.baseSize, 1, Color.WHITE);
//         //Raylib.DrawText(text, x, y, 12, Color.WHITE);
//         Raylib.EndTextureMode();
//     }
//
//     public void DrawSprite(string filename, int x, int y)
//     {
//         _spriteManager.DrawSprite(_target, filename, x, y);
//     }
//     
//     public void PlaySound(string filename)
//     {
//         _soundManager.PlaySound(filename);
//     }
//
//     public void Dispose()
//     {
//         Raylib.UnloadRenderTexture(_target);
//         Raylib.CloseWindow();
//     }
// }