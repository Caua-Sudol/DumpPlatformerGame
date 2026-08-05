using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DontLikePoetry;

public class Scene
{
    private const string DefaultMapPath = "Content/SimpleCutsceneMap.tmx";
    private const int DefaultPlayerStartX = 10;
    private const int DefaultPlayerStartY = 552;
    private const int PlayerWidth = 16;
    private const int PlayerHeight = 16;

    private const double PlayerGravity = -1800.0;
    private const double SecondsPerFramePlaying = 1.0 / 60.0;
    private const double SecondsPerFrameCutscene = 1.0 / 5.0;

    private const int ScreenWidth = 1920;
    private const int ScreenHeight = 1080;
    private const int FallLimitY = ScreenHeight + 128;

    private const float FadeStep = 0.2f;
    private const float CutsceneCameraZoom = 2.0f;
    private const float CutsceneWalkSpeed = 10.0f;
    private const float CutsceneEndX = 940.0f;

    private Texture2D _fadeTexture;
    private Rectangle _fadeRectangle;
    private float _fadeAlpha;

    private Player _player;
    private readonly LevelMap _map;
    private readonly PlayerCollision _playerCollision = new PlayerCollision();
    private Vector2 _checkpoint;
    private bool _playerIsDead;
    private readonly Vector2 _playerStart;

    public SceneMode SceneMode { get; private set; }
    public double SecondsPerFrame { get; private set; }
    public bool PlayerIsDead
    {
        get
        {
            return _playerIsDead;
        }
    }

    public Scene()
        : this(DefaultMapPath, new Vector2(DefaultPlayerStartX, DefaultPlayerStartY))
    {
    }

    public Scene(string mapPath, Vector2 playerStart)
    {
        _map = new LevelMap(mapPath);
        _playerStart = playerStart;
    }

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
        _playerIsDead = false;
        _fadeRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);
        _checkpoint = _playerStart;

        var fadeColor = Enumerable.Repeat(Color.White, ScreenWidth * ScreenHeight).ToArray();
        _fadeTexture = new Texture2D(graphicsDevice, ScreenWidth, ScreenHeight);
        _fadeTexture.SetData(fadeColor);

        _player = new Player((int)_playerStart.X, (int)_playerStart.Y, PlayerGravity, PlayerWidth, PlayerHeight);
        _player.LoadContent(graphicsDevice, PlayerWidth, PlayerHeight);

        _map.LoadContent(graphicsDevice);

        SceneMode = SceneMode.PLAYING;
        SecondsPerFrame = SecondsPerFramePlaying;
    }

    public void Update(Camera camera, double deltaTime)
    {
        if (SceneMode == SceneMode.PLAYING)
        {
            UpdatePlaying(camera, deltaTime);
        }
        else if (SceneMode == SceneMode.FADE_OUT)
        {
            UpdateFadeOut(camera);
        }
        else if (SceneMode == SceneMode.FADE_IN)
        {
            UpdateFadeIn();
        }
        else if (SceneMode == SceneMode.CUTSCENE)
        {
            UpdateCutscene(camera, deltaTime);
        }
    }

    public void Restart(Camera camera)
    {
        RespawnPlayer(camera);
    }

    private void UpdatePlaying(Camera camera, double deltaTime)
    {
        _player.Update(deltaTime);
        UpdatePlayerPhysics(deltaTime);

        if (PlayerFell())
        {
            KillPlayer();
            return;
        }

        UpdatePlayingCamera(camera);

        if (PlayerTouchedCutsceneTrigger())
        {
            StartFadeOut();
        }
    }

    private void UpdatePlayerPhysics(double deltaTime)
    {
        _playerCollision.MoveAndResolve(_player, _map.Platforms, deltaTime);
    }

    private bool PlayerTouchedCutsceneTrigger()
    {
        return _map.HasCutsceneTrigger(_player.HitBox);
    }

    private bool PlayerFell()
    {
        return _player.Position.Y > FallLimitY;
    }

    private void KillPlayer()
    {
        _playerIsDead = true;
        _player.CancelDash();
        _player.ResetVelocity();
    }

    private void RespawnPlayer(Camera camera)
    {
        _playerIsDead = false;
        _player.Move((int)_checkpoint.X, (int)_checkpoint.Y);
        _player.CancelDash();
        _player.ResetVelocity();
        _player.Grounded();
        _player.RestoreDash();

        _fadeAlpha = 0.0f;
        SecondsPerFrame = SecondsPerFramePlaying;
        SceneMode = SceneMode.PLAYING;

        UpdatePlayingCamera(camera);
    }

    private void StartFadeOut()
    {
        SecondsPerFrame = SecondsPerFrameCutscene;
        SceneMode = SceneMode.FADE_OUT;
    }

    private void UpdateFadeOut(Camera camera)
    {
        if (_fadeAlpha < 1.0f)
        {
            _fadeAlpha += FadeStep;
        }

        if (_fadeAlpha >= 1.0f)
        {
            _fadeAlpha = 1.0f;
            _player.Move((int)_checkpoint.X, (int)_checkpoint.Y);
            UpdateCutsceneCamera(camera);
            SceneMode = SceneMode.FADE_IN;
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
            SceneMode = SceneMode.CUTSCENE;
        }
    }

    private void UpdateCutscene(Camera camera, double deltaTime)
    {
        UpdateCutsceneCamera(camera);

        if (_player.Position.X <= CutsceneEndX)
        {
            _player.Walk(CutsceneWalkSpeed, 0, deltaTime);
        }
        else
        {
            FinishCutscene(camera);
        }
    }

    private void UpdatePlayingCamera(Camera camera)
    {
        camera.Zoom = 1.0f;
        camera.Follow(_player);
    }

    private void UpdateCutsceneCamera(Camera camera)
    {
        camera.Zoom = CutsceneCameraZoom;
        camera.Follow(_player);
    }

    private void FinishCutscene(Camera camera)
    {
        SceneMode = SceneMode.PLAYING;
        SecondsPerFrame = SecondsPerFramePlaying;
        camera.Zoom = 1.0f;
        _fadeAlpha = 0.0f;
        _player.Move((int)_playerStart.X, (int)_playerStart.Y);
    }

    public void Draw(SpriteBatch spriteBatch, Camera camera)
    {
        DrawWorld(spriteBatch, camera);
        DrawScreenEffects(spriteBatch);
    }

    private void DrawWorld(SpriteBatch spriteBatch, Camera camera)
    {
        var cameraTransform = camera.GetTransform();
        spriteBatch.Begin(transformMatrix: cameraTransform);

        _player.Draw(spriteBatch, _player.Position);

        _map.Draw(spriteBatch);

        spriteBatch.End();
    }

    private void DrawScreenEffects(SpriteBatch spriteBatch)
    {
        if (!ShouldDrawFade())
        {
            return;
        }

        spriteBatch.Begin();
        spriteBatch.Draw(_fadeTexture, _fadeRectangle, new Color(Color.Black, _fadeAlpha));
        spriteBatch.End();
    }

    private bool ShouldDrawFade()
    {
        return SceneMode == SceneMode.FADE_OUT
            || SceneMode == SceneMode.FADE_IN
            || SceneMode == SceneMode.CUTSCENE;
    }
}
