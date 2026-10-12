using System.Diagnostics;
using System.Drawing;
using AStar;
using AStar.Options;

namespace Zmija_New
{
    /// <summary>
    /// The original autopilot: A* on a board without wrap-around, the whole snake is an obstacle.
    /// When there is no path to the food, it heads for a random free field.
    /// </summary>
    public class AStarAutopilot : IAutopilot
    {
        // Used to be unbounded recursion, which ran out of memory when the snake got trapped
        private const int maxRandomTargets = 20;

        private readonly int columns;
        private readonly int rows;

        private readonly Random rnd = new Random();

        public AStarAutopilot(int columns, int rows)
        {
            this.columns = columns;
            this.rows = rows;
        }

        public Snake.MovingDirection NextDirection(List<Point> body, Point? food, Snake.MovingDirection currentDirection)
        {
            Point head = body[0];

            Point destination = food ?? RandomFreeField(CreateGrid(body, head, false));

            WorldGrid world = CreateGrid(body, destination, food.HasValue);

            var pathFinder = new PathFinder(world, new PathFinderOptions { UseDiagonals = false });

            Point? next = FirstStep(pathFinder, head, destination);

            for (int attempt = 0; next == null && attempt < maxRandomTargets; attempt++)
            {
                next = FirstStep(pathFinder, head, RandomFreeField(world));
            }

            if (next == null)
            {
                return currentDirection;
            }

            return DirectionTo(head, next.Value, currentDirection);
        }

        private static Point? FirstStep(PathFinder pathFinder, Point head, Point destination)
        {
            Position[] path = pathFinder.FindPath(new Position(head.Y, head.X), new Position(destination.Y, destination.X));

            foreach (Position point in path)
            {
                Debug.WriteLine($">>PATH<< X: {point.Column}, Y: {point.Row}");
            }

            if (path.Length > 1 && path[0].Column == head.X && path[0].Row == head.Y)
            {
                return new Point(path[1].Column, path[1].Row);
            }

            return null;
        }

        private Point RandomFreeField(WorldGrid world)
        {
            int x = rnd.Next(0, columns);
            int y = rnd.Next(0, rows);

            while (world[y, x] == 0)
            {
                x = rnd.Next(0, columns);
                y = rnd.Next(0, rows);
            }

            return new Point(x, y);
        }

        private static Snake.MovingDirection DirectionTo(Point head, Point next, Snake.MovingDirection currentDirection)
        {
            if (head.Y == next.Y && head.X < next.X)
            {
                return Snake.MovingDirection.Right;
            }

            if (head.Y == next.Y && head.X > next.X)
            {
                return Snake.MovingDirection.Left;
            }

            if (head.X == next.X && head.Y > next.Y)
            {
                return Snake.MovingDirection.Up;
            }

            if (head.X == next.X && head.Y < next.Y)
            {
                return Snake.MovingDirection.Down;
            }

            return currentDirection;
        }

        /// <summary>
        /// Grid for A*: 0 is closed (snake), 1 is open.
        /// When going for food, also closes some fields around it (original heuristic).
        /// </summary>
        private WorldGrid CreateGrid(List<Point> body, Point destination, bool destinationIsFood)
        {
            var grid = new WorldGrid(rows, columns);
            var snakePoints = new HashSet<Point>(body);

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    grid[row, column] = (short)(snakePoints.Contains(new Point(column, row)) ? 0 : 1);
                }
            }

            if (!destinationIsFood)
            {
                return grid;
            }

            var markedPoints = new HashSet<Point>();

            if (destination.X == 0)
            {
                markedPoints.Add(new Point(destination.X + 1, destination.Y));
            }

            if (destination.X == columns - 1)
            {
                markedPoints.Add(new Point(destination.X - 1, destination.Y));
            }

            if (destination.Y == 0)
            {
                markedPoints.Add(new Point(destination.X, destination.Y + 1));
            }

            if (destination.Y == rows - 1)
            {
                markedPoints.Add(new Point(destination.X, destination.Y - 1));
            }

            foreach (Point snakePoint in snakePoints)
            {
                if (snakePoint.X + 1 == destination.X && snakePoint.Y == destination.Y
                    && destination.X + 1 < columns)
                {
                    markedPoints.Add(new Point(destination.X + 1, destination.Y));
                }

                if (snakePoint.X - 1 == destination.X && snakePoint.Y == destination.Y
                    && destination.X - 1 > 0)
                {
                    markedPoints.Add(new Point(destination.X - 1, destination.Y));
                }

                if (snakePoint.Y + 1 == destination.Y && snakePoint.X == destination.X
                    && destination.Y + 1 < rows)
                {
                    markedPoints.Add(new Point(destination.X, destination.Y + 1));
                }

                if (snakePoint.Y - 1 == destination.Y && snakePoint.X == destination.X
                    && destination.Y - 1 > 0)
                {
                    markedPoints.Add(new Point(destination.X, destination.Y - 1));
                }
            }

            foreach (Point markedPoint in markedPoints)
            {
                grid[markedPoint.Y, markedPoint.X] = 0;
            }

            return grid;
        }
    }
}
