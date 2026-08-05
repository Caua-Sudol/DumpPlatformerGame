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

    private readonly SpriteFont _font;
    private readonly Vector2 _retryPosition;
    private readonly Vector2 _quitPosition;

    private DeathOption _currentOption = DeathOption.RETRY;

    public DeathMenu(SpriteFont font, Vector2 menuCenter)
    {
        _font = font;
        _retryPosition = menuCenter - new Vector2(0, OptionSpacing / 2);
        _quitPosition = menuCenter + new Vector2(0, OptionSpacing / 2);
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
        DrawOption(spriteBatch, "Retry", _retryPosition, GetOptionColor(DeathOption.RETRY));
        DrawOption(spriteBatch, "Quit", _quitPosition, GetOptionColor(DeathOption.QUIT));
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

    private void DrawOption(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
    {
        Vector2 textSize = _font.MeasureString(text);
        Vector2 textPosition = position - textSize / 2;

        spriteBatch.DrawString(_font, text, textPosition, color);
    }
}
