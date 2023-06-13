using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Socket : MonoBehaviour
{
    public string Color { get; set; }

    public bool IsConnect { get; set; }

    void Start()
    {
        IsConnect = false;
    }

    public bool CheckSocketStartPoint(Player player)
    {
        if (this.IsConnect == false && player.IsNotPickWire)
            return true;
        else
            return false;
    }

    public bool CheckSocketEndPoint(Player player)
    {
        if (player.HandleWireColor == this.Color && !this.IsConnect)
            return true;
        else
            return false;
    }

    public void ChangePlayerAttrStartPoint(Player playerScript)
    {
            if (playerScript != null)
    {
        playerScript.IsNotPickWire = false;
        playerScript.IsAtSocket = true;
        this.IsConnect = true;
        playerScript.HandleWireColor = this.Color;
        Debug.Log("Is start point --- " + playerScript.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = playerScript.transform.Find("WholePlayerObject").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, playerScript.HandleWireColor);
        }
    }
    }


    public void ChangePlayerAttrEndPoint(Player player)
    {
        player.IsNotPickWire = true;
        player.IsAtSocket = true;
        this.IsConnect = true;
        Debug.Log("Is end point --- " + player.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = player.transform.Find("WholePlayerObject").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
        }
        //activatePipeEffect = true;
        //gameManager.GetComponent<GameManager>().Score++;
    }
}
