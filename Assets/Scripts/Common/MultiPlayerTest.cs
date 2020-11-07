using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerTest : MonoBehaviour
{

    MultiPlayer MultiPlayer;

    // Start is called before the first frame update
    void Start()
    {
        MultiPlayer = GetComponent<MultiPlayer>();
        MultiPlayer.OnUsersMatched += MultiPlayer_OnUsersMatched;

        //must be refactor :(
        MultiPlayer.CheckConnection((res1, i) =>
        {
            if (!res1)
            {
                Debug.Log("Connection Error!");
            }
            else
            {
                MultiPlayer.Login((err2, id) =>
                {
                    if (!err2)
                    {
                        Debug.Log("Login Error!");
                    }
                    else
                    {
                        MultiPlayer.Match(MultiPlayer.UserId, (err3, res) =>
                        {
                            if (!err3)
                            {
                                Debug.Log("Match Error!");
                            }
                            else
                            {
                                Debug.Log("Other Player Waiting!");
                            }
                        });
                    }
                });
            }
        });
    }

    private void MultiPlayer_OnUsersMatched()
    {
        Debug.Log("Users Matched. Let's play ... ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
