using System.Dynamic;
using System.IO.Pipelines;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.Devices.Sensors;

namespace DontLikePoetry;

public enum Direction{
    Top = 1,
    Down = 2,
    Rigth = 3,
    Left = 4
}

public class Player
{
    private Vector2 _player;
    private int _widthPlayer;
    private int _heightPlayer;

    public Vector2 _velocity { get; private set;}
    public Vector2 _maxVelocity { get; private set;}
    public float _acc { get; private set;}
    public float _gravity { get; private set;}
    public float dt { get; private set;}
    public bool _isGrounded {get; private set;} = false;
    public Direction _direction {get; private set;} = Direction.Rigth;
    public bool isBreath {get; private set;} = true;
    private float _dashTimer = 0;
    private Texture2D _texture;
    private Color[] _color;

    public Player(int x, int y, float gravity, int width, int height)
    {
        _player = new Vector2(x, y);
        _velocity = new Vector2();
        _maxVelocity = new Vector2(10f, 10f);
        _acc = 2.5f;
        dt = 0.85f;
        _gravity = gravity;
        _widthPlayer = width;
        _heightPlayer = height;
    }
    public void LoadContent(GraphicsDevice graphicsDevice, int width, int height)
    {
        _texture = new Texture2D(graphicsDevice, width, height);
        _color = Enumerable.Repeat(Color.White, width*height).ToArray();
        _texture.SetData(_color);
    }

    public Vector2 Position
    {
        set 
        {
            _player = value;
        }
        get
        {
            return _player;
        }
    }

    public Rectangle HitBox
    {
        get
        {
            return new Rectangle((int)_player.X, (int)_player.Y, _widthPlayer, _heightPlayer);
        }
    }

    public void Update()
    {
        var velX = _velocity.X; 
        var velY = _velocity.Y;
        var state = Keyboard.GetState();

        if(_dashTimer > 0)
        {
            if(_direction == Direction.Rigth)
            {
                velX = _dashTimer;
                _dashTimer -= 1f;
            }
            if(_direction == Direction.Left)
            {
                velX = -_dashTimer;
                _dashTimer -= 1f;
            }
            isBreath = false;
        }
        else
        { 
            if (state.IsKeyDown(Keys.Space) && _isGrounded)
            {
                velY = -10f;
                _isGrounded = false;
            }
            if (state.IsKeyDown(Keys.LeftShift) && isBreath)
            {
                _dashTimer = 15f;
                isBreath = false;
            }
            if (state.IsKeyDown(Keys.D))
            {
                velX += _acc * _maxVelocity.X * dt;
                _direction = Direction.Rigth;
            }
            else if (state.IsKeyDown(Keys.A))
            {
                velX += _acc * -_maxVelocity.X * dt;
                _direction = Direction.Left;
            }
            else
            {
                velX = 0;
            }
            if(_isGrounded == false)
            {
                velY -= _gravity;   
            }
        }
        if(velX >= _maxVelocity.X)
        {
            velX = _maxVelocity.X;
        }
        else if (velX <= -_maxVelocity.X)
        {
            velX = -_maxVelocity.X;
        }
        _velocity = new Vector2(velX, velY);
    }

    public void Move(int x, int y)
    {
        _player.Y = y;
        _player.X = x;
    }

    public void Walk(float veloctyX, float velocityY)
    {
        _player.X += veloctyX;
        _player.Y += velocityY;
    }

    public void WalkX(float veloctyX)
    {
        _player.X += veloctyX;
    }
    public void WalkY(float velocityY)
    {
        _player.Y += velocityY;
    }
    public void StopWalkX()
    {
        _player.X += 0;
    }
    public void StopWalkY()
    {
        _player.Y += 0;
    }

    public void StopFalling(float velocityX)
    {
        _velocity = new Vector2(velocityX, 0);
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