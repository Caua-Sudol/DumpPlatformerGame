using Microsoft.Xna.Framework;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;
using System.Collections.Generic;

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
    // private Vector2 PlayerVelocity = new Vector2(10.0f, 10.0f); // Não vai ser const, velocidade vai ficar variando
    private const float PlayerGravity = -0.5f; //-9.81f

    private List<Rectangle> _platform = new List<Rectangle>();
    private List<Rectangle> _triggerColision = new List<Rectangle>();
    private Color [] _platfColor;
    private Texture2D fadeTexture;

    private Color [] _fadeColor;
    private Rectangle _fadeRec;
    private Texture2D _platformeTexture;
    private float _fadeAlph;

    private Player _player;

    public GameMode GameMode { get; private set; }
    public double SecondsPerFrame { get; private set; }

    public Player Player
    {
        get
        {
            return _player;
        }
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        _fadeAlph = 0.0f;

        _fadeColor = Enumerable.Repeat(Color.White, 1920*1080).ToArray();
        _fadeRec = new Rectangle(0, 0, 1920, 1080);
        fadeTexture = new Texture2D(graphicsDevice, 1920, 1080);
        fadeTexture.SetData(_fadeColor);        

        _player = new Player(PlayerStartX, PlayerStartY, PlayerGravity, PlayerWidth, PlayerHeight);
        _player.LoadContent(graphicsDevice, PlayerWidth, PlayerHeight);

        var map = new TmxMap("Content/SimpleCutsceneMap.tmx");

        foreach(var row in map.ObjectGroups["mapaGeral"].Objects)
        {
            _platform.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }
        _platfColor = Enumerable.Repeat(Color.Purple, 32*32).ToArray();
        _platformeTexture = new Texture2D(graphicsDevice, 32, 32);
        _platformeTexture.SetData(_platfColor);

        foreach(var row in map.ObjectGroups["triggerColision"].Objects)
        {
            _triggerColision.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }

        GameMode = GameMode.PLAYING;
        SecondsPerFrame = 1.0 / 60.0;
    }

    public void Update(Camera camera)
    {
        if (GameMode == GameMode.PLAYING)
        {
            _player.Update();

            if (PlayerTouchedObj(_triggerColision[0]))
            {
                SecondsPerFrame = 1.0 / 5.0;
                GameMode = GameMode.FADE_OUT;
            }
        }
        else if (GameMode == GameMode.CUTSCENE)
        {
            StartCutscene(camera);
        }
        else if (GameMode == GameMode.FADE_OUT)
        {
            if(_fadeAlph < 1.0f)
            {
                _fadeAlph += 0.2f;
            }
            if(_fadeAlph >= 1.0f)
            {

                _player.Move(PlayerStartX, PlayerStartY);
                GameMode = GameMode.FADE_IN;
            }
            
        }
        else if (GameMode == GameMode.FADE_IN)
        {
            _fadeAlph -= 0.2f;

            if(_fadeAlph <= 0.0f)
            {
                GameMode = GameMode.CUTSCENE;
            }
        }
        _player.WalkX(_player._velocity.X);
        foreach(var plat in _platform)
        {
            if(PlayerTouchedObj(plat))
            {
                var tempPlayer = _player.Position;
                if(_player._velocity.X > 0)
                {
                    tempPlayer.X = plat.Left - _player.HitBox.Width;
                }
                else
                {
                    tempPlayer.X = plat.Right; 
                }
                _player.Position = tempPlayer;
                _player.StopWalkX();
                break;
            }
        }

        _player.NotGrounded();
        _player.WalkY(_player._velocity.Y);
        
        foreach(var plat in _platform)
        {
            if(PlayerTouchedObj(plat))
            {
                var tempPlayer = _player.Position;
                
                if(_player._velocity.Y > 0)
                {
                    tempPlayer.Y = plat.Top - _player.HitBox.Height;
                    _player.StopFalling(_player._maxVelocity.X);
                }
                else if(_player._velocity.Y < 0)
                {
                    tempPlayer.Y = plat.Bottom;
                }
                
                _player.Position = tempPlayer;
                _player.StopWalkY();
                break;
            }
        }

        Rectangle tempHit = new Rectangle(_player.HitBox.X, _player.HitBox.Bottom, _player.HitBox.Width, 1); //Ajustando flik com o chão
        foreach(var plat in _platform)
        {
            if(tempHit.Intersects(plat))
            {
                _player.Grounded();
                _player.MoreBreath();
                break;
            }
        }
    }

    private bool PlayerTouchedObj(Rectangle obj)
    {
        return _player.HitBox.Intersects(obj);
    }

    private void StartCutscene(Camera camera)
    {
        camera.Zoom = 2.0f;
        UpdateCutscene(camera);
    }

    private void UpdateCutscene(Camera camera)
    {
        camera.Update(_player);
        if(_player.Position.X <= 940)
        {
            _player.Walk(10, 0);
        }
        else
        {
            FinishCutscene(camera);
        }
    }

    private void FinishCutscene(Camera camera)
    {
        GameMode = GameMode.PLAYING;
        SecondsPerFrame = 1.0 / 60.0;
        camera.Zoom = 1.0f;
        _fadeAlph = 0.0f;
        _player.Move(PlayerStartX, PlayerStartY);
    }

    public void Draw(SpriteBatch spriteBatch, Camera camera)
    {
            var cameraTransform = camera.GetTransform();
            spriteBatch.Begin(transformMatrix: cameraTransform);

            _player.Draw(spriteBatch, _player.Position);
            foreach(var row in _platform)
            {
                spriteBatch.Draw(_platformeTexture, row, Color.Purple);
            }
            if (GameMode == GameMode.FADE_OUT || GameMode == GameMode.FADE_IN || GameMode == GameMode.CUTSCENE)
            {
                spriteBatch.Draw(fadeTexture, _fadeRec, new Color(Color.Black, _fadeAlph));
            }
            spriteBatch.End();
    }
}
