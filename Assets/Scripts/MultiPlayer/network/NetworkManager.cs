using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager
{
    private const int MAX_TRY = 3;

    private readonly Queue<NetworkOperation> NetworkOperations;

    private bool isWorking = false;

    public bool Emty
    {
        get
        {
            return NetworkOperations.Count
          == 0;
        }
    }

    public NetworkManager()
    {
        NetworkOperations = new Queue<NetworkOperation>();
    }

    public void AddNetworkOperation(NetworkOperation networkOperation)
    {
        NetworkOperations.Enqueue(networkOperation);

        Work();
    }

    /// <summary>
    /// Update
    /// </summary>
    public void Update()
    {
        Work();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Work()
    {
        if (isWorking || NetworkOperations.Count == 0)
            return;

        isWorking = true;

        var networkOperation = NextOperation();

        networkOperation.Perform((success) =>
        {
            if (success)
            {
                NetworkOperations.Dequeue();
            }
            else
            {
                networkOperation.TryCount++;

                Logger.w("Network error : trying " + networkOperation.TryCount.ToString());

                if (networkOperation.TryCount >= MAX_TRY)
                {
                    throw new Exception("Network error!");
                }
            }

            isWorking = false;
        });
    }

    private NetworkOperation NextOperation() => NetworkOperations.Count > 0 ? NetworkOperations.Peek() : null;
}

/// <summary>
/// 
/// </summary>
public abstract class NetworkOperation
{
    public int TryCount = 0;

    public abstract void Perform(Callback<bool> callback);
}

/// <summary>
/// 
/// </summary>
public class PostOpeation : NetworkOperation
{
    public string Route;

    public string Data;

    public Callback<string> Response;

    public bool Success = false;

    public string Error;

    public MultiPlayer MultiPlayer;

    public PostOpeation(string route, string data, Callback<string> callback, MultiPlayer multiPlayer)
    {
        Route = route;
        Data = data;
        Response = callback;
        MultiPlayer = multiPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public override void Perform(Callback<bool> callback)
    {
        var url = Config.ServerUrl + Route;

        Logger.i("Performing post request... : " + url);

        var webRequest = UnityWebRequest.Post(url, Data);

        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += delegate
        {
            if (webRequest.isNetworkError)
            {
                Logger.e("Network Error in the post. : " + webRequest.error);
                Success = false;
                callback(false);
            }
            else if (webRequest.isHttpError)
            {
                Logger.e("Http Error in the post. : " + webRequest.error);
                Success = false;
                callback(true);
            }
            else
            {
                Success = true;
                callback(true);
                Response(webRequest.downloadHandler.text);
            }
        };
    }
}


/// <summary>
/// 
/// </summary>
public class PutOpeation : NetworkOperation
{
    public string Route;

    public string Data;

    public Callback<string> Response;

    public bool Success = false;

    public string Error;

    public MultiPlayer MultiPlayer;

    public PutOpeation(string route, string data, Callback<string> callback, MultiPlayer multiPlayer)
    {
        Route = route;
        Data = data;
        Response = callback;
        MultiPlayer = multiPlayer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    public override void Perform(Callback<bool> callback)
    {
        var url = Config.ServerUrl + Route;

        Logger.i("Performing put request... : " + url);

        var webRequest = UnityWebRequest.Put(url, Data);

        webRequest.SetRequestHeader("Content-Type", "application/json");

        var req = webRequest.SendWebRequest();

        req.completed += delegate
        {
            if (webRequest.isNetworkError)
            {
                Logger.e("Network Error in the put. : " + webRequest.error);
                Success = false;
                callback(false);
            }
            else if (webRequest.isHttpError)
            {
                Logger.e("Http Error in the put. : " + webRequest.error);
                Success = false;
                callback(true);
            }
            else
            {
                Success = true;
                callback(true);
                Response(webRequest.downloadHandler.text);
            }
        };
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="t"></param>
public delegate void Callback<T>(T t);



