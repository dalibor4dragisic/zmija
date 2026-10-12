using System.Drawing;

namespace Zmija_New
{
    /// <summary>
    /// Follows a Hamiltonian cycle - a closed path through every field of the board exactly once.
    /// While the snake lies on the cycle in order (tail ... head), following the cycle can never
    /// end in a collision, not even when the snake fills the whole board.
    ///
    /// Shortcuts: the head may skip a part of the cycle towards the food, as long as it stays
    /// behind its own tail on the cycle (with a safety margin).
    /// </summary>
    public class HamiltonAutopilot : IAutopilot
    {
        // Free fields on the cycle kept between the head and the tail when taking a shortcut
        private const int safetyMargin = 3;

        private readonly Autopilot autopilot;

        private readonly Point[] cycle;
        private readonly Dictionary<Point, int> cycleIndex = new Dictionary<Point, int>();

        public HamiltonAutopilot(int columns, int rows)
        {
            autopilot = new Autopilot(columns, rows);

            cycle = CreateCycle(columns, rows);

            for (int i = 0; i < cycle.Length; i++)
            {
                cycleIndex[cycle[i]] = i;
            }
        }

        public Snake.MovingDirection NextDirection(List<Point> body, Point? food, Snake.MovingDirection currentDirection)
        {
            if (!IsOnCycle(body))
            {
                return JoinCycle(body, food, currentDirection);
            }

            Point head = body[0];
            int headIndex = cycleIndex[head];
            int tailIndex = cycleIndex[body[body.Count - 1]];

            Point best = cycle[(headIndex + 1) % cycle.Length];
            int bestDistance = 1;

            // Shortcuts only while at least half of the board is free
            if (food.HasValue && cycle.Length - body.Count > cycle.Length / 2)
            {
                // 1 for the growth after eating
                int maxDistance = Math.Min(
                    Distance(headIndex, tailIndex) - 1 - safetyMargin,
                    Distance(headIndex, cycleIndex[food.Value]));

                var snakePoints = new HashSet<Point>(body);

                foreach (Snake.MovingDirection direction in new[] { Snake.MovingDirection.Left, Snake.MovingDirection.Right, Snake.MovingDirection.Up, Snake.MovingDirection.Down })
                {
                    Point next = autopilot.Neighbour(head, direction);

                    if (snakePoints.Contains(next))
                    {
                        continue;
                    }

                    int distance = Distance(headIndex, cycleIndex[next]);

                    if (distance <= maxDistance && distance > bestDistance)
                    {
                        best = next;
                        bestDistance = distance;
                    }
                }
            }

            return autopilot.DirectionTo(head, best);
        }

        /// <summary>
        /// The snake is not on the cycle yet (new mode chosen during the game, or hybrid switching over).
        /// Follows the cycle if every field of it ahead is free by the time the head gets there;
        /// after body.Count such moves the snake lies on the cycle. Otherwise moves like the BFS autopilot.
        /// </summary>
        private Snake.MovingDirection JoinCycle(List<Point> body, Point? food, Snake.MovingDirection currentDirection)
        {
            Dictionary<Point, int> freeAt = Autopilot.GetFreeAt(body);
            int headIndex = cycleIndex[body[0]];

            bool safe = true;

            for (int step = 1; step <= body.Count && safe; step++)
            {
                // step - 1: the tail stays in place one move if the snake eats on the way
                safe = Autopilot.IsFree(freeAt, cycle[(headIndex + step) % cycle.Length], step - 1);
            }

            if (safe)
            {
                return autopilot.DirectionTo(body[0], cycle[(headIndex + 1) % cycle.Length]);
            }

            return autopilot.NextDirection(body, food, currentDirection);
        }

        /// <summary>
        /// Going from the head towards the tail, every segment must lie further back on the cycle.
        /// </summary>
        private bool IsOnCycle(List<Point> body)
        {
            int tailIndex = cycleIndex[body[body.Count - 1]];
            int previous = int.MaxValue;

            foreach (Point point in body)
            {
                int distanceFromTail = Distance(tailIndex, cycleIndex[point]);

                if (distanceFromTail >= previous)
                {
                    return false;
                }

                previous = distanceFromTail;
            }

            return true;
        }

        /// <summary>
        /// Number of moves along the cycle from one index to the other.
        /// </summary>
        private int Distance(int from, int to)
        {
            return (to - from + cycle.Length) % cycle.Length;
        }

        /// <summary>
        /// Row 0 left to right, then the rows in a zigzag through columns 1 .. columns - 1,
        /// and back up along column 0. Needs an even number of rows.
        /// </summary>
        private static Point[] CreateCycle(int columns, int rows)
        {
            if (rows % 2 != 0)
            {
                throw new ArgumentException("Hamiltonian cycle needs an even number of rows.", nameof(rows));
            }

            var cycle = new List<Point>();

            for (int x = 0; x < columns; x++)
            {
                cycle.Add(new Point(x, 0));
            }

            for (int y = 1; y < rows; y++)
            {
                if (y % 2 == 1)
                {
                    for (int x = columns - 1; x >= 1; x--)
                    {
                        cycle.Add(new Point(x, y));
                    }
                }
                else
                {
                    for (int x = 1; x < columns; x++)
                    {
                        cycle.Add(new Point(x, y));
                    }
                }
            }

            for (int y = rows - 1; y >= 1; y--)
            {
                cycle.Add(new Point(0, y));
            }

            return cycle.ToArray();
        }
    }
}
