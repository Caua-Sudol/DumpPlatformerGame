using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum DeathOption
{
    NONE = 0,
    RETRY = 1,
    QUIT = 2
}

public class DeathMenu
{
    private const float OptionSpacing = 20.0f;

    private SpriteFont _font;
    private Vector2 _retryPosition;
    private Vector2 _quitPosition;

    private DeathOption _currentOption = DeathOption.RETRY;

    public DeathMenu(SpriteFont font, Vector2 position)
    {
        _font = font;
        _retryPosition = position;
        _quitPosition = position + new Vector2(0, OptionSpacing);
    }

    public void Open()
    {
        _currentOption = DeathOption.RETRY;
    }

    public DeathOption Update(KeyboardState keyboardState, KeyboardState previousKeyboardState)
    {
        if (KeyWasPressed(keyboardState, previousKeyboardState, Keys.W) ||
            KeyWasPressed(keyboardState, previousKeyboardState, Keys.S))
        {
            ToggleOption();
        }

        if (KeyWasPressed(keyboardState, previousKeyboardState, Keys.Enter))
        {
            return _currentOption;
        }

        return DeathOption.NONE;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(_font, "Retry", _retryPosition, GetOptionColor(DeathOption.RETRY));
        spriteBatch.DrawString(_font, "Quit", _quitPosition, GetOptionColor(DeathOption.QUIT));
        spriteBatch.End();
    }

    private bool KeyWasPressed(KeyboardState keyboardState, KeyboardState previousKeyboardState, Keys key)
    {
        return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
    }

    private void ToggleOption()
    {
        if (_currentOption == DeathOption.RETRY)
        {
            _currentOption = DeathOption.QUIT;
        }
        else
        {
            _currentOption = DeathOption.RETRY;
        }
    }

    private Color GetOptionColor(DeathOption option)
    {
        if (_currentOption == option)
        {
            return Color.Yellow;
        }

        return Color.White;
    }
}
