using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DontLikePoetry;

public class Game1 : Game
{
    private const int LogicalWidth = 1920;
    private const int LogicalHeight = 1080;
    private const int WindowMargin = 64;
    private const double SecondsPerFrameMenu = 1.0 / 60.0;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _gameRenderTarget;
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
        Point initialWindowSize = GetInitialWindowSize();

        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = initialWindowSize.X;
        _graphics.PreferredBackBufferHeight = initialWindowSize.Y;
        _graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnClientSizeChanged;
    }

    protected override void Initialize()
    { 
        _scene = new Scene();

        SpriteFont font = Content.Load<SpriteFont>("font");
        Vector2 menuCenter = new Vector2(LogicalWidth / 2, LogicalHeight / 2);

        _startMenu = new StartMenu(font, menuCenter);
        _pauseMenu = new PauseMenu(font, menuCenter);
        _deathMenu = new DeathMenu(font, menuCenter);

        Vector2 cameraPosition = new Vector2(LogicalWidth / 2, LogicalHeight / 2);
        Vector2 cameraDimensions = new Vector2(LogicalWidth, LogicalHeight);

        _camera = new Camera(cameraPosition, cameraDimensions, 1.0f);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _gameRenderTarget = new RenderTarget2D(GraphicsDevice, LogicalWidth, LogicalHeight);
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
        GraphicsDevice.SetRenderTarget(_gameRenderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        if(_activeScreen == AppScreen.PLAYING)
        {
            DrawPlaying();
        }
        if(_activeScreen == AppScreen.START_MENU)
        {
            DrawStartMenu();
        }

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _spriteBatch.Draw(_gameRenderTarget, GetPresentationRectangle(), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Point GetInitialWindowSize()
    {
        DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
        int availableWidth = displayMode.Width - WindowMargin;
        int availableHeight = displayMode.Height - WindowMargin;
        float widthScale = availableWidth / (float)LogicalWidth;
        float heightScale = availableHeight / (float)LogicalHeight;
        float scale = Math.Min(1.0f, Math.Min(widthScale, heightScale));

        return new Point((int)(LogicalWidth * scale), (int)(LogicalHeight * scale));
    }

    private Rectangle GetPresentationRectangle()
    {
        Viewport viewport = GraphicsDevice.Viewport;
        float widthScale = viewport.Width / (float)LogicalWidth;
        float heightScale = viewport.Height / (float)LogicalHeight;
        float scale = Math.Min(widthScale, heightScale);
        int width = (int)(LogicalWidth * scale);
        int height = (int)(LogicalHeight * scale);
        int x = (viewport.Width - width) / 2;
        int y = (viewport.Height - height) / 2;

        return new Rectangle(x, y, width, height);
    }

    private void OnClientSizeChanged(object sender, EventArgs eventArgs)
    {
        int width = Window.ClientBounds.Width;
        int height = Window.ClientBounds.Height;

        if (width <= 0 || height <= 0 ||
            _graphics.PreferredBackBufferWidth == width && _graphics.PreferredBackBufferHeight == height)
        {
            return;
        }

        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
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
