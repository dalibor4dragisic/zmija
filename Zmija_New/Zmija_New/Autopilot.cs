using System.Drawing;

namespace Zmija_New
{
    /// <summary>
    /// Chooses the snake's next move. Works on a lightweight model of the snake
    /// (list of points, head first, tail last) on a board whose edges wrap around.
    ///
    /// Strategy, in order:
    /// 1. Food  - take the shortest path to the food, but only if after eating
    ///            the snake can still reach its own tail (it is not trapped).
    /// 2. Tail  - otherwise follow the tail, taking the longest safe way to it,
    ///            to buy time until the space opens up.
    /// 3. Space - otherwise move to the neighbour with the most reachable fields.
    /// </summary>
    public class Autopilot : IAutopilot
    {
        private static readonly Snake.MovingDirection[] directions =
        {
            Snake.MovingDirection.Left,
            Snake.MovingDirection.Right,
            Snake.MovingDirection.Up,
            Snake.MovingDirection.Down
        };

        private readonly int columns;
        private readonly int rows;

        public Autopilot(int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
        }

        public Snake.MovingDirection NextDirection(List<Point> body, Point? food, Snake.MovingDirection currentDirection)
        {
            if (food.HasValue)
            {
                Snake.MovingDirection? towardsFood = SafePathToFood(body, food.Value);

                if (towardsFood.HasValue)
                {
                    return towardsFood.Value;
                }
            }

            Snake.MovingDirection? towardsTail = FollowTail(body, food);

            if (towardsTail.HasValue)
            {
                return towardsTail.Value;
            }

            return MostSpace(body, food) ?? currentDirection;
        }

        /// <summary>
        /// BFS from the head to the goal. Returns the shortest path (without the head),
        /// or null when the goal cannot be reached.
        /// </summary>
        public List<Point> FindPath(List<Point> body, Point goal)
        {
            Point start = body[0];
            Dictionary<Point, int> freeAt = GetFreeAt(body);

            var queue = new Queue<(Point point, int step)>();
            var cameFrom = new Dictionary<Point, Point>();

            queue.Enqueue((start, 0));
            cameFrom[start] = start;

            while (queue.Count > 0)
            {
                (Point current, int step) = queue.Dequeue();

                if (current == goal && current != start)
                {
                    var path = new List<Point>();

                    while (current != start)
                    {
                        path.Add(current);
                        current = cameFrom[current];
                    }

                    path.Reverse();

                    return path;
                }

                foreach (Snake.MovingDirection direction in directions)
                {
                    Point next = Neighbour(current, direction);

                    if (cameFrom.ContainsKey(next) || !IsFree(freeAt, next, step + 1))
                    {
                        continue;
                    }

                    cameFrom[next] = current;
                    queue.Enqueue((next, step + 1));
                }
            }

            return null;
        }

        public Point Neighbour(Point point, Snake.MovingDirection direction)
        {
            switch (direction)
            {
                case Snake.MovingDirection.Left:
                    return new Point((point.X - 1 + columns) % columns, point.Y);
                case Snake.MovingDirection.Right:
                    return new Point((point.X + 1) % columns, point.Y);
                case Snake.MovingDirection.Up:
                    return new Point(point.X, (point.Y - 1 + rows) % rows);
                case Snake.MovingDirection.Down:
                    return new Point(point.X, (point.Y + 1) % rows);
                default:
                    return point;
            }
        }

        /// <summary>
        /// Simulates the whole path to the food and checks that the snake can still
        /// reach its tail afterwards.
        /// </summary>
        private Snake.MovingDirection? SafePathToFood(List<Point> body, Point food)
        {
            List<Point> path = FindPath(body, food);

            if (path == null)
            {
                return null;
            }

            List<Point> virtualBody = body;

            for (int i = 0; i < path.Count; i++)
            {
                virtualBody = Move(virtualBody, path[i], path[i] == food);
            }

            if (FindPath(virtualBody, virtualBody[virtualBody.Count - 1]) == null)
            {
                return null;
            }

            return DirectionTo(body[0], path[0]);
        }

