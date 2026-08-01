using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum Direction
{
    Top = 1,
    Down = 2,
    Rigth = 3,
    Left = 4
}

public enum PlayerState
{
    Idle,
    Moving,
    Jumping,
    Falling,
    Dashing
}

public class Player
{
    private Vector2 _player;
    private int _widthPlayer;
    private int _heightPlayer;

    public Vector2 _velocity { get; private set; }
    public Vector2 _maxVelocity { get; private set; }
    public double _gravity { get; private set; }
    public bool _isGrounded { get; private set; } = false;
    public Direction _direction { get; private set; } = Direction.Rigth;

    private float _groundAcceleration = 3300f;
    private float _groundDeceleration = 3300f;
    private float _airAcceleration = 1800f;

    public bool isBreath { get; private set; } = true;
    private Vector2 _dashDirection;
    private float _dashSpeed = 960f;
    private float _dashDuration = 0.18f;
    private double _dashTimer;

    public PlayerState State { get; private set; } = PlayerState.Idle;

    private KeyboardState _previousKeyboardState;
    private int _lastHorizontalKeyPressed = 0;

    private Texture2D _texture;
    private Color[] _color;

    public Player(int x, int y, double gravity, int width, int height)
    {
        _player = new Vector2(x, y);
        _velocity = new Vector2();
        _maxVelocity = new Vector2(528f, 600f);
        _gravity = gravity;
        _widthPlayer = width;
        _heightPlayer = height;
    }

    public void LoadContent(GraphicsDevice graphicsDevice, int width, int height)
    {
        _texture = new Texture2D(graphicsDevice, width, height);
        _color = Enumerable.Repeat(Color.White, width * height).ToArray();
        _texture.SetData(_color);
    }

    public Vector2 Position
    {
        set { _player = value; }
        get { return _player; }
    }

    public Rectangle HitBox
    {
        get { return new Rectangle((int)_player.X, (int)_player.Y, _widthPlayer, _heightPlayer); }
    }

    public void Update(double deltaTime)
    {
        var keyboardState = Keyboard.GetState();
        int horizontalInput = ReadHorizontalInput(keyboardState);

        if (horizontalInput != 0)
            _direction = horizontalInput > 0 ? Direction.Rigth : Direction.Left;

        UpdateState(keyboardState, horizontalInput);

        if (State == PlayerState.Dashing)
            UpdateDash(deltaTime);
        else
            ApplyNormalMovement(keyboardState, horizontalInput, deltaTime);
    }

    private int ReadHorizontalInput(KeyboardState state)
    {
        bool dDown = state.IsKeyDown(Keys.D);
        bool aDown = state.IsKeyDown(Keys.A);
        bool dWasDown = _previousKeyboardState.IsKeyDown(Keys.D);
        bool aWasDown = _previousKeyboardState.IsKeyDown(Keys.A);

        if (dDown && !dWasDown)
            _lastHorizontalKeyPressed = 1;
        if (aDown && !aWasDown)
            _lastHorizontalKeyPressed = -1;

        _previousKeyboardState = state;

        if (dDown && aDown)
            return _lastHorizontalKeyPressed;
        if (dDown)
            return 1;
        if (aDown)
            return -1;

        return 0;
    }

    private void UpdateState(KeyboardState state, int horizontalInput)
    {
        if (State == PlayerState.Dashing)
            return;

        if (isBreath && state.IsKeyDown(Keys.LeftShift))
        {
            StartDash(state, horizontalInput);
            return;
        }

        if (_isGrounded)
        {
            if (state.IsKeyDown(Keys.Space))
            {
                _velocity = new Vector2(_velocity.X, -_maxVelocity.Y);
                _isGrounded = false;
                State = PlayerState.Jumping;
                return;
            }

            State = horizontalInput != 0 ? PlayerState.Moving : PlayerState.Idle;
        }
        else
        {
            State = _velocity.Y < 0 ? PlayerState.Jumping : PlayerState.Falling;
        }
    }

    private void StartDash(KeyboardState state, int horizontalInput)
    {
        int verticalInput = 0;
        if (state.IsKeyDown(Keys.W))
            verticalInput = -1;
        else if (state.IsKeyDown(Keys.S))
            verticalInput = 1;

        Vector2 direction = new Vector2(horizontalInput, verticalInput);

        if (direction == Vector2.Zero)
            direction = new Vector2(_direction == Direction.Rigth ? 1 : -1, 0);

        direction.Normalize();

        _dashDirection = direction;
        _dashTimer = _dashDuration;
        State = PlayerState.Dashing;
        isBreath = false;
    }

    private void UpdateDash(double deltaTime)
    {
        _velocity = _dashDirection * _dashSpeed;
        _dashTimer -= deltaTime;

        if (_dashTimer <= 0f)
            EndDash();
    }
    public void CancelDash()
    {
        if (State != PlayerState.Dashing)
            return;

        EndDash();
    }

    public void EndDash()
    {
        _dashTimer = 0f;
        State = _isGrounded ? PlayerState.Idle : PlayerState.Falling;

        _velocity = _velocity * 0.5f;
    }

    private void ApplyNormalMovement(KeyboardState state, int horizontalInput, double deltaTime)
    {
        float targetVelocityX = horizontalInput * _maxVelocity.X;

        float accelerationRate;
        if (horizontalInput == 0 && _isGrounded)
            accelerationRate = _groundDeceleration;
        else
            accelerationRate = _isGrounded ? _groundAcceleration : _airAcceleration;

        double velX = MoveToward(_velocity.X, targetVelocityX, accelerationRate * deltaTime);
        double velY = _velocity.Y;

        if (!_isGrounded)
            velY -= _gravity * deltaTime;

        if (state.IsKeyUp(Keys.Space) && velY < 0)
            velY *= 0.85f;

        _velocity = new Vector2((float)velX, (float)velY);
    }

    private static double MoveToward(double current, double target, double maxDelta)
    {
        if (Math.Abs(target - current) <= maxDelta)
            return target;

        return current + Math.Sign(target - current) * maxDelta;
    }

    public void Move(int x, int y)
    {
        _player.Y = y;
        _player.X = x;
    }

    public void Walk(float velocityX, float velocityY, double deltaTime)
    {
        _player.X += velocityX * (float)deltaTime;
        _player.Y += velocityY * (float)deltaTime;
    }

    public void WalkX(float velocityX, double deltaTime)
    {
        _player.X += velocityX * (float)deltaTime;
    }

    public void WalkY(float velocityY, double deltaTime)
    {
        _player.Y += velocityY * (float)deltaTime;
    }

    public void StopWalkX()
    {
        _velocity = new Vector2(0, _velocity.Y);
    }

    public void StopWalkY()
    {
        _velocity = new Vector2(_velocity.X, 0);
    }

    public void StopFalling()
    {
        _velocity = new Vector2(_velocity.X, 0);
    }

    public void Grounded()
    {
        _isGrounded = true;
    }

    public void NotGrounded()
    {
        _isGrounded = false;
    }

    public void MoreBreath()
    {
        isBreath = true;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 actor)
    {
        spriteBatch.Draw(_texture, actor, Color.Purple);
    }
}