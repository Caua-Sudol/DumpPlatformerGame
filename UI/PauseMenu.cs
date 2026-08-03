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

    private SpriteFont _font;
    private Vector2 _resumePosition;
    private Vector2 _restartPosition;
    private Vector2 _quitPosition;

    private PauseOption _currentOption = PauseOption.RESUME;

    public PauseMenu(SpriteFont font, Vector2 position)
    {
        _font = font;
        _resumePosition = position;
        _restartPosition = position + new Vector2(0, OptionSpacing);
        _quitPosition = position + new Vector2(0, OptionSpacing * 2);
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
        spriteBatch.DrawString(_font, "Resume", _resumePosition, GetOptionColor(PauseOption.RESUME));
        spriteBatch.DrawString(_font, "Restart", _restartPosition, GetOptionColor(PauseOption.RESTART));
        spriteBatch.DrawString(_font, "Quit", _quitPosition, GetOptionColor(PauseOption.QUIT));
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
}
