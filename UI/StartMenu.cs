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
    private const float OptionSpacing = 20.0f;
    private const string StartText = "Start";
    private const string ExitText = "Exit";

    private readonly SpriteFont _font;
    private readonly Vector2 _fontPositionStart;
    private readonly Vector2 _fontPositionExit;
    private StartMenuOption _currentOption = StartMenuOption.NONE;

    public StartMenu(SpriteFont font, Vector2 menuCenter)
    {
        _font = font;
        _fontPositionStart = menuCenter - new Vector2(0, OptionSpacing / 2);
        _fontPositionExit = menuCenter + new Vector2(0, OptionSpacing / 2);
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
            DrawOption(spriteBatch, StartText, _fontPositionStart, Color.Yellow);
            DrawOption(spriteBatch, ExitText, _fontPositionExit, Color.White);
        }
        else
        {
            DrawOption(spriteBatch, StartText, _fontPositionStart, Color.White);
            DrawOption(spriteBatch, ExitText, _fontPositionExit, Color.Yellow);
        }
        spriteBatch.End();
    }

    private void DrawOption(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = position - textSize / 2;

        spriteBatch.DrawString(_font, text, textPosition, color);
    }
}
