using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum CurrentScreen
{
    START_MENU = 1,
    PLAYING = 2
}
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private int windowWidth = 1920;
    private int windowHeight = 1080;
    private Camera _camera;
    private Scene _scene;
    private StartMenu _startMenu;
    private CurrentScreen _activeScreen = CurrentScreen.START_MENU;

    private Vector2 positionCamera;
    private Vector2 dimentionsCamera;

    private SpriteFont font;
    private Vector2 fontPositionStart;
    private Vector2 fontPositionExit;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = windowWidth;
        _graphics.PreferredBackBufferHeight = windowHeight;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    { 
        _scene = new Scene();

        font = Content.Load<SpriteFont>("font");
        fontPositionStart = new Vector2(windowWidth/2, windowHeight/2);
        fontPositionExit = new Vector2(windowWidth/2, windowHeight/2 + 20);

        _startMenu = new StartMenu(font, fontPositionStart, fontPositionExit);

        positionCamera = new Vector2((float)windowWidth/2, (float)windowHeight/2);
        dimentionsCamera = new Vector2((float)windowWidth, (float)windowHeight);

        _camera = new Camera(positionCamera, dimentionsCamera, 1.0f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scene.LoadContent(GraphicsDevice);
        _startMenu.LoadContent(GraphicsDevice);

        TargetElapsedTime = TimeSpan.FromSeconds(_scene.FPS);
    }

    protected override void Update(GameTime gameTime)
    {
        double deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if(_scene.Player.Position.Y >= windowHeight)
            Exit(); // ajustar para reiniciar a fase e não fechar o jogo

        if(_activeScreen == CurrentScreen.PLAYING)
        {
            _scene.Update(_camera, deltaTime);
        }
        if(_activeScreen == CurrentScreen.START_MENU)
        {
            _startMenu.Update();
            
            if(_startMenu._currentOption == Option.START && _startMenu._enterIsPressed)
            {
                _activeScreen = CurrentScreen.PLAYING;
                _startMenu.Pressed = false;
            }
            else if(_startMenu._currentOption == Option.EXIT && _startMenu._enterIsPressed)
            {
                Exit();
            }
        }
        
        TargetElapsedTime = TimeSpan.FromSeconds(_scene.FPS);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if(_activeScreen == CurrentScreen.PLAYING)
        {
            _scene.Draw(_spriteBatch, _camera);
        }
        if(_activeScreen == CurrentScreen.START_MENU)
        {
            _startMenu.Draw(_spriteBatch);
        }

        base.Draw(gameTime);
    }
}
