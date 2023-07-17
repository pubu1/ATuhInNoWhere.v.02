using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionOut : MonoBehaviour
{
    public DimensionIn BaseDimension { get; set; }
    public string OutDirection { get; set; }

    private static float[] dimensionOutRotation = { 0f, 90.0f, 180.0f, 270.0f };

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

    public bool CheckNextStep(Player player, GameObject nextStepObject, Dictionary<Vector2,bool> wireMap){
        bool totalCheck = true;
        if(wireMap.ContainsKey(GetNextPosition(player)) && !player.IsNotPickWire){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Socket" && !player.IsNotPickWire 
        && nextStepObject.GetComponent<Socket>().Color != player.HandleWireColor
        && nextStepObject.GetComponent<Socket>().IsConnect == false){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Bridge" && !nextStepObject.GetComponent<Bridge>().CheckNextStep(player)){
            totalCheck = false;
        }
        else if(nextStepObject.tag == "Wall"){
            totalCheck = false;
        }

        return totalCheck;
    }

    public void RenderSprite(string direction){
        if(direction == "Left"){
            transform.Rotate(0f, 0f, dimensionOutRotation[0]);
        } else if(direction == "Bottom"){
            transform.Rotate(0f, 0f, dimensionOutRotation[1]);
        } else if(direction == "Right"){
            transform.Rotate(0f, 0f, dimensionOutRotation[2]);
        } else if(direction == "Top"){
            transform.Rotate(0f, 0f, dimensionOutRotation[3]);
        }
    }
}
