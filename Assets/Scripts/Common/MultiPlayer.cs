using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiPlayer : MonoBehaviour
{
    public string ServerUrl = "https://safe-falls-95007.herokuapp.com/";

    public bool ConnectionOk = false;

    public bool LoginOk = false;

    public bool CheckIsMatched = false;

    public bool Matched = false;

    public string UserId = null;

    public event OnUsersMatched OnUsersMatched;

    public float tickTime = 1.0f; //1 second

    public float sumDelta = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        sumDelta += Time.deltaTime;
        if(sumDelta >= tickTime)
        {
            sumDelta = 0;
            Tick();
        }
    }

    /// <summary>
    /// This method is called every second.
    /// </summary>
    void Tick()
    {
        if (CheckIsMatched)
        {
            IsMatched(UserId, (err, res) => 
            { 
                if(err && res)
                {
                    Matched = true;
                    CheckIsMatched = false;
                    OnUsersMatched?.Invoke();
                }
            });
        }
    }

    public void CheckConnection(OnOperationResult<int> onOperationResult)
    {
        StartCoroutine(SendHttpRequest("", (err, data) =>
        {
            if(data == "ok")
            {
                ConnectionOk = true;
                onOperationResult(true, 0);
            }
            else
            {
                onOperationResult(false, 0);
            }
        }));
    }

    public void Login(OnOperationResult<string> onOperationResult)
    {
        if (ConnectionOk)
        {
            StartCoroutine(SendHttpRequest("login", (err, data) =>
            {
                if (err)
                {
                    onOperationResult(false, null);
                }
                else
                {
                    LoginOk = true;
                    onOperationResult(true, data);
                }
            }));
        }
    }

    public void Match(string userID, OnOperationResult<bool> onOperationResult)
    {
        if (LoginOk)
        {
            StartCoroutine(SendHttpRequest($"match?id={userID}", (err, data) =>
            {
                if (err)
                {
                    onOperationResult(false, false);
                }
                else
                {
                    CheckIsMatched = true;
                    onOperationResult(true, false);
                }
            }));
        }
    }

    public void IsMatched(string userID, OnOperationResult<bool> onOperationResult)
    {
        if (CheckIsMatched)
        {
            StartCoroutine(SendHttpRequest($"isMatched?id={userID}", (err, data) =>
            {
                if (err)
                {
                    onOperationResult(false, false);
                }
                else if (data == "true")
                {
                    onOperationResult(true, true);
                }
                else if (data == "false")
                {
                    onOperationResult(true, false);
                }
                else
                {
                    onOperationResult(true, false);
                }
            }));
        }
    }

    public void Reset()
    {
        ConnectionOk = false;
        LoginOk = false;
        CheckIsMatched = false;
        Matched = false;
        UserId = null;
    }

    private IEnumerator SendHttpRequest(string operation, OnDataReceived OnDataReceived)
    {
        UnityWebRequest www = UnityWebRequest.Get(ServerUrl + operation);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.url); 
            Debug.Log(www.error);
            OnDataReceived(true, www.error);
        }
        else
        {
            Debug.Log("Received : " + www.downloadHandler.text);
            yield return www.downloadHandler.text;
            OnDataReceived(false, www.downloadHandler.text);

        }
    }
}

public delegate void OnUsersMatched();

public delegate void OnDataReceived(bool err, string data);

public delegate void OnOperationResult<T>(bool done, T data);
