using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace Assets.Scripts
{
    public class LabirentAlgorithm
    {
        public enum GameLevel
        {
            easy
        }
        public Point startPoint;
        public Point finishPoint;
        private GameLevel gameLevel;
        private int[,] map;
        public Size mapLenght;
        private int minHorizantal;
        private int maxHorizantal;
        private int minVertical;
        private int maxVertical;
        private Random random = new Random();
        public List<Pivot> pivots = new List<Pivot>();
        private List<List<Point>> allPoints = new List<List<Point>>();

        public LabirentAlgorithm(GameLevel gameLevel, Size mapLenght, int minHorizantal, int maxHorizantal, int minVertical, int maxVertical)
        {
            this.gameLevel = gameLevel;
            this.mapLenght = mapLenght;
            this.minHorizantal = minHorizantal;
            this.maxHorizantal = maxHorizantal;
            this.minVertical = minVertical;
            this.maxVertical = maxVertical;
            FillMapArray(0);    // Zero is empty value of map 
        }
        private void FillMapArray(int startValue)
        {
            map = new int[mapLenght.Width, mapLenght.Height];
            for (int i = 0; i < mapLenght.Width; i++)
            {
                for (int j = 0; j < mapLenght.Height; j++)
                {
                    map[i, j] = startValue;
                }
            }
        }
        public int[,] GetRandomMap()
        {
            switch (gameLevel)
            {
                case GameLevel.easy:
                    return GetEasyMap();
                default:
                    return null;
            }
        }
        private int[,] GetEasyMap()
        {
            startPoint = GetStartPoint();
            Point[] mainWay = null;
            CreateMainWay(mainWay, startPoint, gameLevel);
            return map;
        }
        private void CreateMainWay(Point[] mainWay, Point startPoint, GameLevel gameLevel)
        {
            List<Point> mainWayPoints = new List<Point>();
            Point headOfMainWay = startPoint;
            mainWayPoints.Add(headOfMainWay);
            Pivot headPivot = new Pivot(Direction.East, 0, headOfMainWay);
            pivots.Add(headPivot);
            bool reverse = true;
            while (headOfMainWay.X < mapLenght.Width - 1)
            {

                if (reverse)        // That`s mean add horizontal way
                {
                    headOfMainWay = HorizantalPivotAdder(headOfMainWay, 0, mainWayPoints);
                }
                else                // That`s mean add vertical way
                {
                    headOfMainWay = VerticalPivotAdder(headOfMainWay, 0, mainWayPoints);
                }
                reverse = !reverse;
            }
            PassListToArray(mainWayPoints, mainWay);
            signToMap(mainWayPoints);
            finishPoint = pivots.Last<Pivot>().point;
            AddFalseWay();
            AddFalseWay();
        }
        private Point HorizantalPivotAdder(Point headOfMainWay, int layer, List<Point> mainWayPoints)
        {
            int lenght = GetRndHorizontal(headOfMainWay, true);
            Point lastElement = mainWayPoints.Last<Point>();
            headOfMainWay = AddHorizantalPoint(lastElement, mainWayPoints, lenght);
            Pivot pivot = new Pivot(Direction.East, layer, headOfMainWay);
            pivots.Add(pivot);
            return headOfMainWay;
        }
        private Point VerticalPivotAdder(Point headOfMainWay, int layer, List<Point> mainWayPoints)
        {
            bool goUp = random.Next(2) == 1 ? true : false;  // get random boolean value

            int lenght = GetRndVertical(headOfMainWay, goUp, true);
            Point lastElement = mainWayPoints.Last<Point>();
            headOfMainWay = AddVerticalPoint(lastElement, mainWayPoints, lenght, goUp);
            Pivot pivot = new Pivot(layer, headOfMainWay);
            if (goUp)
                pivot.direction = Direction.North;
            else
                pivot.direction = Direction.South;
            pivots.Add(pivot);
            return headOfMainWay;
        }
        private void AddFalseWay()
        {
            List<Pivot> refPivots = GetLastLayerPivots();
            List<Direction> newDirection = new List<Direction>();
            List<Point> pivotPoints = new List<Point>();
            List<Point> falsePoints = new List<Point>();
            for (int i = 2; i < refPivots.Count; i++)
            {
                List<Direction> willAddDirection = DetectWillAddDirection(refPivots[i].direction, refPivots[i - 1].direction);
                if (willAddDirection != null && willAddDirection.Count > 0)
                {
                    AddNewWayWithDirection(refPivots[i].point, willAddDirection, falsePoints, (refPivots[i].layerNo + 1));
                }
            }
            allPoints.Add(falsePoints);
            signToMap(falsePoints);
        }
        private List<Pivot> GetLastLayerPivots()
        {
            Pivot lastPivot = pivots.Last<Pivot>();
            int lastestLayer = lastPivot.layerNo;
            List<Pivot> newPivots = new List<Pivot>();
            for (int i = pivots.Count - 1; i > 0; i--)
            {
                if (pivots[i].layerNo == lastestLayer)
                    newPivots.Add(pivots[i]);
                else
                    return newPivots;
            }
            return newPivots;
        }
        private void AddNewWayWithDirection(Point point, List<Direction> directions, List<Point> falsePoints, int layer)
        {
            foreach (var item in directions)
            {
                Pivot pivot = null;
                switch (item)
                {
                    case Direction.North:
                        int lenght = GetRndVertical(point, true, false);
                        if (lenght > 0 && lenght < mapLenght.Height)
                        {
                            Point tempPoint = AddVerticalPoint(point, falsePoints, lenght, true);
                            pivot = new Pivot(Direction.North, layer, tempPoint);
                        }
                        break;
                    case Direction.South:
                        lenght = GetRndVertical(point, false, false);
                        if (lenght > 0 && lenght < mapLenght.Height)
                        {
                            Point tempPoint = AddVerticalPoint(point, falsePoints, lenght, false);
                            pivot = new Pivot(Direction.South, layer, tempPoint);
                        }
                        break;
                    case Direction.East:
                        lenght = GetRndHorizontal(point, false);
                        if (lenght > 0 && lenght < mapLenght.Height)
                        {
                            Point tempPoint = AddHorizantalPoint(point, falsePoints, lenght);
                            pivot = new Pivot(Direction.East, layer, tempPoint);
                        }
                        break;
                    case Direction.West:
                        break;
                    default:
                        break;
                }
                if (pivot != null)
                    pivots.Add(pivot);
            }
        }
        private List<Direction> DetectWillAddDirection(Direction curDirection, Direction preDirection)
        {
            List<Direction> directions = new List<Direction>();
            if (curDirection != Direction.East && preDirection != Direction.East)
                directions.Add(Direction.East);
            if (curDirection != Direction.North && preDirection != Direction.North)
                directions.Add(Direction.North);
            if (curDirection != Direction.South && preDirection != Direction.South)
                directions.Add(Direction.South);
            if (curDirection != Direction.West && preDirection != Direction.West)
                directions.Add(Direction.West);
            int rndVal = random.Next(1);
            if (directions.Count == 2)
            {
                if (rndVal == 1)
                    return directions;
                else if (rndVal == 0)
                {
                    directions.RemoveAt(1);
                    return directions;
                }
                else
                {
                    return new List<Direction>();
                }
            }
            else
                Console.WriteLine("DetectWillAddDirection have an error !");
            return new List<Direction>();

        }
        private void signToMap(List<Point> Points)
        {
            foreach (var point in Points)
            {
                map[point.X, point.Y] = 1;
            }
        }
        private void PassListToArray(List<Point> points, Point[] array)
        {
            array = new Point[points.Count];
            int i = 0;
            foreach (var point in points)
            {
                array[i++] = point;
            }
        }
        private int GetRndHorizontal(Point headPoint, bool forMain)
        {
            int lenght = GetRandomHorizantalValue();
            int checkValue = mapLenght.Width - 1;
            if (!forMain)
                checkValue = mapLenght.Width - 2;
            while (lenght > -1 && checkValue < headPoint.X + lenght)
            {
                lenght--;
            }
            return lenght;
        }

        private Point AddVerticalPoint(Point point, List<Point> WayPoints, int lenght, bool goUp)
        {
            for (int i = 1; i <= lenght; i++)
            {
                if (goUp)
                    WayPoints.Add(new Point(point.X, point.Y - i));
                else
                    WayPoints.Add(new Point(point.X, point.Y + i));
            }
            return WayPoints.Last<Point>();
        }
        private Point AddHorizantalPoint(Point point, List<Point> WayPoints, int lenght)
        {
            for (int i = 1; i <= lenght; i++)
            {
                WayPoints.Add(new Point(point.X + i, point.Y));
            }
            return WayPoints.Last<Point>();   // return last point. because I will add pivot points;
        }
        private int GetRndVertical(Point headPoint, bool goUp, bool mustAdd)
        {
            if (headPoint.X == mapLenght.Width - 1)
            {
                return -1;
            }
            int lenght = GetRandomVerticalValue();
            if (goUp)
            {
                while (lenght > -1 && 0 >= headPoint.Y - lenght)
                {
                    lenght--;
                }
                if (lenght == 0 && mustAdd)
                {
                    goUp = !goUp;
                    GetRndVertical(headPoint, goUp, mustAdd);
                }
                return lenght;
            }
            else
            {
                while (lenght > -1 && mapLenght.Height <= headPoint.Y + lenght)
                {
                    lenght--;
                }
                if (lenght == 0 && mustAdd)
                {
                    goUp = !goUp;
                    GetRndVertical(headPoint, goUp, mustAdd);
                }
                return lenght;
            }

        }
        private int GetRandomVerticalValue()
        {
            return random.Next(minVertical, maxVertical);
        }
        private int GetRandomHorizantalValue()
        {
            return random.Next(minHorizantal, maxHorizantal);
        }
        private int GetRandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
        private Point[] GetStartArray()
        {
            Point[] startArray = new Point[mapLenght.Height];
            for (int i = 0; i < mapLenght.Height; i++)
            {
                Point point = new Point(mapLenght.Width - 1, i);
                startArray[i] = point;
            }

            return startArray;
        }
        private Point[] GetStopArray()
        {
            Point[] stopArray = new Point[mapLenght.Height];
            for (int i = 0; i < mapLenght.Height; i++)
            {
                Point point = new Point(0, i);
                stopArray[i] = point;
            }
            return stopArray;
        }
        private Point GetStartPoint()
        {
            int val = random.Next(mapLenght.Height);
            return new Point(0, val);
        }

    }
}
