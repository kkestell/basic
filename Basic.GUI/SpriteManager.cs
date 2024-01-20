//
// namespace Basic.Gui;
//
// public class SpriteManager
// {
//     private readonly Dictionary<string, Texture2D> spriteCache = new Dictionary<string, Texture2D>();
//
//     public void DrawSprite(RenderTexture2D target, string filename, int x, int y)
//     {
//         try
//         {
//             if (!spriteCache.ContainsKey(filename))
//             {
//                 var texture = Raylib.LoadTexture(filename);
//                 spriteCache[filename] = texture;
//             }
//
//             var sprite = spriteCache[filename];
//             Raylib.BeginTextureMode(target);
//             Raylib.SetTextureFilter(sprite, TextureFilter.TEXTURE_FILTER_POINT);
//             Raylib.DrawTexture(sprite, x, y, Color.WHITE);
//             Raylib.EndTextureMode();
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//         }
//     }
// }