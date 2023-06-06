using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionIn : MonoBehaviour
{
    public int ID { get; set; }
    public GameObject exitTop { get; set; }
    public GameObject exitRight { get; set; }
    public GameObject exitBottom { get; set; }
    public GameObject exitLeft {get;set;}

    // [SerializeField]
    // private Camera previousBaseCam;
    // [SerializeField]
    // private Camera targetBaseCam;

    void Start()
    {

    }

    public Vector3 GetNextPosition(Player player)
    {
        Vector3 entrancePosition = new Vector3();
        if (player.TempNextKey == "Right" && exitLeft != null)
        {          
            entrancePosition = new Vector3(exitLeft.transform.position.x+1, exitLeft.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Down" && exitTop != null)
        {          
            entrancePosition = new Vector3(exitTop.transform.position.x, exitTop.transform.position.y-1, player.transform.position.z);
        }
        else if (player.TempNextKey == "Left" && exitRight != null)
        {          
            entrancePosition = new Vector3(exitRight.transform.position.x-1, exitRight.transform.position.y, player.transform.position.z);
        }
        else if (player.TempNextKey == "Up" && exitBottom != null)
        {          
            entrancePosition = new Vector3(exitBottom.transform.position.x, exitBottom.transform.position.y+1, player.transform.position.z);
        }

        return entrancePosition;
    }

    public bool CheckNextStep(Player player, GameObject nextStepObject, Dictionary<Vector2,Wire> wireMap){
        bool totalCheck = true;
        if(wireMap.ContainsKey(this.GetNextPosition(player)) && !player.IsNotPickWire){
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

    // public void SetTargetBaseCamera(){
    //     targetBaseCam.enabled = true;
    //     previousBaseCam.enabled = false;
    // }

    // public void SetPreviousBaseCamera(){
    //     previousBaseCam.enabled = true;
    //     targetBaseCam.enabled = false;
    // }

    void Update()
    {

    }
}
