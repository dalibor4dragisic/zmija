using System.Drawing;

namespace Zmija_New
{
    public enum GameMode
    {
        Manual,
        AStar,
        Auto,
        Hamilton,
        Hybrid
    }

    /// <summary>
    /// Chooses the snake's next move.
    /// The snake is given as a list of points: head first, tail last.
    /// </summary>
    public interface IAutopilot
    {
        Snake.MovingDirection NextDirection(List<Point> body, Point? food, Snake.MovingDirection currentDirection);
    }
}
