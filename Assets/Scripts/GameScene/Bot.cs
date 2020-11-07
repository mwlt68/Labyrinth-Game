using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

public class Bot : MonoBehaviour
{
    public float speed = 0.9f;
    public bool botActive;
    public static  Point botPosition;
    private bool isTargetActive;
    private float moveTime = 0;
    private float timeCounter = 0;
    private List<Point> targetWay;
    private List<List<Point>> targetWaysMain;
    private int targetIndex = 0;
    private List<Point> exitPoints;
    private List<Point> removedExitPoints;
    GameSceneInit gameSceneInit;
    GameEvent gameEvent;
    BranchAlgorithm.BranchAlgorithm br;
    void Start()
    {
        if (!botActive)
        {
            return;
        }
        gameSceneInit = this.GetComponent<GameSceneInit>();
        gameEvent = this.GetComponent<GameEvent>();
        br = gameSceneInit.br;
        botPosition = br.start;
        exitPoints = br.gameExitPoints;
        removedExitPoints = new List<Point>();
        targetWay = GetFirstTargetWays();
        targetWaysMain = GetTargetWaysList(targetWay[targetWay.Count-1]);
        isTargetActive = true;
        moveTime = speed * gameEvent.deltaTimeMax;
        
    }

   
    void Update()
    {
        if (botActive)
            MoveBot();
    }
    private void MoveBot()
    {

        timeCounter += Time.deltaTime;
        if (!gameSceneInit.isGameFinish && timeCounter >= moveTime) 
        {
            timeCounter -= moveTime;
            if (! isTargetActive)
            {
                targetWay = targetWaysMain[targetIndex++];
                isTargetActive = true;
            }
            else
            {
                botPosition = targetWay[1];
                if (gameEvent.DoesGameFinish(botPosition))
                {
                    gameSceneInit.GameFinish(false);
                    Debug.Log("Finish Bot point :X= " + botPosition.X.ToString() + "\t Y= " + botPosition.Y.ToString());
                }
                targetWay.RemoveAt(0);
                if (targetWay.Count == 2)
                {
                    Debug.Log("Now,Bot point :X= " + targetWay[0].X.ToString() + "\t Y= " + targetWay[0].Y.ToString());
                }
                if (targetWay.Count == 1)
                {
                    isTargetActive = false;
                    return;
                }

            }
        }
    }
    List<Point> GetFirstTargetWays()
    {
        List<Point> exitPointsTemp = new List<Point>(exitPoints.Count);
        PointCoppier(exitPoints, exitPointsTemp);
        Point pstart = botPosition;
        List<Point> mainWay = new List<Point>();
        mainWay.Add(pstart);
        List<List<Point>> targetWays = new List<List<Point>>();
        targetWays.Add(mainWay);
        while (0 < exitPointsTemp.Count)
        {
            GetNewWays(targetWays, exitPointsTemp);
        }
        int rnd = Random.Range(0, (targetWays.Count-1));
        RemoveExitPoint(targetWays[rnd]);
        return targetWays[rnd];

    }
    List<List<Point>> GetTargetWaysList(Point pstart)
    {
        List<Point> exitPointsTemp = new List<Point>(exitPoints.Count);
        PointCoppier(exitPoints, exitPointsTemp);
        List<Point> mainWay = new List<Point>();
        mainWay.Add(pstart);
        List<List<Point>> targetWays = new List<List<Point>>();
        targetWays.Add(mainWay);
        while (0 < exitPointsTemp.Count)
        {
            GetNewWays(targetWays, exitPointsTemp);
        }
        sortTargetWays(targetWays);
        return targetWays;
    }
    private void sortTargetWays(List<List<Point>> targetWays)
    {
        for (int i = 0; i < targetWays.Count-1; i++)
        {
            for (int j = i+1; j < targetWays.Count; j++)
            {
                if (targetWays[j].Count < targetWays[i].Count)
                {
                    List<Point> tempList = targetWays[i];
                    targetWays[i] = targetWays[j];
                    targetWays[j] = tempList;
                }
            }
        }
    }
    private void RemoveTargetWayContainRemovedExitPoints(List<List<Point>> targetWays)
    {
        foreach (var tw in targetWays.ToArray())
        {
            Point lastPoint = tw[tw.Count - 1];
            if (RemovedExitPointCheck(lastPoint))
            {
                targetWays.Remove(tw);
            }
        }
    }
    private bool RemovedExitPointCheck(Point point)
    {
        foreach (var rep in removedExitPoints)
        {
            if (isEqual(rep,point))
            {
                return true;
            }
        }
        return false;
    }
    private void RemoveExitPoint(List<Point> targetWayP)
    {
        Point last = targetWayP[targetWayP.Count - 1];
        foreach (var ep in exitPoints)
        {
            if (isEqual(ep,last))
            {
                exitPoints.Remove(ep);
                removedExitPoints.Add(ep);
                return;
            }
        }
        Debug.Log("Never exit way does not removed int RemoveExitPoint function !");
    }
    private void PointCoppier(List<Point> source, List<Point> destination)
    {
        foreach (var s in source)
        {
            destination.Add(s);
        }
    }
    private int  SellectTargetWayIndexWithMinDistance(List<List<Point>> targetWays)
    {
        int minVal=100,counter=0, minIndex = 0;
        foreach (var tw in targetWays)
        {
            if (tw.Count < minVal)
            {
                minVal = tw.Count;
                minIndex = counter;
            }
            counter++;
        }
        return minIndex;
    }
    void GetNewWays(List<List<Point>> targetWays, List<Point> exitPoints)
    {
        int twIndex = 0;
        foreach (var tw in targetWays)
        {
            Point lastPoint = tw[tw.Count - 1];
            Point lastPrePoint = new Point(-1,-1);
            if (tw.Count>1)
                lastPrePoint= tw[tw.Count - 2];
            List<Point> nearPoints = GetNearPoints(lastPoint, lastPrePoint);
            if (nearPoints.Count == 0)
            {
                twIndex++;
                continue;
            }
            else if (nearPoints.Count == 1)
            {
                if (RemovedExitPointCheck(nearPoints[0]))
                {
                    targetWays.Remove(tw);
                }
                else
                {
                    AddNewOnePoint(targetWays, exitPoints, twIndex, nearPoints[0]);
                }
                break;
            }
            else if (nearPoints.Count == 2)
            {
                AddNewTwoPoint(targetWays, twIndex, nearPoints);
                break;
            }
            else
            {
                Debug.Log("Error in GetNewWays function !");
                break;
            }
            
        }
    }
    private void AddNewOnePoint(List<List<Point>> targetWays, List<Point> exitPoints, int twIndex,Point point)
    {
        CheckPointInExitPoint(exitPoints, point);
        targetWays[twIndex].Add(point);
    }
    private void AddNewTwoPoint(List<List<Point>> targetWays, int twIndex, List<Point> point)
    {
        List<List<Point>> newGenerationWays = GetNewGenerationWays(point[0], point[1], targetWays[twIndex]);
        targetWays.RemoveAt(twIndex);
        targetWays.Add(newGenerationWays[0]);
        targetWays.Add(newGenerationWays[1]);
    }
    private List<List<Point>> GetNewGenerationWays(Point p1, Point p2,List<Point> main)
    {
        List<List<Point>> res = new List<List<Point>>();
        List<Point> p1List = new List<Point>();
        p1List.AddRange(main);
        p1List.Add(p1);
        List<Point> p2List = new List<Point>();
        p2List.AddRange(main);
        p2List.Add(p2);
        res.Add(p1List);
        res.Add(p2List);
        return res;
    }
    private void CheckPointInExitPoint(List<Point> exitPoints,Point point)
    {
        foreach (var ep in exitPoints)
        {
            if (isEqual(ep,point))
            {
                exitPoints.Remove(ep);
                return;
            }
        }
    }
    private List<Point> GetNearPoints(Point lastPoint, Point lastPrePoint)
    {
        List<Point> resPoints = GetNearWayPoints(lastPoint);
        // Last pre point is null. So,This is first point.
        if (isPointInvalid(lastPrePoint))
            return resPoints;
        else
        {
            foreach (var res in resPoints)
            {
                if (isEqual(res,lastPrePoint))
                {
                    resPoints.Remove(res);
                    return resPoints;
                }
            }
        }
        return resPoints;
    }
    private List<Point> GetNearWayPoints(Point lastPoint)
    {
        List<Point> checkPoints = new List<Point>(4);
        List<Point> resPoints = new List<Point>();
        checkPoints.Add(new Point(lastPoint.X+1,lastPoint.Y));
        checkPoints.Add(new Point(lastPoint.X-1,lastPoint.Y));
        checkPoints.Add(new Point(lastPoint.X,lastPoint.Y+1));
        checkPoints.Add(new Point(lastPoint.X,lastPoint.Y-1));
        for (int i = 0; i < 4; i++)
        {
            Point resPoint=CheckCoordinate(checkPoints[i]);
            if (!isPointInvalid(resPoint))
            {
                resPoints.Add(resPoint);
            }
        }
        return resPoints;
    }
    private Point CheckCoordinate(Point point)
    {
        if (gameSceneInit.CheckPointInLabirentMap(point) && gameSceneInit.map[point.X, point.Y] == 1)
            return point;
        else
            return new Point(-1, -1);
    }
    private bool isPointInvalid(Point p)
    {
        return isEqual(p, new Point(-1, -1));
    }
    private bool isEqual(Point p1,Point p2)
    {
        if (p1.X == p2.X && p1.Y == p2.Y)
        {
            return true;
        }
        else
            return false;
    }
}
