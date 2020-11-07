using Assets.Scripts;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
public class GameSceneInit : MonoBehaviour
{

    public Material defaultMaterial;
    public Material playerMaterial;
    public Material blockMaterial;
    public Material voidMaterial;
    public Material borderMaterial;
    public Material finishMaterial;

    public Material random1Color;
    public Material random2Color;
    public Material random3Color;
    public Material random4Color;
    public Material random5Color;
    public Material random6Color;

    private DataVisualizeInGame dataVisualizeInGame;

    public enum GamePlatform
    {
        Mobile,
        Desktop
    }
    public enum GameMode
    {
        Easy,
        Normal,
        Hard
    }
    public static GamePlatform gamePlatform;
    public static GameMode gameMode;
    public static bool timerActive =true;
    public GameObject directionIdleReferanceObj;
    public GameObject cubeReferance;
    public  Point curPointOfPlayer;
    public  Point finishPoint;
    public int[,] map;
    private List<Material> colors = new List<Material>(3);
    private Point squareSize;
    private LabirentAlgorithm labirentAlgorithm;
    private Size mapSize;
    Point imageSize;
    private Vector3 mapDrawStartPoint;
    private List<GameObject> cubes;
    private GameObject directionIdleObj;
    private GameObject directionChildAutoObj;
    private GameObject directionChildObj;
    private bool opposite=false;
    private int painterChangeCounter=0;
    private int painterChangeMaxVal=5;
    private int timer=0;
    private float tempTimer=0;
    void Start()
    {
        DirectionObjChildDetect();
        ColorAddList();
        GameStart();
        SetComponentsValue();
    }
    void Update()
    {
        CheckTimerTick();
        
    }
    private void CheckTimerTick()
    {
        if (timerActive)
        {
            tempTimer += Time.deltaTime;
            if (tempTimer >= 1)
            {
                tempTimer--;
                timer++;
                WriteTimeTextComponent(timer);
            }
        }
    }
    private void GameStart()
    {
        dataVisualizeInGame = this.GetComponent<DataVisualizeInGame>();
        BranchAlgorithm.BranchAlgorithm br = new BranchAlgorithm.BranchAlgorithm(4, new Size(10, 9));
        map = br.map;
        mapSize = br.mapSize;
        curPointOfPlayer = br.start;
        finishPoint = br.finish;   
        squareSize = new Point(11, 7);
        CubeGenerator(squareSize.X, squareSize.Y);
        List<Point> points = GetPointsThenMap();
        PaintCubes(points,GameEvent.DirectionType.idle,false);
    }
    public void GameFinish(bool doesWin)
    {
        GameMain.player.AddExperience(GetExperienceFromGameMode());
        dataVisualizeInGame.GameFinishComponentVisualize(doesWin, timer);
    }
    private int GetExperienceFromGameMode()
    {
        switch (gameMode)
        {
            case GameMode.Easy:
                return 1;
            case GameMode.Normal:
                return 2;
            case GameMode.Hard:
                return 3;
            default:
                return 0;
        }
    }
    private void SetComponentsValue()
    {
        if (GameMain.optionData != null)
        {
            dataVisualizeInGame.volumeSlider.value = GameMain.optionData.volumeLevel;
            WriteTimeTextComponent(0);
        }

    }
    private void WriteTimeTextComponent(int val)
    {
        if (val >= 0)
            dataVisualizeInGame.timeText.text = val.ToString();
    }
    private void ColorAddList()
    {
        colors.Add(random1Color);
        colors.Add(random2Color);
        colors.Add(random3Color);
    }
    private void DirectionObjChildDetect()
    {
        directionIdleObj = Instantiate(directionIdleReferanceObj);
        directionChildAutoObj=GetChildWithName(directionIdleObj, "directionChildAutoObj");
        directionChildObj= GetChildWithName(directionIdleObj, "directionChildAutoObj");
    }
    private GameObject GetChildWithName(GameObject obj, string name)
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);
        if (childTrans != null)
        {
            return childTrans.gameObject;
        }
        else
        {
            Debug.Log("Game object is null in GetChildWithName Func !");
            return null;
        }
    }
    private void CubeGenerator(int width, int height,float size=1)
    {
        cubes = new List<GameObject>(width * height);
        PointF cubeCreateStartPoint;
        float halfOfWidth = width / 2;
        float halfOfheight = height / 2;
        cubeCreateStartPoint.X = -1*(halfOfWidth * size); // I multiplied by minus one because start point side is left of coordinate  system
        cubeCreateStartPoint.Y = (halfOfheight * size);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector2 position = new Vector2(cubeCreateStartPoint.X + j * size, cubeCreateStartPoint.Y - i * size);
                GameObject cube = Instantiate(cubeReferance);
                cube.transform.localScale = new Vector3(size, size, size);
                cube.transform.localPosition = position;
                cubes.Add(cube);
            }
        }
    }

    public  List<Point> GetPointsThenMap()
    {
        List<Point> points = new List<Point>(squareSize.X * squareSize.Y);
        int yFront = squareSize.Y - (squareSize.Y / 2);
        int x = (squareSize.X - 1) / 2;
        for (int i = 0; i < squareSize.Y; i++)
        {
            int xTemp = -x;
            for (int j = 0; j < squareSize.X; j++)
            {
                points.Add(new Point(curPointOfPlayer.X + yFront, curPointOfPlayer.Y + xTemp));
                xTemp++;
            }
            yFront--;
        }
        return points;
    }
    public void PaintCubes(List<Point> pointList, GameEvent.DirectionType dType,bool autoDriveActive)
    {
        GameObject playerCube;
        playerCube=SetCubesMaterial(pointList);
        PaintDirectionEffect(playerCube, dType, autoDriveActive);
    }
    private GameObject SetCubesMaterial(List<Point> pointList)
    {
        int counter = 0;
        painterChangeCounter++;
        int diffVal = 0;
        opposite = !opposite;
        foreach (var item in pointList)
        {
            if (CheckPointInLabirentMap(item))
            {
                if (map[item.X, item.Y] == 1)
                {
                    if (item.X == finishPoint.X && item.Y == finishPoint.Y)
                        cubes[counter].GetComponent<Renderer>().material = playerMaterial;
                    else
                        cubes[counter].GetComponent<Renderer>().material = voidMaterial;
                }
                else
                {
                    if (gamePlatform==GamePlatform.Mobile)
                        cubes[counter].GetComponent<Renderer>().material = GetMaterial(ref diffVal, counter);
                    else
                        cubes[counter].GetComponent<Renderer>().material = GetMaterialOutsideMobile( counter);
                }
            }
            else
                cubes[counter].GetComponent<Renderer>().material = borderMaterial;

            counter++;
        }
        int myPosition = GetCurrentPointInViewPoints(pointList);
        GameObject playerCube = cubes[myPosition];
        playerCube.GetComponent<Renderer>().material = playerMaterial;
        return playerCube;
    }
    private Material GetMaterialOutsideMobile(int value)
    {
        if (opposite)
        {
            value++;
        }
        int res =value% colors.Count;
        
        return colors[res];
    }
    private Material GetMaterial(ref int diffVal, int value)
    {
        if (painterChangeMaxVal <= painterChangeCounter)
        {
            diffVal++;
            painterChangeCounter = 0;
        }
        int res = (value + diffVal) % colors.Count;

        return colors[res];
    }
    private void PaintDirectionEffect(GameObject playerCube, GameEvent.DirectionType dType, bool autoDriveActive)
    {
        directionIdleObj.transform.localPosition = playerCube.transform.localPosition;
        DirectionObjUnvisible(autoDriveActive);
        switch (dType)
        {
            case GameEvent.DirectionType.up:
                directionIdleObj.transform.localRotation = Quaternion.Euler(0,0,0);
                break;
            case GameEvent.DirectionType.down:
                directionIdleObj.transform.localRotation = Quaternion.Euler(0, 0,180);
                break;
            case GameEvent.DirectionType.left:
                directionIdleObj.transform.localRotation = Quaternion.Euler(0, 0, 90);
                break;
            case GameEvent.DirectionType.right:
                directionIdleObj.transform.localRotation = Quaternion.Euler(0, 0,270);
                break;
            case GameEvent.DirectionType.idle:
                directionChildObj.SetActive(false);
                break;
            default:
                break;
        }
    }
    private void DirectionObjUnvisible(bool autoDriveActive)
    {
        var pos3 = directionIdleObj.transform.localPosition;
        directionIdleObj.transform.localPosition = new Vector3(pos3.x, pos3.y, -1);
        directionChildObj.SetActive(true);
        if (autoDriveActive)
            directionChildAutoObj.SetActive(true);
        else
            directionChildAutoObj.SetActive(false);
    }
    private int GetCurrentPointInViewPoints(List<Point> points)
    {
        int counter = 0;
        foreach (var item in points)
        {
            if (curPointOfPlayer.X == item.X && curPointOfPlayer.Y == item.Y)
            {
                return counter;
            }
            counter++;
        }
        return counter;
    }
    public  bool CheckPointInLabirentMap(Point point)
    {
        if (point.X >= 0 && point.X < mapSize.Width &&
            point.Y >= 0 && point.Y < mapSize.Height
            )
        {
            return true;
        }
        return false;
    }

}