using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public class Game1 : Game
{
    private const int WindowWidth = 1920;
    private const int WindowHeight = 1080;
    private const double SecondsPerFrameMenu = 1.0 / 60.0;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Camera _camera;
    private Scene _scene;
    private StartMenu _startMenu;
    private PauseMenu _pauseMenu;
    private DeathMenu _deathMenu;
    private AppScreen _activeScreen = AppScreen.START_MENU;
    private OverlayState _overlayState = OverlayState.NONE;
    private KeyboardState _previousKeyboardState;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = WindowWidth;
        _graphics.PreferredBackBufferHeight = WindowHeight;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    { 
        _scene = new Scene();

        SpriteFont font = Content.Load<SpriteFont>("font");
        Vector2 menuPosition = new Vector2(WindowWidth / 2, WindowHeight / 2);

        _startMenu = new StartMenu(font, menuPosition, menuPosition + new Vector2(0, 20));
        _pauseMenu = new PauseMenu(font, menuPosition);
        _deathMenu = new DeathMenu(font, menuPosition);

        Vector2 cameraPosition = new Vector2(WindowWidth / 2, WindowHeight / 2);
        Vector2 cameraDimensions = new Vector2(WindowWidth, WindowHeight);

        _camera = new Camera(cameraPosition, cameraDimensions, 1.0f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _scene.LoadContent(GraphicsDevice);

        TargetElapsedTime = TimeSpan.FromSeconds(SecondsPerFrameMenu);
    }

    protected override void Update(GameTime gameTime)
    {
        double deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        if(_activeScreen == AppScreen.PLAYING)
        {
            UpdatePlaying(keyboardState, deltaTime);
        }
        if(_activeScreen == AppScreen.START_MENU)
        {
            UpdateStartMenu(keyboardState);
        }

        _previousKeyboardState = keyboardState;
        
        TargetElapsedTime = TimeSpan.FromSeconds(GetSecondsPerFrame());

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

    private void UpdateStartMenu(KeyboardState keyboardState)
    {
        StartMenuOption selectedOption = _startMenu.Update(keyboardState);
        
        if(selectedOption == StartMenuOption.START)
        {
            _activeScreen = AppScreen.PLAYING;
            _overlayState = OverlayState.NONE;
        }
        else if(selectedOption == StartMenuOption.EXIT)
        {
            Exit();
        }
    }

    private double GetSecondsPerFrame()
    {
        if (_activeScreen == AppScreen.PLAYING && _overlayState == OverlayState.NONE)
        {
            return _scene.SecondsPerFrame;
        }

        return SecondsPerFrameMenu;
    }

    private void UpdatePlaying(KeyboardState keyboardState, double deltaTime)
    {
        if (_overlayState == OverlayState.DEATH_MENU)
        {
            UpdateDeathMenu(keyboardState);
            return;
        }

        if (EscapeWasPressed(keyboardState))
        {
            TogglePause();
        }

        if (_overlayState == OverlayState.PAUSED)
        {
            UpdatePauseMenu(keyboardState);
            return;
        }

        _scene.Update(_camera, deltaTime);

        if (_scene.PlayerIsDead)
        {
            OpenDeathMenu();
        }
    }

    private void UpdatePauseMenu(KeyboardState keyboardState)
    {
        PauseOption selectedOption = _pauseMenu.Update(keyboardState, _previousKeyboardState);

        if (selectedOption == PauseOption.RESUME)
            _overlayState = OverlayState.NONE;
        else if (selectedOption == PauseOption.RESTART)
        {
            _scene.Restart(_camera);
            _overlayState = OverlayState.NONE;
        }
        else if (selectedOption == PauseOption.QUIT)
            Exit();
    }

    private void UpdateDeathMenu(KeyboardState keyboardState)
    {
        DeathOption selectedOption = _deathMenu.Update(keyboardState, _previousKeyboardState);

        if (selectedOption == DeathOption.RETRY)
        {
            _scene.Restart(_camera);
            _overlayState = OverlayState.NONE;
        }
        else if (selectedOption == DeathOption.QUIT)
            Exit();
    }

    private bool EscapeWasPressed(KeyboardState keyboardState)
    {
        return KeyWasPressed(keyboardState, Keys.Escape);
    }

    private bool KeyWasPressed(KeyboardState keyboardState, Keys key)
    {
        return keyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    private void TogglePause()
    {
        if (_overlayState == OverlayState.PAUSED)
        {
            _overlayState = OverlayState.NONE;
        }
        else
        {
            _overlayState = OverlayState.PAUSED;
            _pauseMenu.Open();
        }
    }

    private void OpenDeathMenu()
    {
        _overlayState = OverlayState.DEATH_MENU;
        _deathMenu.Open();
    }

    private void DrawStartMenu()
    {
        _startMenu.Draw(_spriteBatch);
    }

    private void DrawPlaying()
    {
        _scene.Draw(_spriteBatch, _camera);

        if (_overlayState == OverlayState.PAUSED)
        {
            _pauseMenu.Draw(_spriteBatch);
        }
        else if (_overlayState == OverlayState.DEATH_MENU)
        {
            _deathMenu.Draw(_spriteBatch);
        }
    }
}
