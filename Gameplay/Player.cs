using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DontLikePoetry;

public enum Direction
{
    Top = 1,
    Down = 2,
    Right = 3,
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

    private Vector2 _velocity;
    private readonly Vector2 _maxVelocity;
    private readonly double _gravity;
    private bool _isGrounded;
    private Direction _direction = Direction.Right;

    private float _groundAcceleration = 3300f;
    private float _groundDeceleration = 3300f;
    private float _airAcceleration = 1800f;
    private double _coyoteTime = 0.10;
    private double _coyoteTimer;
    private double _jumpBufferTime = 0.10;
    private double _jumpBufferTimer;

    public bool CanDash { get; private set; } = true;
    private Vector2 _dashDirection;
    private float _dashSpeed = 960f;
    private float _dashDuration = 0.18f;
    private double _dashTimer;

    public PlayerState State { get; private set; } = PlayerState.Idle;

    private readonly PlayerInput _input = new PlayerInput();

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

    public Vector2 Velocity
    {
        get { return _velocity; }
    }

    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

    public Rectangle HitBox
    {
        get { return new Rectangle((int)_player.X, (int)_player.Y, _widthPlayer, _heightPlayer); }
    }

    public void Update(double deltaTime)
    {
        _input.Update();
        int horizontalInput = _input.Horizontal;

        UpdateCoyoteTimer(deltaTime);
        UpdateJumpBufferTimer(deltaTime);

        if (horizontalInput != 0)
            _direction = horizontalInput > 0 ? Direction.Right : Direction.Left;

        UpdateState(horizontalInput);

        if (State == PlayerState.Dashing)
            UpdateDash(deltaTime);
        else
            ApplyNormalMovement(horizontalInput, deltaTime);
    }

    private void UpdateState(int horizontalInput)
    {
        if (State == PlayerState.Dashing)
            return;

        if (CanDash && _input.DashWasPressed)
        {
            StartDash(horizontalInput);
            return;
        }

        if (HasBufferedJump() && CanJump())
        {
            StartJump();
            return;
        }

        if (_isGrounded)
        {
            State = horizontalInput != 0 ? PlayerState.Moving : PlayerState.Idle;
        }
        else
        {
            State = _velocity.Y < 0 ? PlayerState.Jumping : PlayerState.Falling;
        }
    }

    private void UpdateCoyoteTimer(double deltaTime)
    {
        if (_isGrounded)
        {
            _coyoteTimer = _coyoteTime;
        }
        else if (_coyoteTimer > 0)
        {
            _coyoteTimer -= deltaTime;
        }
    }

    private bool CanJump()
    {
        return _isGrounded || _coyoteTimer > 0;
    }

    private void UpdateJumpBufferTimer(double deltaTime)
    {
        if (_input.JumpWasPressed)
        {
            _jumpBufferTimer = _jumpBufferTime;
        }
        else if (_jumpBufferTimer > 0)
        {
            _jumpBufferTimer -= deltaTime;
        }
    }

    private bool HasBufferedJump()
    {
        return _jumpBufferTimer > 0;
    }

    private void StartJump()
    {
        _velocity = new Vector2(_velocity.X, -_maxVelocity.Y);
        _isGrounded = false;
        _coyoteTimer = 0;
        _jumpBufferTimer = 0;
        State = PlayerState.Jumping;
    }

    private void StartDash(int horizontalInput)
    {
        int dashHorizontalInput = horizontalInput;
        int dashVerticalInput = _input.Vertical;

        Vector2 direction = new Vector2(dashHorizontalInput, dashVerticalInput);

        if (direction == Vector2.Zero)
            direction = new Vector2(_direction == Direction.Right ? 1 : -1, 0);

        direction.Normalize();

        _dashDirection = direction;
        _dashTimer = _dashDuration;
        State = PlayerState.Dashing;
        CanDash = false;
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

    private void ApplyNormalMovement(int horizontalInput, double deltaTime)
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

        if (!_input.IsJumpHeld && velY < 0)
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
        CancelDash();
    }

    public void StopWalkY()
    {
        _velocity = new Vector2(_velocity.X, 0);
        CancelDash();
    }

    public void StopFalling()
    {
        _velocity = new Vector2(_velocity.X, 0);
        CancelDash();
    }

    public void ResetVelocity()
    {
        _velocity = Vector2.Zero;
    }

    public void Grounded()
    {
        _isGrounded = true;
        _coyoteTimer = _coyoteTime;
    }

    public void NotGrounded()
    {
        _isGrounded = false;
    }

    public void RestoreDash()
    {
        CanDash = true;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 actor)
    {
        spriteBatch.Draw(_texture, actor, Color.Purple);
    }
}
