using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchAlgorithm
{
    public class BranchAlgorithm
    {
        public int[,] map;
        public Size mapSize;
        public Point start, finish;
        public List<Point> gameExitPoints;
        int branchCount;
        int branchLevel;//3
        Size mapBorder;//10,9
        private List<Pivot> pivots=new List<Pivot>();
        Random random = new Random();
        
        public BranchAlgorithm(int branchLevel,Size mapBorder, int branchCount = 2)
        {
            this.branchCount = branchCount;
            this.branchLevel = branchLevel;
            this.mapBorder = mapBorder;
            MapCreater();
        }
        public void MapCreater()
        {
            MapSizeCalculator();
            map=FillArrayFreeValue( );
            CreateMainPivots();
            List<Point> mapPoints=PivotsBinder();
            start = mapPoints.ElementAt(0);
            finish = GetRandomFinishPoint();
            PivotSingToMap(mapPoints);
        }
        private void MapSizeCalculator()
        {
            int width = branchLevel * mapBorder.Width;
            int powVal = (int)Math.Pow(branchCount, branchLevel);
            int height =(mapBorder.Height*(powVal-1))+powVal;
            mapSize = new Size(width,height);
        }
        private int[,] FillArrayFreeValue()
        {
            int[,] array = new int[mapSize.Width, mapSize.Height];
            for (int i = 0; i < mapSize.Width; i++)
            {
                for (int j = 0; j < mapSize.Height; j++)
                {
                    array[i, j] = 0;
                }
            }
            return array;
        }
        private Point GetRandomFinishPoint()
        {
            gameExitPoints=GetPointsOfLevel(branchLevel);
            int rnd = random.Next(gameExitPoints.Count);
            return gameExitPoints.ElementAt(rnd);
        }
        private void CreateMainPivots()
        {
            int tempHeight = mapBorder.Height;
            Point rightTopOfArray= new Point(mapSize.Width - 1, 0);
            Point tempPoint = rightTopOfArray;
            for (int i = branchLevel; i >= 0; i--)
            {
                int powVal = (int)Math.Pow(branchCount, i);
                if (i == 0)
                    tempPoint.X += 1;
                for (int j = 0; j < powVal; j++)
                {
                    Point point = new Point(tempPoint.X, tempPoint.Y);
                    Pivot pivot = new Pivot(i, point);
                    pivots.Insert(0,pivot);
                    tempPoint.Y += (tempHeight + 1);
                }
                tempPoint.X -= mapBorder.Width;
                tempPoint.Y = rightTopOfArray.Y + (tempHeight / 2);
                rightTopOfArray = tempPoint;
                tempHeight = (2 * tempHeight) + 1;
            }
        }
        private void PivotSingToMap(List<Point> mapPoints)
        {
            foreach (var item in mapPoints)
            {
                map[item.X, item.Y] = 1;
            }
        }
        private List<Point> PivotsBinder()
        {
            List<Point> mapPoints = new List<Point>();
            for (int i = 0; i < branchLevel; i++)
            {
                List<Point> mainPoints = GetPointsOfLevel(i);
                List<Point> childPoints = GetPointsOfLevel(i+1);
                mapPoints.AddRange(mainPoints);
                BindMainPointsToChildPoints(mapPoints,mainPoints, childPoints);
                if (i+1==branchLevel)
                    mainPoints.AddRange(childPoints);
            }
            return mapPoints;
        }
        private void BindMainPointsToChildPoints(List<Point> mapPoints,List<Point> mainPoints, List<Point> childPoints)
        {
            for (int i = 0; i < mainPoints.Count; i++)
            {
                Point main = mainPoints[i];
                for (int j = 0; j < branchCount; j++)
                {
                    int value = (i * branchCount) + j;
                    Point child = childPoints[value];
                    mapPoints.AddRange(DetectBinderPoints(main, child));
                }
            }
        }
        private List<Point> DetectBinderPoints(Point main,Point child)
        {
            List<Point> points = new List<Point>();
            Point point = main;
            while (child.Y < point.Y)
            {
                point = new Point(point.X, point.Y - 1);
                points.Add(point);
            }
            while (child.Y> point.Y)
            {
                point = new Point(point.X, point.Y + 1);
                points.Add(point);
            }
            while (child.Y== point.Y && point.X < child.X)
            {
                point = new Point(point.X+1, point.Y);
                points.Add(point);
            }
            return points;
        }
        private List<Point> GetPointsOfLevel(int level)
        {
            List<Point> points = new List<Point>();
            foreach (var item in pivots)
            {
                if (item.level > level)
                    break;
                if (item.level==level)
                {
                    points.Add(item.point);
                }
            }
            return points;
        }
    }
    class Pivot
    {
        public int level;
        public Point point;

        public Pivot(int level, Point point)
        {
            this.level = level;
            this.point = point;
        }
    }
}
