using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DontLikePoetry;

public class Floor
{
    // posição
    // tamanho
    // retângulo de colisão
    // método para desenhar

    private Rectangle _floor;
    private Texture2D texture;
    private Color[] color;

    public Floor(int x, int y, int width, int height)
    {
        _floor = new Rectangle(x, y, width, height);
    }

    public Rectangle Bound
    {
        get
        {
            return _floor;
        }
        set
        {
            _floor = value;
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice, int wallWidth, int wallHeight)
    {
        texture = new Texture2D(graphicsDevice, wallWidth, wallHeight);
        color = Enumerable.Repeat(Color.White, wallWidth*wallHeight).ToArray();
        texture.SetData(color);
    }
    public void Move(int x, int y)
    {
        _floor.Y = y;
        _floor.X = x;
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle ator)
    {
        spriteBatch.Draw(texture, ator, Color.Green);
    }
}