// using Raylib_cs;
//
// namespace Basic.Gui;
//
// public class SoundManager
// {
//     private readonly Dictionary<string, Sound> soundCache = new Dictionary<string, Sound>();
//
//     public void PlaySound(string filename)
//     {
//         try
//         {
//             if (!soundCache.ContainsKey(filename))
//             {
//                 var sound = Raylib.LoadSound(filename);
//                 soundCache[filename] = sound;
//             }
//
//             Raylib.PlaySound(soundCache[filename]);
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//         }
//     }
// }