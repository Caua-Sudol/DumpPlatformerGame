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

public enum OverlayState
{
    NONE = 0,
    PAUSED = 1
}

public enum PauseOption
{
    RESUME = 1,
    RESTART = 2,
    QUIT = 3
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
    private OverlayState _overlayState = OverlayState.NONE;
    private PauseOption _pauseOption = PauseOption.RESUME;
    private KeyboardState _previousKeyboardState;

    private Vector2 positionCamera;
    private Vector2 dimentionsCamera;

    private SpriteFont font;
    private Vector2 fontPositionStart;
    private Vector2 fontPositionExit;
    private Vector2 fontPositionPauseResume;
    private Vector2 fontPositionPauseRestart;
    private Vector2 fontPositionPauseQuit;

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
        fontPositionPauseResume = new Vector2(windowWidth/2, windowHeight/2);
        fontPositionPauseRestart = new Vector2(windowWidth/2, windowHeight/2 + 20);
        fontPositionPauseQuit = new Vector2(windowWidth/2, windowHeight/2 + 40);

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
        var keyboardState = Keyboard.GetState();

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            Exit();

        if(_activeScreen == AppScreen.PLAYING)
        {
            UpdatePlaying(keyboardState, deltaTime);
        }
        if(_activeScreen == AppScreen.START_MENU)
        {
            UpdateStartMenu();
        }

        _previousKeyboardState = keyboardState;
        
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
            _overlayState = OverlayState.NONE;
            _startMenu.Pressed = false;
        }
        else if(_startMenu._currentOption == Option.EXIT && _startMenu._enterIsPressed)
        {
            Exit();
        }
    }

    private void UpdatePlaying(KeyboardState keyboardState, double deltaTime)
    {
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
    }

    private void UpdatePauseMenu(KeyboardState keyboardState)
    {
        if (KeyWasPressed(keyboardState, Keys.W))
        {
            MovePauseOptionUp();
        }
        else if (KeyWasPressed(keyboardState, Keys.S))
        {
            MovePauseOptionDown();
        }

        if (KeyWasPressed(keyboardState, Keys.Enter))
        {
            SelectPauseOption();
        }
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
            _pauseOption = PauseOption.RESUME;
        }
    }

    private void MovePauseOptionUp()
    {
        if (_pauseOption == PauseOption.RESUME)
        {
            _pauseOption = PauseOption.QUIT;
        }
        else if (_pauseOption == PauseOption.RESTART)
        {
            _pauseOption = PauseOption.RESUME;
        }
        else if (_pauseOption == PauseOption.QUIT)
        {
            _pauseOption = PauseOption.RESTART;
        }
    }

    private void MovePauseOptionDown()
    {
        if (_pauseOption == PauseOption.RESUME)
        {
            _pauseOption = PauseOption.RESTART;
        }
        else if (_pauseOption == PauseOption.RESTART)
        {
            _pauseOption = PauseOption.QUIT;
        }
        else if (_pauseOption == PauseOption.QUIT)
        {
            _pauseOption = PauseOption.RESUME;
        }
    }

    private void SelectPauseOption()
    {
        if (_pauseOption == PauseOption.RESUME)
        {
            _overlayState = OverlayState.NONE;
        }
        else if (_pauseOption == PauseOption.RESTART)
        {
            _scene.Restart(_camera);
            _overlayState = OverlayState.NONE;
        }
        else if (_pauseOption == PauseOption.QUIT)
        {
            Exit();
        }
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
            DrawPauseOverlay();
        }
    }

    private void DrawPauseOverlay()
    {
        _spriteBatch.Begin();
        _spriteBatch.DrawString(font, "Resume", fontPositionPauseResume, GetPauseOptionColor(PauseOption.RESUME));
        _spriteBatch.DrawString(font, "Restart", fontPositionPauseRestart, GetPauseOptionColor(PauseOption.RESTART));
        _spriteBatch.DrawString(font, "Quit", fontPositionPauseQuit, GetPauseOptionColor(PauseOption.QUIT));
        _spriteBatch.End();
    }

    private Color GetPauseOptionColor(PauseOption option)
    {
        if (_pauseOption == option)
        {
            return Color.Yellow;
        }

        return Color.White;
    }
}
