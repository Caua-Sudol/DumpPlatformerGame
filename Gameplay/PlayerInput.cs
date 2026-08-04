using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public class PlayerInput
{
    private KeyboardState _previousKeyboardState;
    private int _lastHorizontalKeyPressed;

    public int Horizontal { get; private set; }
    public int Vertical { get; private set; }
    public bool IsJumpHeld { get; private set; }
    public bool JumpWasPressed { get; private set; }
    public bool DashWasPressed { get; private set; }

    public void Update()
    {
        KeyboardState keyboardState = Keyboard.GetState();

        Horizontal = ReadHorizontalInput(keyboardState);
        Vertical = ReadVerticalInput(keyboardState);
        IsJumpHeld = keyboardState.IsKeyDown(Keys.Space);
        JumpWasPressed = WasKeyPressed(keyboardState, Keys.Space);
        DashWasPressed = WasKeyPressed(keyboardState, Keys.LeftShift);

        _previousKeyboardState = keyboardState;
    }

    private int ReadHorizontalInput(KeyboardState keyboardState)
    {
        bool dDown = keyboardState.IsKeyDown(Keys.D);
        bool aDown = keyboardState.IsKeyDown(Keys.A);
        bool dWasDown = _previousKeyboardState.IsKeyDown(Keys.D);
        bool aWasDown = _previousKeyboardState.IsKeyDown(Keys.A);

        if (dDown && !dWasDown)
        {
            _lastHorizontalKeyPressed = 1;
        }

        if (aDown && !aWasDown)
        {
            _lastHorizontalKeyPressed = -1;
        }

        if (dDown && aDown)
        {
            return _lastHorizontalKeyPressed;
        }

        if (dDown)
        {
            return 1;
        }

        if (aDown)
        {
            return -1;
        }

        return 0;
    }

    private int ReadVerticalInput(KeyboardState keyboardState)
    {
        bool wDown = keyboardState.IsKeyDown(Keys.W);
        bool sDown = keyboardState.IsKeyDown(Keys.S);

        if (wDown && !sDown)
        {
            return -1;
        }

        if (sDown && !wDown)
        {
            return 1;
        }

        return 0;
    }

    private bool WasKeyPressed(KeyboardState keyboardState, Keys key)
    {
        return keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }
}
