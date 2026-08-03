using System.Linq;
using System.Reflection.Metadata;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum Option
{
    START = 1, 
    EXIT = 2,
    NONE = 0
}
public class StartMenu
{
    private const string startS = "Start";
    private const string exitS = "Exit";
    private SpriteFont _font;
    private Vector2 _fontPositionStart;
    private Vector2 _fontPositionExit;
    public Option _currentOption {get; private set;} = Option.NONE;
    public bool _enterIsPressed {get; private set;} = false;

    public StartMenu(SpriteFont font, Vector2 fontPositionStart, Vector2 fontPositionExit)
    {
        _font = font;
        _fontPositionStart = fontPositionStart;
        _fontPositionExit = fontPositionExit;
    }

    public bool Pressed
    {
        set
        {
            _enterIsPressed = value;
        }
        get
        {
            return _enterIsPressed;
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
    }

    public void Update()
    {
        // movimento de teclado muda a opção selecionada trocando o "currentOption
        var state = Keyboard.GetState();

        if (state.IsKeyDown(Keys.W))
        {
            _currentOption = Option.START;
        }
        if (state.IsKeyDown(Keys.S))
        {
            _currentOption = Option.EXIT;
        }
        if (state.IsKeyDown(Keys.Enter))
        {
            _enterIsPressed = true;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        if(_currentOption == Option.START)
        {
            spriteBatch.DrawString(_font, startS, _fontPositionStart, Color.Yellow);
            spriteBatch.DrawString(_font, exitS, _fontPositionExit, Color.White);
        }
        else
        {
            spriteBatch.DrawString(_font, startS, _fontPositionStart, Color.White);
            spriteBatch.DrawString(_font, exitS, _fontPositionExit, Color.Yellow);
        }
        spriteBatch.End();
    }
}