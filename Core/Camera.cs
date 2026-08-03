using Microsoft.Xna.Framework;

namespace DontLikePoetry;

public class Camera
{
   public float Zoom {get; set;}
   public Vector2 Position {get; set;}
   public Vector2 Dimensions {get; set;}

   public Camera(Vector2 position, Vector2 dimensions, float zoom)
    {
        this.Position = position;
        this.Dimensions = dimensions;
        this.Zoom = zoom;
    }

    public void Follow(Player player)
    {
        var playerCenter = player.HitBox.Center;
        LookAt(new Vector2(playerCenter.X, playerCenter.Y));
    }

    public void LookAt(Vector2 position)
    {
        Position = position;
    }

    public Matrix GetTransform()
    {
        Matrix translation = Matrix.CreateTranslation(-Position.X, -Position.Y, 0f);

        Matrix zoom = Matrix.CreateScale(Zoom, Zoom, 1f);

        Matrix center = Matrix.CreateTranslation(Dimensions.X / 2f, Dimensions.Y / 2f, 0f);

        return translation * zoom * center;
    }

}
