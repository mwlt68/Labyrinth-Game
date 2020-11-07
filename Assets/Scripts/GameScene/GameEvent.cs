using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GameEvent : MonoBehaviour
{
    public int autoMoveCount = 3;
    public float deltaTimeMax=0.125f;
    MobileProcess mobileProcess;
    GameSceneInit gameSceneInit;
    private List<DirectionType> lastDirections;
    private float sumOfDeltaTime;
    private DirectionType autoDriveDir;
    
    public enum DirectionType{
        up,
        down,
        left,
        right,
        idle
    }
    void Start()
    {
        gameSceneInit = this.GetComponent<GameSceneInit>();
        mobileProcess = this.gameObject.GetComponent<MobileProcess>();
        lastDirections = new List<DirectionType>(autoMoveCount);
        sumOfDeltaTime = 0;
        deltaTimeMax = GetDeltaTime();
        autoDriveDir = DirectionType.idle;
        SetGamePlatform();
    }
    void Update()
    {
        CheckEvent();
    }
    private float GetDeltaTime()
    {
        float minTime = 0.075f;
        float sensitivity = (100 - GameMain.optionData.sensitivityLevel);
        float res =minTime + 3*(sensitivity/ 1000);
        Debug.Log(res);
        return res;
    }
    private void CheckEvent()
    {
        switch (GameSceneInit.gamePlatform)
        {
            case GameSceneInit.GamePlatform.Mobile:
                CheckMouseEventWitoutAutoDirive(mobileProcess.HandleTouch());
                break;
            case GameSceneInit.GamePlatform.Desktop:
                CheckKeyDown();
                break;
            default:
                break;
        }
    }
    private void SetGamePlatform()
    {
#if UNITY_IOS || UNITY_ANDROID
        GameSceneInit.gamePlatform = GameSceneInit.GamePlatform.Mobile;
#else
        GameSceneInit.gamePlatform = GameSceneInit.GamePlatform.Desktop;
#endif
    }
    void CheckMouseEventWitoutAutoDirive(Vector2 way)
    {
        Point checkPoint = new Point(0, 0);
        DirectionType dType = DirectionType.idle;
        if (way.Equals(Vector2.up))
        {
            dType = CheckReverseMovement(DirectionType.up);
            checkPoint = MoveControl(dType);
        }
        else if (way.Equals(Vector2.right) )
        {
            dType = CheckReverseMovement(DirectionType.right);
            checkPoint = MoveControl(dType);
        }
        else if (way.Equals(Vector2.left))
        {
            dType = CheckReverseMovement(DirectionType.left);
            checkPoint = MoveControl(dType);
        }

        else if (way.Equals(Vector2.down) )
        {
            dType = CheckReverseMovement(DirectionType.down);
            checkPoint = MoveControl(dType);
        }
        if (IsPointSuitableForMove(checkPoint))
        {
            MoveProcess(checkPoint, dType);
        }
    }
    void CheckKeyDown()
    {
        Point checkPoint = new Point(0,0);
        DirectionType dType = DirectionType.idle;
        if (Input.GetKeyDown(KeyCode.W) && !autoDriveDir.Equals(CheckReverseMovement(DirectionType.up)))
        {
            dType = CheckReverseMovement(DirectionType.up);
            checkPoint = MoveControl(dType);
            
        }
        else if (Input.GetKeyDown(KeyCode.D) && !autoDriveDir.Equals(CheckReverseMovement(DirectionType.right)))
        {
            dType = CheckReverseMovement(DirectionType.right);
            checkPoint =MoveControl(dType);
            
        }
        else if (Input.GetKeyDown(KeyCode.A) && !autoDriveDir.Equals(CheckReverseMovement(DirectionType.left)))
        {
            dType = CheckReverseMovement(DirectionType.left);
            checkPoint = MoveControl(dType);
        }

        else if (Input.GetKeyDown(KeyCode.S) && !autoDriveDir.Equals(CheckReverseMovement(DirectionType.down)))
        {
            dType = CheckReverseMovement(DirectionType.down);
            checkPoint = MoveControl(dType);
        }
        if (IsPointSuitableForMove(checkPoint))
        {
            AddNewDirection(dType);
            autoDriveDir = CheckLastDirectionsEqual();
            MoveProcess(checkPoint, dType);
        }
        else
            CheckAutoMove();

    }
    private DirectionType CheckReverseMovement(DirectionType dType)
    {
        if (GameMain.optionData != null && GameMain.optionData.reverseMovement)
        {
            switch (dType)
            {
                case DirectionType.up:
                    return DirectionType.down;
                case DirectionType.down:
                    return DirectionType.up;
                case DirectionType.left:
                    return DirectionType.right;
                case DirectionType.right:
                    return DirectionType.left;
                case DirectionType.idle:
                    return DirectionType.idle;
                default:
                    break;
            }
        }
        
        return dType;
    }
    private void CheckAutoMove()
    {
        autoDriveDir = CheckLastDirectionsEqual();
        if (autoDriveDir != DirectionType.idle && CheckSumOfDeltaTime())
        {
            Point p = MoveControl(autoDriveDir);
            if (IsPointSuitableForMove(p))
            {
                AddNewDirection(autoDriveDir);
                MoveProcess(p, autoDriveDir);
            }
        }
    }
    private void MoveProcess(Point point, DirectionType dType,bool forMobile=false)
    {
        gameSceneInit.curPointOfPlayer = point;
        List<Point> points = gameSceneInit.GetPointsThenMap();
        gameSceneInit.PaintCubes(points, dType,GetBoolAutoDrive(autoDriveDir));
        DoesGameFinish(point);
    }

    private bool IsPointSuitableForMove(Point p)
    {
        if (p.X==0 && p.Y==0)       // that`s meaan point is not suitable for move 
        {
            return false;
        }
        return true;
    }
    private Point MoveControl(DirectionType dType)
    {
        switch (dType)
        {
            case DirectionType.up:
                    return DetectAndCheckCoordinate(new Point(1,0), dType);
            case DirectionType.down:
                    return DetectAndCheckCoordinate(new Point(-1, 0), dType);
            case DirectionType.left:
                    return DetectAndCheckCoordinate(new Point(0, -1), dType);
            case DirectionType.right:
                    return DetectAndCheckCoordinate(new Point(0, 1), dType);
            default:
                return new Point(0,0);
        }

    }
    private bool CheckSumOfDeltaTime()
    {
        sumOfDeltaTime += Time.deltaTime;
        if (sumOfDeltaTime >= deltaTimeMax)
        {
            sumOfDeltaTime = 0;
            return true;
        }
        return false;
    }
    private Point  DetectAndCheckCoordinate(Point addValue,DirectionType dType)
    {
        Point point = new Point(gameSceneInit.curPointOfPlayer.X +addValue.X, gameSceneInit.curPointOfPlayer.Y + addValue.Y);
        if (gameSceneInit.CheckPointInLabirentMap(point) && gameSceneInit.map[point.X, point.Y] == 1)
            return point;
        else
            return new Point(0, 0);
    }
    private void AddNewDirection(DirectionType dType)
    {
        if (lastDirections.Count==lastDirections.Capacity)
        {
            lastDirections.RemoveAt(0);
        }
        lastDirections.Add(dType);
    }
    private DirectionType CheckLastDirectionsEqual()
    {
        if (lastDirections.Count == lastDirections.Capacity)
        {
            DirectionType firstDirection = lastDirections[0];
            foreach (var item in lastDirections)
            {
                if (item != firstDirection)
                {
                    return DirectionType.idle;
                }
            }
            return firstDirection;
        }
        return DirectionType.idle;

    }

    private void DoesGameFinish(Point point)
    {
        if (point.X == gameSceneInit.finishPoint.X && point.Y == gameSceneInit.finishPoint.Y)
        {
            gameSceneInit.GameFinish(true);
        }
    }
    private bool GetBoolAutoDrive(DirectionType dType)
    {
        if (!dType.Equals(DirectionType.idle))
            return true;
        else
            return false;
    }
}
