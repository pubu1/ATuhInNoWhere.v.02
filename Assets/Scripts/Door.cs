using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int ID { get; set; }

    public DoorButton Button { get; set; }

    [SerializeField]
    private string doorOpenDirection;

    [SerializeField]
    private bool isReverseDoor;

    public bool IsActive{get; set;}  

    public bool HasPipeAtDoorPosition{get; set;}

    [SerializeField]
    private float moveSpeed = 15f;
    private Vector2 previousPosition; 
    private Vector2 targetPosition; 
    private Vector2 openAxis;

    // Start is called before the first frame update
    public void Start()
    {
        if(isReverseDoor)           
            IsActive = true;
        else{
            IsActive = false;
        }

        previousPosition = this.transform.position;

        if(doorOpenDirection == "Up") targetPosition = new Vector2(previousPosition.x, previousPosition.y+1);
        else if(doorOpenDirection == "Down") targetPosition = new Vector2(previousPosition.x, previousPosition.y-1);  
        else if(doorOpenDirection == "Left") targetPosition = new Vector2(previousPosition.x-1, previousPosition.y);
        else if(doorOpenDirection == "Right") targetPosition = new Vector2(previousPosition.x+1, previousPosition.y);
    }

    // Update is called once per frame
    void Update()
    {
        if(Button.IsActive){
            openAxis = targetPosition;
            if(isReverseDoor){
                IsActive = false;
            } else{
                IsActive = true;
            }     
        } else {
            openAxis = previousPosition;
            if(isReverseDoor){
                IsActive = true;
            } else{
                IsActive = false;
            }   
        }
        DoorTransition();
    }

    void DoorTransition(){
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
}
