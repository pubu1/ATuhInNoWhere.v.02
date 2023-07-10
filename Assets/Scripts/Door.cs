using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int ID { get; set; }

    public DoorButton Button { get; set; }

    public string DoorOpenDirection { get; set; }

    public bool isReverseDoor {get; set;}

    public bool IsActive{get; set;}  

    public bool HasPipeAtDoorPosition{get; set;}

    private float moveSpeed = 5f;
    private Vector2 previousPosition; 
    private Vector2 targetPosition; 
    private Vector2 openAxis;

    // Start is called before the first frame update
    public void Start()
    {
        //DoorOpenDirection = "Down";
    }

    public void Init()
    {
        previousPosition = this.transform.position;

        if (DoorOpenDirection == "Up") targetPosition = new Vector2(previousPosition.x, previousPosition.y + 1);
        else if (DoorOpenDirection == "Down") targetPosition = new Vector2(previousPosition.x, previousPosition.y - 1);
        else if (DoorOpenDirection == "Left") targetPosition = new Vector2(previousPosition.x - 1, previousPosition.y);
        else if (DoorOpenDirection == "Right") targetPosition = new Vector2(previousPosition.x + 1, previousPosition.y);

        if (isReverseDoor)
        {
            IsActive = true;
            this.transform.position = targetPosition;

            //swap
            Vector2 tmp = previousPosition;
            previousPosition = targetPosition;
            targetPosition = tmp;
        }
        else
        {
            IsActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Button.IsActive)
        {
            openAxis = targetPosition;
            IsActive = !isReverseDoor;
/*            if (isReverseDoor)
            {
                IsActive = false;
            }
            else
            {
                IsActive = true;
            }*/
        }
        else
        {
            openAxis = previousPosition;
            IsActive = isReverseDoor;
/*            if (isReverseDoor)
            {
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }*/
        }
        DoorTransition();
    }

    public void DebugPosition()
    {
        Debug.Log(this.transform.position + " : " + previousPosition + " - " + targetPosition);
    }

    public void DoorTransition(){
        if(!HasPipeAtDoorPosition){
            this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(openAxis.x, openAxis.y, 9), moveSpeed * Time.deltaTime);            
        }       
    }

    public Vector2 GetReverseDoorLocation(){
        return new Vector2(targetPosition.x, targetPosition.y);
    }

    public bool CheckReverseDoor(){
        return isReverseDoor;
    }

    public bool CheckNextStep(Player player){
        bool totalCheck = false;

        if(this.IsActive){
            totalCheck = true;
            if (!player.IsNotPickWire) this.HasPipeAtDoorPosition = true;
        } else{
            totalCheck = false;
        }

        return totalCheck;
    }
}
