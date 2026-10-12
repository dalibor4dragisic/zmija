using System.Drawing;

namespace Zmija_New
{
    /// <summary>
    /// BFS autopilot while the snake is short (faster, more natural),
    /// Hamiltonian cycle once it gets long (no collisions).
    /// </summary>
    public class HybridAutopilot : IAutopilot
    {
        // Share of the board the snake has to take up before switching to the Hamiltonian cycle
        public const double SwitchToHamiltonAt = 0.25;

        private readonly Autopilot autopilot;
        private readonly HamiltonAutopilot hamilton;

        private readonly int switchLength;

        public HybridAutopilot(int columns, int rows)
        {
            autopilot = new Autopilot(columns, rows);
            hamilton = new HamiltonAutopilot(columns, rows);

            switchLength = (int)(columns * rows * SwitchToHamiltonAt);
        }

        public Snake.MovingDirection NextDirection(List<Point> body, Point? food, Snake.MovingDirection currentDirection)
        {
            if (body.Count < switchLength)
            {
                return autopilot.NextDirection(body, food, currentDirection);
            }

            return hamilton.NextDirection(body, food, currentDirection);
        }
    }
}
