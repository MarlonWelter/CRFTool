using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBase
{
    public interface IConnectedGridElement3D
    {
        IConnectedGridElement3D NeighbourBack { get; set; }
        IConnectedGridElement3D NeighbourFront { get; set; }
        IConnectedGridElement3D NeighbourLeft { get; set; }
        IConnectedGridElement3D NeighbourRight { get; set; }
        IConnectedGridElement3D NeighbourDown { get; set; }
        IConnectedGridElement3D NeighbourUp { get; set; }
    }

    public interface ILocalizableGridElement3D
    {
        int Column { get; }
        int Row { get; }
        int Level { get; }
    }

    public enum Direction
    {
        Left,
        Right,
        Back,
        Front,
        Down,
        Up
    }

    public static class GridOperation3D
    {
        public static Direction OppositeDirection(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                case Direction.Back:
                    return Direction.Front;
                case Direction.Front:
                    return Direction.Back;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Up:
                    return Direction.Down;
                default:
                    return default(Direction);
            }
        }
        public static T Neighbour<T>(this T gridElement, Direction direction) where T : IConnectedGridElement3D
        {
            switch (direction)
            {
                case Direction.Left:
                    return gridElement.NeighbourLeft();
                case Direction.Right:
                    return gridElement.NeighbourRight();
                case Direction.Back:
                    return gridElement.NeighbourBack();
                case Direction.Front:
                    return gridElement.NeighbourFront();
                case Direction.Down:
                    return gridElement.NeighbourDown();
                case Direction.Up:
                    return gridElement.NeighbourUp();
                default:
                    return default(T);
            }
        }
        public static T NeighbourBack<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourBack;
        }
        public static T NeighbourFront<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourFront;
        }
        public static T NeighbourLeft<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourLeft;
        }
        public static T NeighbourRight<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourRight;
        }
        public static T NeighbourDown<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourDown;
        }
        public static T NeighbourUp<T>(this T gridElement) where T : IConnectedGridElement3D
        {
            return (T)gridElement.NeighbourUp;
        }
        public static void Connect(this IConnectedGridElement3D element, IConnectedGridElement3D other, Direction direction)
        {
            if (element == null || other == null)
                throw new ArgumentException("null values not allowed.");
            switch (direction)
            {
                case Direction.Left:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourLeft = other;
                    other.NeighbourRight = element;
                    break;
                case Direction.Right:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourRight = other;
                    other.NeighbourLeft = element;
                    break;
                case Direction.Back:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourBack = other;
                    other.NeighbourFront = element;
                    break;
                case Direction.Front:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourFront = other;
                    other.NeighbourBack = element;
                    break;
                case Direction.Down:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourDown = other;
                    other.NeighbourUp = element;
                    break;
                case Direction.Up:
                    element.Disconnect(direction);
                    other.Disconnect(direction.OppositeDirection());
                    element.NeighbourUp = other;
                    other.NeighbourDown = element;
                    break;
                default:
                    break;
            }
        }
        public static void Disconnect(this IConnectedGridElement3D element, Direction direction)
        {
            var other = element.Neighbour(direction);
            if (other == null)
                return;

            switch (direction)
            {
                case Direction.Left:
                    other.NeighbourRight = null;
                    element.NeighbourLeft = null;
                    break;
                case Direction.Right:
                    other.NeighbourLeft = null;
                    element.NeighbourRight = null;
                    break;
                case Direction.Back:
                    other.NeighbourFront = null;
                    element.NeighbourBack = null;
                    break;
                case Direction.Front:
                    other.NeighbourFront = null;
                    element.NeighbourBack = null;
                    break;
                case Direction.Down:
                    other.NeighbourUp = null;
                    element.NeighbourDown = null;
                    break;
                case Direction.Up:
                    other.NeighbourDown = null;
                    element.NeighbourUp = null;
                    break;
                default:
                    break;
            }
        }

        public delegate bool Job<T>(T data);
        public const bool CONTINUEJOB = false;
        public const bool JOBFINISHED = true;
        public static void ExecuteMany<T>(this T element, Job<T> Job, int columns, int rows, int levels) where T : IConnectedGridElement3D
        {
            if (columns < 1 || rows < 1 || levels < 1 || element == null || Job == null)
                throw new ArgumentException("wrong input for ExecuteMany (" + columns + ", " + rows + ", " + levels + ")"); 

            var rootColumn = element;
            for (int pw = 1; pw <= columns; pw++)
            {
                var rootRow = rootColumn;
                for (int pl = 1; pl <= rows; pl++)
                {
                    var currentElement = rootRow;
                    for (int ph = 1; ph <= levels; ph++)
                    {
                        bool finish = Job(currentElement);
                        if(finish)
                            return;
                        currentElement = currentElement.NeighbourUp();
                    }
                    rootRow = rootRow.NeighbourFront();
                }
                rootColumn = rootColumn.NeighbourRight();
            }
        }
        public static void ExecuteMany<T>(this T element, Job<T> Job, int extensionDirOne, int extensionDirTwo, int extensionDirThree,
            Direction dirOne, Direction dirTwo, Direction dirThree) where T : IConnectedGridElement3D
        {
            if (extensionDirThree < 1 || extensionDirTwo < 1 || extensionDirOne < 1 || element == null || Job == null)
                throw new ArgumentException("wrong input for ExecuteMany");

            var rootColumn = element;
            for (int pw = 1; pw <= extensionDirThree; pw++)
            {
                var rootRow = rootColumn;
                for (int pl = 1; pl <= extensionDirTwo; pl++)
                {
                    var currentElement = rootRow;
                    for (int ph = 1; ph <= extensionDirOne; ph++)
                    {
                        bool finish = Job(currentElement);
                        if (finish)
                            return;
                        currentElement = currentElement.Neighbour(dirOne);
                    }
                    rootRow = rootRow.Neighbour(dirTwo);
                }
                rootColumn = rootColumn.Neighbour(dirThree);
            }
        }
        public static void ExecuteAll<T>(this T element, Job<T> Job) where T : IConnectedGridElement3D
        {
            if ( element == null || Job == null)
                throw new ArgumentException("wrong input for ExecuteMany");

            var rootColumn = element;
            while (rootColumn != null)
            {
                var rootRow = rootColumn;
                while (rootRow != null)
                {
                    var currentElement = rootRow;
                    while (currentElement != null)
                    {
                        bool finish = Job(currentElement);
                        if (finish)
                            return;
                        currentElement = currentElement.NeighbourUp();
                    }
                    rootRow = rootRow.NeighbourFront();
                }
                rootColumn = rootColumn.NeighbourRight();
            }
        }
        public static void ExecuteAll<T>(this T element, Job<T> Job, Direction directionOne, Direction directionTwo, Direction directionThree) where T : IConnectedGridElement3D
        {
            if (element == null || Job == null)
                throw new ArgumentException("wrong input for ExecuteMany");

            var rootDirOne = element;
            while (rootDirOne != null)
            {
                var rootDirTwo = rootDirOne;
                while (rootDirTwo != null)
                {
                    var currentElement = rootDirTwo;
                    while (currentElement != null)
                    {
                        bool finish = Job(currentElement);
                        if (finish)
                            return;
                        currentElement = currentElement.Neighbour(directionOne);
                    }
                    rootDirTwo = rootDirTwo.Neighbour(directionTwo);
                }
                rootDirOne = rootDirOne.Neighbour(directionThree);
            }
        }

        public static LinkedList<T> OrderedElements<T>(this T element) where T : IConnectedGridElement3D
        {
            var gatherer = new ElementGathering<T>();
            element.ExecuteAll(gatherer.Gather);
            return gatherer.Elements;
        }
    }
    class ElementGathering<T>
    {
        public LinkedList<T> Elements = new LinkedList<T>();
        public bool Gather(T data)
        {
            Elements.AddLast(data);
            return false;
        }
    }
}
