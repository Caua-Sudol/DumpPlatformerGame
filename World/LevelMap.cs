using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TiledSharp;

namespace DontLikePoetry;

public class LevelMap
{
    private const int TileSize = 32;
    private const string PlatformLayerName = "mapaGeral";
    private const string CutsceneTriggerLayerName = "triggerColision";

    private readonly string _mapPath;
    private readonly List<Rectangle> _platforms = new List<Rectangle>();
    private readonly List<Rectangle> _cutsceneTriggers = new List<Rectangle>();
    private Texture2D _platformTexture;

    public IEnumerable<Rectangle> Platforms
    {
        get
        {
            return _platforms;
        }
    }

    public LevelMap(string mapPath)
    {
        _mapPath = mapPath;
    }

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        LoadObjects();
        LoadPlatformTexture(graphicsDevice);
    }

    public bool HasCutsceneTrigger(Rectangle hitBox)
    {
        foreach (var trigger in _cutsceneTriggers)
        {
            if (hitBox.Intersects(trigger))
            {
                return true;
            }
        }

        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var platform in _platforms)
        {
            spriteBatch.Draw(_platformTexture, platform, Color.Purple);
        }
    }

    private void LoadObjects()
    {
        _platforms.Clear();
        _cutsceneTriggers.Clear();

        var map = new TmxMap(_mapPath);

        foreach (var row in map.ObjectGroups[PlatformLayerName].Objects)
        {
            _platforms.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }

        foreach (var row in map.ObjectGroups[CutsceneTriggerLayerName].Objects)
        {
            _cutsceneTriggers.Add(new Rectangle((int)row.X, (int)row.Y, (int)row.Width, (int)row.Height));
        }
    }

    private void LoadPlatformTexture(GraphicsDevice graphicsDevice)
    {
        var platformColor = Enumerable.Repeat(Color.Purple, TileSize * TileSize).ToArray();
        _platformTexture = new Texture2D(graphicsDevice, TileSize, TileSize);
        _platformTexture.SetData(platformColor);
    }
}
