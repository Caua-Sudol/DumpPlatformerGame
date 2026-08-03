using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public enum AppScreen
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
    private AppScreen _activeScreen = AppScreen.START_MENU;

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

        if(_activeScreen == AppScreen.PLAYING)
        {
            UpdatePlaying(deltaTime);
        }
        if(_activeScreen == AppScreen.START_MENU)
        {
            UpdateStartMenu();
        }
        
        TargetElapsedTime = TimeSpan.FromSeconds(_scene.FPS);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if(_activeScreen == AppScreen.PLAYING)
        {
            DrawPlaying();
        }
        if(_activeScreen == AppScreen.START_MENU)
        {
            DrawStartMenu();
        }

        base.Draw(gameTime);
    }

    private void UpdateStartMenu()
    {
        _startMenu.Update();
        
        if(_startMenu._currentOption == Option.START && _startMenu._enterIsPressed)
        {
            _activeScreen = AppScreen.PLAYING;
            _startMenu.Pressed = false;
        }
        else if(_startMenu._currentOption == Option.EXIT && _startMenu._enterIsPressed)
        {
            Exit();
        }
    }

    private void UpdatePlaying(double deltaTime)
    {
        _scene.Update(_camera, deltaTime);
    }

    private void DrawStartMenu()
    {
        _startMenu.Draw(_spriteBatch);
    }

    private void DrawPlaying()
    {
        _scene.Draw(_spriteBatch, _camera);
    }
}
