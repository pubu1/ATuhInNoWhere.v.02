using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escalator : MonoBehaviour
{
    public string Direction{get; set;}

    void Start(){

    }

    void Update(){
    
    }

    public Vector3 GetNextPosition(Player player)
    {
        Vector3 targetPosition = new Vector3();
        if (Direction == "Up")
        {          
            targetPosition = new Vector3(this.transform.position.x, this.transform.position.y+1, player.transform.position.z);
        } else if(Direction == "Right"){
            targetPosition = new Vector3(this.transform.position.x+1, this.transform.position.y, player.transform.position.z);
        } else if(Direction == "Down"){
            targetPosition = new Vector3(this.transform.position.x, this.transform.position.y-1, player.transform.position.z);
        } else if(Direction == "Left"){
            targetPosition = new Vector3(this.transform.position.x-1, this.transform.position.y, player.transform.position.z);
        }
       
        return targetPosition;
    }


    public bool CheckNextStep(Player player, Dictionary<Vector2,Wire> wireMap){
        if(wireMap.ContainsKey(player.TempTargetPosition) && !player.IsNotPickWire){
            return false;
        }
        else{
            return true;
        }
    }
}
