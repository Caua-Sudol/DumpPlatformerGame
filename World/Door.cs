using System.IO.Pipelines;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DontLikePoetry;

public class Door
{
    // posição
    // tamanho
    // retângulo de colisão
    // método para desenhar

    private Rectangle _door;
    private Texture2D _texture;
    private Color[] _color;

    public Door(int x, int y, int width, int height)
    {
        _door = new Rectangle(x, y, width, height);
    }

    public Rectangle Bound
    {
        set
        {
            _door = value;
        }
        get
        {
            return _door;
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice, int wallWidth, int wallHeight)
    {
        _texture = new Texture2D(graphicsDevice, wallWidth, wallHeight);
        _color = Enumerable.Repeat(Color.White, wallWidth*wallHeight).ToArray();
        _texture.SetData(_color);
    }

    public void Draw(SpriteBatch spriteBatch, Rectangle ator)
    {
        spriteBatch.Draw(_texture, ator, Color.Blue);
    }
}