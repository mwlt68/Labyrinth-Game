using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MultiPlayer : MonoBehaviour
{
    #region Private Fields

    private bool IsLogin = false;

    private string PlayerID = null;

    private Timer Timer;

    private NetworkManager NetworkManager;

    private MessageManager MessageManager;

    #endregion

    #region Events

    /// <summary>
    /// Finded a rival and game start
    /// </summary>
    public event On OnGameStart;

    /// <summary>
    /// You can use this for anything 
    /// </summary>
    public event On<string> OnSpecialMessage;

    /// <summary>
    /// Chat messaging
    /// </summary>
    public event On<string> OnChatMessage;

    /// <summary>
    /// if rival has a network problem or leave the game
    /// </summary>
    public event On OnRivalLeave;

    /// <summary>
    /// Rival won
    /// </summary>
    public event On OnRivalWon;

    /// <summary>
    /// if rival want to play again 
    /// </summary>
    public event On OnRivalWantPlayAgain;

    /// <summary>
    /// On Game ended
    /// </summary>
    public event On OnGameEnded;

    #endregion

    #region Public Methods

    /// <summary>
    /// Start game
    /// </summary>
    public void StartGame()
    {
        Logger.i("StartGame()");
        Match((success) => { });
    }

    /// <summary>
    /// if you want to send a special message to rival
    /// Listen 
    /// </summary>
    /// <param name="message"></param>
    public void SendSpecialMessage(string message)
    {
        MessageManager.SendMessage(MessageManager.SPECIAL, message);
    }

    /// <summary>
    /// if player send a chat message to rival
    /// </summary>
    /// <param name="message"></param>
    public void SendChatMessage(string message)
    {
        MessageManager.SendMessage(MessageManager.CHAT, message);
    }

    /// <summary>
    /// You must call in the game end and if player want to play against to current rival.
    /// </summary>
    public void SendPlayAgain()
    {
        MessageManager.SendMessage(MessageManager.GAME_CONROL, MessageManager.RIVALPLAYAGAIN);
    }

    /// <summary>
    /// You must call this method when this player win.
    /// See OnRivalWon
    /// </summary>
    public void PlayerWon()
    {
        MessageManager.SendMessage(MessageManager.GAME_CONROL, MessageManager.RIVAL_WON);
    }


    /// <summary>
    /// Reset
    /// </summary>
    public void Reset()
    {
        IsLogin = false;
        PlayerID = null;
    }


    #endregion

    #region Private Methods


    // Start is called before the first frame update
    void Start()
    {
        Timer = new Timer(1.0f, Tick);
        Timer.Start();

        NetworkManager = new NetworkManager();

        MessageManager = new MessageManager(this, NetworkManager, PlayerID);
    }

    // Update is called once per frame
    void Update()
    {
        //Logger.i("update");
        Timer.Update();

        NetworkManager.Update();
    }

    /// <summary>
    /// This method is called every second.
    /// </summary>
    void Tick()
    {
        if (IsLogin && NetworkManager.Emty)
        {
            GetMessages();
        }
    }

    /// <summary>
    /// Match a rival and start game
    /// </summary>
    private void Match(Callback<bool> callback)
    {
        var request = CreateMatchRequest(callback);

        NetworkManager.AddNetworkOperation(request);
    }

    /// <summary>
    /// Create Match Post Operation
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private PostOpeation CreateMatchRequest(Callback<bool> callback)
    {
        return new PostOpeation("match", "", (data) =>
        {
            if (data == null)
            {
                callback(false);
            }
            else
            {
                callback(true);
                PlayerID = data;
                MessageManager.PlayerID = PlayerID;
                IsLogin = true;
            }
        }, this);
    }

    /// <summary>
    /// Check has a new message
    /// </summary>
    private void GetMessages()
    {
        var request = CreateGetMessagesRequest();

        NetworkManager.AddNetworkOperation(request);
    }

    /// <summary>
    /// Create GetMessage Request
    /// </summary>
    /// <returns></returns>
    private PutOpeation CreateGetMessagesRequest()
    {
        var getMessagesData = new GetMessagePostData()
        {
            playerId = PlayerID,
            localMessageCount = MessageManager.InBox.Count,
        };

        var json = JsonUtility.ToJson(getMessagesData);

        return new PutOpeation("getMessages", json, (data) =>
        {
            if (data == null)
            {
                Logger.e("An error occurs while getting messages!");
                //callback(false);
            }
            else
            {
                //callback(true);
                Logger.i("GetMessage response : " + data);
                MessageManager.ParseMessagesData(data);
            }
        }, this);
    }

    #endregion

    #region Fire Event Methods

    public void FireOnGameStart()
    {
        OnGameStart?.Invoke();
    }

    public void FireOnSpecialMessage(string message)
    {
        OnSpecialMessage?.Invoke(message);
    }

    public void FireOnChatMessage(string message)
    {
        OnChatMessage?.Invoke(message);
    }

    public void FireOnRivalLeave()
    {
        OnRivalLeave?.Invoke();
    }

    public void FireOnRivalWon()
    {
        OnRivalWon?.Invoke();
    }

    public void FireOnRivalWantPlayAgain()
    {
        OnRivalWantPlayAgain?.Invoke();
    }

    public void FireOnGameEnded()
    {
        OnGameEnded?.Invoke();
    }

    #endregion
}

public delegate void OnUsersMatched();

public delegate void On<T>(T t);

public delegate void On();