        /// <summary>
        /// Of all moves after which the tail is still reachable, takes the one
        /// with the longest way to the tail.
        /// </summary>
        private Snake.MovingDirection? FollowTail(List<Point> body, Point? food)
        {
            Dictionary<Point, int> freeAt = GetFreeAt(body);

            Snake.MovingDirection? best = null;
            int bestDistance = -1;

            foreach (Snake.MovingDirection direction in directions)
            {
                Point next = Neighbour(body[0], direction);

                if (!IsFree(freeAt, next, 1))
                {
                    continue;
                }

                List<Point> virtualBody = Move(body, next, next == food);
                List<Point> pathToTail = FindPath(virtualBody, virtualBody[virtualBody.Count - 1]);

                if (pathToTail != null && pathToTail.Count > bestDistance)
                {
                    best = direction;
                    bestDistance = pathToTail.Count;
                }
            }

            return best;
        }

        /// <summary>
        /// Last resort: the move after which the most fields are reachable (flood fill).
        /// </summary>
        private Snake.MovingDirection? MostSpace(List<Point> body, Point? food)
        {
            Dictionary<Point, int> freeAt = GetFreeAt(body);

            Snake.MovingDirection? best = null;
            int bestSpace = -1;

            foreach (Snake.MovingDirection direction in directions)
            {
                Point next = Neighbour(body[0], direction);

                if (!IsFree(freeAt, next, 1))
                {
                    continue;
                }

                int space = CountReachable(Move(body, next, next == food));

                if (space > bestSpace)
                {
                    best = direction;
                    bestSpace = space;
                }
            }

            return best;
        }

        private int CountReachable(List<Point> body)
        {
            Dictionary<Point, int> freeAt = GetFreeAt(body);

            var queue = new Queue<(Point point, int step)>();
            var visited = new HashSet<Point> { body[0] };

            queue.Enqueue((body[0], 0));

            while (queue.Count > 0)
            {
                (Point current, int step) = queue.Dequeue();

                foreach (Snake.MovingDirection direction in directions)
                {
                    Point next = Neighbour(current, direction);

                    if (visited.Contains(next) || !IsFree(freeAt, next, step + 1))
                    {
                        continue;
                    }

                    visited.Add(next);
                    queue.Enqueue((next, step + 1));
                }
            }

            return visited.Count - 1;
        }

        /// <summary>
        /// Returns a new body moved one field: the head goes to newHead, and the tail
        /// stays in place when the snake grows (eats).
        /// </summary>
        private static List<Point> Move(List<Point> body, Point newHead, bool grow)
        {
            var moved = new List<Point>(body.Count + 1) { newHead };

            moved.AddRange(body);

            if (!grow)
            {
                moved.RemoveAt(moved.Count - 1);
            }

            return moved;
        }

        public Snake.MovingDirection DirectionTo(Point from, Point to)
        {
            foreach (Snake.MovingDirection direction in directions)
            {
                if (Neighbour(from, direction) == to)
                {
                    return direction;
                }
            }

            return Snake.MovingDirection.Stop;
        }

        /// <summary>
        /// For every field occupied by the snake: after how many moves it becomes free.
        /// The tail leaves its field after 1 move, the segment before it after 2 moves, etc.
        /// </summary>
        public static Dictionary<Point, int> GetFreeAt(List<Point> body)
        {
            var freeAt = new Dictionary<Point, int>();

            for (int i = 0; i < body.Count; i++)
            {
                freeAt[body[i]] = body.Count - i;
            }

            return freeAt;
        }

        public static bool IsFree(Dictionary<Point, int> freeAt, Point point, int step)
        {
            return !freeAt.TryGetValue(point, out int freeAtStep) || step >= freeAtStep;
        }
    }
}
