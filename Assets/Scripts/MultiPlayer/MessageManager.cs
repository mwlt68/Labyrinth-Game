using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MessageManager
{
    public static readonly string GAME_CONROL = "game control";
    public static readonly string CHAT = "chat";
    public static readonly string SPECIAL = "special";
    public static readonly string GAME_START = "game start";
    public static readonly string RIVAL_LEAVE = "rival leave";
    public static readonly string RIVAL_WON = "rival won";
    public static readonly string RIVALPLAYAGAIN = "rival play again";
    public static readonly string GAME_ENDED = "game ended";

    public List<Message> InBox { get; }

    public List<Message> Sended { get; }

    public MultiPlayer MultiPlayer;

    public NetworkManager NetworkManager;

    private string playerId;

    public MessageManager(MultiPlayer multiPlayer, NetworkManager networkManager, string playerId)
    {
        InBox = new List<Message>();

        Sended = new List<Message>();

        MultiPlayer = multiPlayer;

        NetworkManager = networkManager;

        this.playerId = playerId;
    }

    public string PlayerID
    {
        set
        {
            playerId = value;
        }
    }

    public void SendMessage(string type, string content)
    {
        var newMessage = new Message(type, content);

        Sended.Add(newMessage);

        NetworkManager.AddNetworkOperation(CreateSendMessageOperation(newMessage));
    }

    /// <summary>
    /// Parse given json data to messages
    /// </summary>
    /// <returns></returns>
    public bool ParseMessagesData(string data)
    {
        var receivedData = JsonUtility.FromJson<GetMessageResponseData>(data);

        foreach (var message in receivedData.messages)
        {
            var newMessage = new Message(message.type, message.content);

            InBox.Add(newMessage);

            HandleMessage(newMessage);
        }

        return true;
    }

    private void HandleMessage(Message message)
    {
        if (message.Type == GAME_CONROL)
        {
            HandleGameControlMessage(message);
        }
        else if (message.Type == CHAT)
        {
            HandleChatMessage(message);
        }
        else if (message.Type == SPECIAL)
        {
            HandleSpecialMessage(message);
        }
        else
        {

        }
    }

    private void HandleGameControlMessage(Message message)
    {
        var content = message.Content;

        if (content == GAME_START)
        {
            MultiPlayer.FireOnGameStart();
        }
        else if (content == RIVAL_LEAVE)
        {
            MultiPlayer.FireOnRivalLeave();
        }
        else if (content == RIVAL_WON)
        {
            MultiPlayer.FireOnRivalWon();
        }
        else if (content == RIVALPLAYAGAIN)
        {
            MultiPlayer.FireOnRivalWantPlayAgain();
        }
        else if (content == GAME_ENDED)
        {
            MultiPlayer.FireOnGameEnded();
        }
        else
        {

        }
    }

    private void HandleChatMessage(Message message)
    {

    }

    private void HandleSpecialMessage(Message message)
    {
        MultiPlayer.FireOnSpecialMessage(message.Content);
    }

    private PutOpeation CreateSendMessageOperation(Message message)
    {
        var primitiveMessage = new PrimitiveMessage(message.Type, message.Content);

        var sendMessagePostData = new SendMessagePostData()
        {
            playerId = playerId,
            messages = new PrimitiveMessage[] { primitiveMessage },
        };
        var jsonData = JsonUtility.ToJson(sendMessagePostData);

        return new PutOpeation("sendMessages", jsonData, (response)=>
        {

        }, MultiPlayer);
    }
}