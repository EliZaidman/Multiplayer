using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    #region Protocol
    /*
     * This is a proposal only
     Message = playerID [space] positionID

     int that represents position of the player's step.
     [0][1][2]
     [3][4][5]
     [6][7][8]
     */
    #endregion

    public GameObject[] Xs;
    public GameObject[] Os;
    public bool playerOne = false;
    public bool playerTwo = false;
    [HideInInspector]
    public string Pside = "";

    public NetworkManager networkManager; //don't forget to drag in inspector

    public void GotNetworkMessage(string message)
    {
        Debug.Log("got network message: " + message);

        string catchNumbers = new string(message.Where(Char.IsDigit).ToArray());

        if (int.Parse(message) <= 8)
        {
            Xs[int.Parse(message)].SetActive(true);
        }
        else
        {
            Os[int.Parse(message) - 10].SetActive(true);

        }
    }

    public void PositionClicked(int position)
    {
        //draw the shape on the UI

        //update the other player about the shape
        networkManager.SendMessage("");// your job to finish it
    }

    //for debug purpouses only
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            networkManager.SendMessage("0");
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            networkManager.SendMessage("1");
        }
    }

    public void clickedOn(string squareID)
    {
        networkManager.SendMessage(squareID);
        if (playerOne)
        {
            Xs[int.Parse(squareID)].SetActive(true);
        }
        else
        {
            Os[int.Parse(squareID) - 10 ].SetActive(true);
        }
    }
}
