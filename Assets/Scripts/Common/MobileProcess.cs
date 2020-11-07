using UnityEngine;

public class MobileProcess: MonoBehaviour
{
    private Vector2 firstTouchPosition;

    private bool isBegan = false;

    private Vector2 deltaPosition;
    private float MinTouchMove;
    
    void Start()
    {
        InitAssing();
    }
    
    void Update()
    {
        
    }
    private void InitAssing()
    {
        if (GameMain.optionData != null)
            MinTouchMove = GameMain.optionData.sensitivityLevel;
        else
            MinTouchMove = 50;
    }
    public Vector2 HandleTouch()
    {
        Vector2 result = new Vector2(0,0);
        if (Input.touchCount > 0)
        {
            var touch = Input.touches[0];

            if (touch.phase == TouchPhase.Began)
            {
                firstTouchPosition = touch.position;
                isBegan = true;
            }
            else if (touch.phase == TouchPhase.Moved && isBegan)
            {
                deltaPosition += touch.deltaPosition;

                var absX = Mathf.Abs(deltaPosition.x);

                var absY = Mathf.Abs(deltaPosition.y);

                if (Mathf.Max(absX, absY) > MinTouchMove)
                {
                    result = FindDirection(deltaPosition);
                    deltaPosition = Vector2.zero;
                    return result;
                    
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isBegan = false;
            }
        }
        return result;
    }

    private Vector2 FindDirection(Vector2 delta)
    {
        var absX = Mathf.Abs(delta.x);

        var absY = Mathf.Abs(delta.y);

        if (absX > absY)
        {
            if(delta.x < 0)
            {
                return Vector2.left;
            }
            else
            {
                return Vector2.right;
            }
        }
        else
        {
            if (delta.y < 0)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.up;
            }
        }
    }
    
}
