using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionOut : MonoBehaviour
{
    public DimensionIn BaseDimension { get; set; }
    public string OutDirection { get; set; }

    // public Vector2 getBaseDimension()
    // {
    //     return BaseDimension.transform.position;
    // }

    // public Vector2 getBaseDimensionEntrance()
    // {
    //     return BaseDimension.transform.position;
    // }

    public Vector3 GetNextPosition(Player player)
    {
        Vector3 exitPosition = new Vector3();
        if (player.TempNextKey == "Left" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x-1, BaseDimension.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Down" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y-1, player.transform.position.z);
        }
        else if (player.TempNextKey == "Right" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x+1, BaseDimension.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Up" && this.BaseDimension != null)
        {
            exitPosition = new Vector3(BaseDimension.transform.position.x, BaseDimension.transform.position.y+1, player.transform.position.z);
        }

        return exitPosition;
    }

    public bool CheckNextStep(Player player, GameObject nextStepObject){
        bool totalCheck = true;
        if(nextStepObject.tag == "Wire" && !player.IsNotPickWire){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Socket" && !player.IsNotPickWire 
        && nextStepObject.GetComponent<Socket>().Color != player.HandleWireColor
        && nextStepObject.GetComponent<Socket>().IsConnect == false){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Wall"){
            totalCheck = false;
        }

        return totalCheck;
    }
}
