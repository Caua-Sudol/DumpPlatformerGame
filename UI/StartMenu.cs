using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum StartMenuOption
{
    START = 1, 
    EXIT = 2,
    NONE = 0
}

public class StartMenu
{
    private const string StartText = "Start";
    private const string ExitText = "Exit";

    private readonly SpriteFont _font;
    private readonly Vector2 _fontPositionStart;
    private readonly Vector2 _fontPositionExit;
    private StartMenuOption _currentOption = StartMenuOption.NONE;

    public StartMenu(SpriteFont font, Vector2 fontPositionStart, Vector2 fontPositionExit)
    {
        _font = font;
        _fontPositionStart = fontPositionStart;
        _fontPositionExit = fontPositionExit;
    }

    public StartMenuOption Update(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.W))
        {
            _currentOption = StartMenuOption.START;
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            _currentOption = StartMenuOption.EXIT;
        }
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            return _currentOption;
        }

        return StartMenuOption.NONE;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        if(_currentOption == StartMenuOption.START)
        {
            spriteBatch.DrawString(_font, StartText, _fontPositionStart, Color.Yellow);
            spriteBatch.DrawString(_font, ExitText, _fontPositionExit, Color.White);
        }
        else
        {
            spriteBatch.DrawString(_font, StartText, _fontPositionStart, Color.White);
            spriteBatch.DrawString(_font, ExitText, _fontPositionExit, Color.Yellow);
        }
        spriteBatch.End();
    }
}
