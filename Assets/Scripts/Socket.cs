using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void ChangePlayerAttrStartPoint(Player player)
    {
        player.IsNotPickWire = false;
        player.IsAtSocket = true;
        this.IsConnect = true;
        player.HandleWireColor = this.Color;
        Debug.Log("Is start point --- " + player.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = player.transform.Find("ATuh").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, player.HandleWireColor);
        }
    }

    public void ChangePlayerAttrEndPoint(Player player)
    {
        player.IsNotPickWire = true;
        player.IsAtSocket = true;
        this.IsConnect = true;
        Debug.Log("Is end point --- " + player.HandleWireColor);

        // Accessing the child object by name
        Transform childTransform = player.transform.Find("ATuh").transform.Find("Body");
        if (childTransform != null)
        {
            GameObject body = childTransform.gameObject;
            body.GetComponent<ChangeColor>().ChangeSpriteColor(body, "Default");
        }
        //activatePipeEffect = true;
        //gameManager.GetComponent<GameManager>().Score++;
    }

    /*public bool CheckNextStep(Player player){
        //true: handle pipe yellow
        Debug.Log((!player.IsNotPickWire && player.HandleWireColor != this.color) +" "+ (!player.IsNotPickWire && this.IsConnect));

        if ((!player.IsNotPickWire && player.HandleWireColor != this.color) || (!player.IsNotPickWire && this.IsConnect)) 
            return false;
        else
            return true;
    }*/
}
