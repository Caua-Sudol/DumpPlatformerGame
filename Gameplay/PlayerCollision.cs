using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DontLikePoetry;

public class PlayerCollision
{
    public void MoveAndResolve(Player player, IEnumerable<Rectangle> platforms, double deltaTime)
    {
        player.WalkX(player.Velocity.X, deltaTime);
        ResolveHorizontalCollision(player, platforms);

        player.NotGrounded();
        player.WalkY(player.Velocity.Y, deltaTime);
        ResolveVerticalCollision(player, platforms);
        UpdateGroundedState(player, platforms);
    }

    private void ResolveHorizontalCollision(Player player, IEnumerable<Rectangle> platforms)
    {
        foreach (var platform in platforms)
        {
            if (!player.HitBox.Intersects(platform))
            {
                continue;
            }

            var playerPosition = player.Position;

            if (player.Velocity.X > 0)
            {
                playerPosition.X = platform.Left - player.HitBox.Width;
            }
            else if (player.Velocity.X < 0)
            {
                playerPosition.X = platform.Right;
            }

            player.Position = playerPosition;
            player.StopWalkX();
            return;
        }
    }

    private void ResolveVerticalCollision(Player player, IEnumerable<Rectangle> platforms)
    {
        foreach (var platform in platforms)
        {
            if (!player.HitBox.Intersects(platform))
            {
                continue;
            }

            var playerPosition = player.Position;

            if (player.Velocity.Y > 0)
            {
                playerPosition.Y = platform.Top - player.HitBox.Height;
            }
            else if (player.Velocity.Y < 0)
            {
                playerPosition.Y = platform.Bottom;
            }

            player.Position = playerPosition;
            player.StopWalkY();
            return;
        }
    }

    private void UpdateGroundedState(Player player, IEnumerable<Rectangle> platforms)
    {
        var groundCheck = new Rectangle(player.HitBox.X, player.HitBox.Bottom, player.HitBox.Width, 1);

        foreach (var platform in platforms)
        {
            if (!groundCheck.Intersects(platform))
            {
                continue;
            }

            player.Grounded();
            player.CancelDash();
            player.RestoreDash();
            return;
        }
    }
}
