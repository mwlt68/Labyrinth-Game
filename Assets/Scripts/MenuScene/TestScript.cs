using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// It is only test. Move to right place later.
/// </summary>
public class TestScript : MonoBehaviour
{
    public MultiPlayer MultiPlayer;

    public Text Text;

    bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        MultiPlayer = GetComponent<MultiPlayer>();
        Text = GameObject.Find("Info").GetComponent<Text>();

        var buttonObject = GameObject.Find("MultiplayerBtn").GetComponent<Button>();
        buttonObject.onClick.AddListener(delegate
        {
            if (isStarted)
            {
                isStarted = false;

                
            }
            else
            {
                isStarted = true;

                MultiPlayer.StartGame();
                Text.text += "Game starting...";
                MultiPlayer.OnGameStart += delegate
                {
                    Text.text += "Game started";
                    MultiPlayer.SendSpecialMessage("hello");
                };

                MultiPlayer.OnSpecialMessage += (message) =>
                {
                    Text.text += message;
                };
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
