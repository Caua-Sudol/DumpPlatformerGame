using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace DontLikePoetry;

public enum GameMode
{
   PLAYING = 0,
   CUTSCENE = 1,
   FADE_OUT = 2,
   FADE_IN = 3
}

public class Scene
{
    private const int PlayerStartX = 10;
    private const int PlayerStartY = 552;
    private const int PlayerWidth = 16;
    private const int PlayerHeight = 16;

    private const double PlayerGravity = -1800.0;
    private const double SecondsPerFramePlaying = 1.0 / 60.0;
    private const double SecondsPerFrameCutscene = 1.0 / 5.0;

    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private const int TileSize = 32;

    private const float FadeStep = 0.2f;
    private const float CutsceneCameraZoom = 2.0f;
    private const float CutsceneWalkSpeed = 10.0f;
    private const float CutsceneEndX = 940.0f;

    private readonly List<Rectangle> _platforms = new List<Rectangle>();
    private readonly List<Rectangle> _triggerCollisions = new List<Rectangle>();

    private Texture2D _fadeTexture;
    private Rectangle _fadeRectangle;
    private Texture2D _platformTexture;
    private float _fadeAlpha;

    private Player _player;

    public GameMode GameMode { get; private set; }
    public double FPS { get; private set; }

    public Player Player
    {
        get
        {
            return _player;
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _fadeAlpha = 0.0f;
        _fadeRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

        var fadeColor = Enumerable.Repeat(Color.White, ScreenWidth * ScreenHeight).ToArray();
        _fadeTexture = new Texture2D(graphicsDevice, ScreenWidth, ScreenHeight);
        _fadeTexture.SetData(fadeColor);

        _player = new Player(PlayerStartX, PlayerStartY, PlayerGravity, PlayerWidth, PlayerHeight);
        _player.LoadContent(graphicsDevice, PlayerWidth, PlayerHeight);

        LoadMap();
        LoadPlatformTexture(graphicsDevice);

        GameMode = GameMode.PLAYING;
        FPS = SecondsPerFramePlaying;
    }

    private void LoadMap()
    {
        var map = new TmxMap("Content/SimpleCutsceneMap.tmx");

        foreach (var row in map.ObjectGroups["mapaGeral"].Objects)
        {
            _platforms.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }

        foreach (var row in map.ObjectGroups["triggerColision"].Objects)
        {
            _triggerCollisions.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }
    }

    private void LoadPlatformTexture(GraphicsDevice graphicsDevice)
    {
        var platformColor = Enumerable.Repeat(Color.Purple, TileSize * TileSize).ToArray();
        _platformTexture = new Texture2D(graphicsDevice, TileSize, TileSize);
        _platformTexture.SetData(platformColor);
    }

    public void Update(Camera camera, double deltaTime)
    {
        if (GameMode == GameMode.PLAYING)
        {
            UpdatePlaying(deltaTime);
        }
        else if (GameMode == GameMode.FADE_OUT)
        {
            UpdateFadeOut();
        }
        else if (GameMode == GameMode.FADE_IN)
        {
            UpdateFadeIn();
        }
        else if (GameMode == GameMode.CUTSCENE)
        {
            UpdateCutscene(camera, deltaTime);
        }
    }

    private void UpdatePlaying(double deltaTime)
    {
        _player.Update(deltaTime);
        UpdatePlayerPhysics(deltaTime);

        if (PlayerTouchedCutsceneTrigger())
        {
            StartFadeOut();
        }
    }

    private void UpdatePlayerPhysics(double deltaTime)
    {
        MovePlayerX(deltaTime);
        MovePlayerY(deltaTime);
        UpdateGroundedState();
    }

    private void MovePlayerX(double deltaTime)
    {
        _player.WalkX(_player.Velocity.X, deltaTime);
        ResolvePlayerXCollisions();
    }

    private void ResolvePlayerXCollisions()
    {
        foreach (var platform in _platforms)
        {
            if (PlayerIntersects(platform))
            {
                ResolveHorizontalCollision(platform);
                break;
            }
        }
    }

    private void ResolveHorizontalCollision(Rectangle platform)
    {
        var playerPosition = _player.Position;

        if (PlayerIsMovingRight())
        {
            playerPosition.X = platform.Left - _player.HitBox.Width;
        }
        else if (PlayerIsMovingLeft())
        {
            playerPosition.X = platform.Right;
        }

        _player.Position = playerPosition;
        _player.StopWalkX();
    }

    private void MovePlayerY(double deltaTime)
    {
        _player.NotGrounded();
        _player.WalkY(_player.Velocity.Y, deltaTime);
        ResolvePlayerYCollisions();
    }

    private void ResolvePlayerYCollisions()
    {
        foreach (var platform in _platforms)
        {
            if (PlayerIntersects(platform))
            {
                ResolveVerticalCollision(platform);
                break;
            }
        }
    }

    private void ResolveVerticalCollision(Rectangle platform)
    {
        var playerPosition = _player.Position;

        if (PlayerIsFalling())
        {
            playerPosition.Y = platform.Top - _player.HitBox.Height;
        }
        else if (PlayerIsJumping())
        {
            playerPosition.Y = platform.Bottom;
        }

        _player.Position = playerPosition;
        _player.StopWalkY();
    }

    private void UpdateGroundedState()
    {
        var groundCheck = GetGroundCheck();

        foreach (var platform in _platforms)
        {
            if (groundCheck.Intersects(platform))
            {
                _player.Grounded();
                _player.CancelDash();
                _player.RestoreDash();
                break;
            }
        }
    }

    private Rectangle GetGroundCheck()
    {
        return new Rectangle(_player.HitBox.X, _player.HitBox.Bottom, _player.HitBox.Width, 1);
    }

    private bool PlayerIntersects(Rectangle obj)
    {
        return _player.HitBox.Intersects(obj);
    }

    private bool PlayerIsMovingRight()
    {
        return _player.Velocity.X > 0;
    }

    private bool PlayerIsMovingLeft()
    {
        return _player.Velocity.X < 0;
    }

    private bool PlayerIsFalling()
    {
        return _player.Velocity.Y > 0;
    }

    private bool PlayerIsJumping()
    {
        return _player.Velocity.Y < 0;
    }

    private bool PlayerTouchedCutsceneTrigger()
    {
        foreach (var trigger in _triggerCollisions)
        {
            if (PlayerIntersects(trigger))
            {
                return true;
            }
        }

        return false;
    }

    private void StartFadeOut()
    {
        FPS = SecondsPerFrameCutscene;
        GameMode = GameMode.FADE_OUT;
    }

    private void UpdateFadeOut()
    {
        if (_fadeAlpha < 1.0f)
        {
            _fadeAlpha += FadeStep;
        }

        if (_fadeAlpha >= 1.0f)
        {
            _fadeAlpha = 1.0f;
            _player.Move(PlayerStartX, PlayerStartY);
            GameMode = GameMode.FADE_IN;
        }
    }

    private void UpdateFadeIn()
    {
        if (_fadeAlpha > 0.0f)
        {
            _fadeAlpha -= FadeStep;
        }

        if (_fadeAlpha <= 0.0f)
        {
            _fadeAlpha = 0.0f;
            GameMode = GameMode.CUTSCENE;
        }
    }

    private void UpdateCutscene(Camera camera, double deltaTime)
    {
        camera.Zoom = CutsceneCameraZoom;
        camera.Update(_player);

        if (_player.Position.X <= CutsceneEndX)
        {
            _player.Walk(CutsceneWalkSpeed, 0, deltaTime);
        }
        else
        {
            FinishCutscene(camera);
        }
    }

    private void FinishCutscene(Camera camera)
    {
        GameMode = GameMode.PLAYING;
        FPS = SecondsPerFramePlaying;
        camera.Zoom = 1.0f;
        _fadeAlpha = 0.0f;
        _player.Move(PlayerStartX, PlayerStartY);
    }

    public void Draw(SpriteBatch spriteBatch, Camera camera)
    {
        var cameraTransform = camera.GetTransform();
        spriteBatch.Begin(transformMatrix: cameraTransform);

        _player.Draw(spriteBatch, _player.Position);

        foreach (var platform in _platforms)
        {
            spriteBatch.Draw(_platformTexture, platform, Color.Purple);
        }

        if (GameMode == GameMode.FADE_OUT || GameMode == GameMode.FADE_IN || GameMode == GameMode.CUTSCENE)
        {
            spriteBatch.Draw(_fadeTexture, _fadeRectangle, new Color(Color.Black, _fadeAlpha));
        }

        spriteBatch.End();
    }
}
