using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum PauseOption
{
    NONE = 0,
    RESUME = 1,
    RESTART = 2,
    QUIT = 3
}

public class PauseMenu
{
    private const float OptionSpacing = 20.0f;

    private readonly SpriteFont _font;
    private readonly Vector2 _resumePosition;
    private readonly Vector2 _restartPosition;
    private readonly Vector2 _quitPosition;

    private PauseOption _currentOption = PauseOption.RESUME;

    public PauseMenu(SpriteFont font, Vector2 menuCenter)
    {
        _font = font;
        _resumePosition = menuCenter - new Vector2(0, OptionSpacing);
        _restartPosition = menuCenter;
        _quitPosition = menuCenter + new Vector2(0, OptionSpacing);
    }

    public void Open()
    {
        _currentOption = PauseOption.RESUME;
    }

    public PauseOption Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
    {
        if (KeyWasPressed(keyboardState, previousKeyboardState, Keys.W))
        {
            MoveOptionUp();
        }
        else if (KeyWasPressed(keyboardState, previousKeyboardState, Keys.S))
        {
            MoveOptionDown();
        }

        if (KeyWasPressed(keyboardState, previousKeyboardState, Keys.Enter))
        {
            return _currentOption;
        }

        return PauseOption.NONE;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        DrawOption(spriteBatch, "Resume", _resumePosition, GetOptionColor(PauseOption.RESUME));
        DrawOption(spriteBatch, "Restart", _restartPosition, GetOptionColor(PauseOption.RESTART));
        DrawOption(spriteBatch, "Quit", _quitPosition, GetOptionColor(PauseOption.QUIT));
        spriteBatch.End();
    }

    private bool KeyWasPressed(KeyboardState keyboardState, KeyboardState previousKeyboardState, Keys key)
    {
        return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }

    private void MoveOptionUp()
    {
        if (_currentOption == PauseOption.RESUME)
        {
            _currentOption = PauseOption.QUIT;
        }
        else if (_currentOption == PauseOption.RESTART)
        {
            _currentOption = PauseOption.RESUME;
        }
        else if (_currentOption == PauseOption.QUIT)
        {
            _currentOption = PauseOption.RESTART;
        }
    }

    private void MoveOptionDown()
    {
        if (_currentOption == PauseOption.RESUME)
        {
            _currentOption = PauseOption.RESTART;
        }
        else if (_currentOption == PauseOption.RESTART)
        {
            _currentOption = PauseOption.QUIT;
        }
        else if (_currentOption == PauseOption.QUIT)
        {
            _currentOption = PauseOption.RESUME;
        }
    }

    private Color GetOptionColor(PauseOption option)
    {
        if (_currentOption == option)
        {
            return Color.Yellow;
        }

        return Color.White;
    }

    private void DrawOption(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = position - textSize / 2;

        spriteBatch.DrawString(_font, text, textPosition, color);
    }
}
