namespace Basic;

public interface IWindow
{
    public void Clear();
    public void Present();
    public void DrawCircle(int x, int y, int r);
    public void DrawText(int x, int y, string text);
    public void DrawSprite(string filename, int x, int y);
    public void PlaySound(string filename);
}