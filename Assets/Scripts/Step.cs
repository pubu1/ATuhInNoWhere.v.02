using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Step : MonoBehaviour
{
    private GameObject gameManager;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject player;
    private Player playerScript;
    //[SerializeField] private GameObject body;
    private bool enableMove = true;
    private bool isPauseGame = false;
    private Vector2 entranceDimensionPosition;

    //private List<string> path;
    private List<GameObject> pipes;
    private Dictionary<Vector2, string> obstaclePosition;
    private Dictionary<Vector2, Socket> pointType;
    private Dictionary<Vector2, Bridge> bridgeType;
    private Dictionary<Vector2, Dimension> dimensionType;
    private Dictionary<Vector2, DimensionTeleporter> dimensionTeleporterType;
    private Dictionary<Vector2, DoorButton> doorButtonType;
    private Dictionary<Vector2, Door> doorType;
    private Dictionary<Vector2, WaterPool> poolType;
    private bool activatePipeEffect = false;
    private bool isStepOnPool = false;
    private string previousMove;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectsWithTag("GameController")[0];
        gameManager.GetComponent<GameManager>().Start();

        player = gameManager.GetComponent<GameManager>().GetPlayer();
        playerScript = player.GetComponent<Player>();
        
        entranceDimensionPosition = player.transform.position;
        obstaclePosition = gameManager.GetComponent<GameManager>().GetObstaclePosition();
        pointType = gameManager.GetComponent<GameManager>().GetPointType();
        bridgeType = gameManager.GetComponent<GameManager>().GetBridgeType();
        dimensionType = gameManager.GetComponent<GameManager>().GetDimensionType();
        dimensionTeleporterType = gameManager.GetComponent<GameManager>().GetDimensionTeleporterType();
        doorButtonType = gameManager.GetComponent<GameManager>().GetDoorButtonType();
        doorType = gameManager.GetComponent<GameManager>().GetDoorType();
        poolType = gameManager.GetComponent<GameManager>().GetPoolType();
        //path = gameManager.GetComponent<GameManager>().GetPath();
        pipes = new List<GameObject>();

        playerScript.HandleWireColor = "Default";
        previousMove = "";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseGame = !isPauseGame;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && enableMove && !isPauseGame)
        {
            playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
            playerScript.TempNextKey = "Up";
            if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
            {
                // if ((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                // || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                //     playerScript.CurrentPosition = playerScript.TempCurrentPosition;

                SetPreviousMove("Up");
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && enableMove && !isPauseGame)
        {
            playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
            playerScript.TempNextKey = "Down";
            if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
            {

                //CheckPipeEffect();
                SetPreviousMove("Down");
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && enableMove && !isPauseGame)
        {
            playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
            playerScript.TempNextKey = "Left";
            if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
            {
                //CheckPipeEffect();
                SetPreviousMove("Left");
            }
            this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && enableMove && !isPauseGame)
        {
            playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
            playerScript.TempNextKey = "Right";
            if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
            {
                //CheckPipeEffect();
                SetPreviousMove("Right");
            }
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        /*else if (isStepOnPool && enableMove)
        {
            moveSpeed = 25f;
            playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            if(GetPreviousMove() == "Left"){
                playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
            }else if(GetPreviousMove() == "Right"){
                playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
            }else if(GetPreviousMove() == "Up"){
                playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
            }else if(GetPreviousMove() == "Down"){
                playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
            }
                
            if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, GetPreviousMove()))
            {
                playerScript.CurrentPosition = this.transform.position;
                if((obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "Dimension")
                || (obstaclePosition.ContainsKey(entranceDimensionPosition) && obstaclePosition[entranceDimensionPosition] == "DimensionTeleporter"))
                    playerScript.CurrentPosition = playerScript.TempCurrentPosition;
                
                playerScript.TargetPosition = playerScript.TempTargetPosition;
                if (!playerScript.IsNotPickWire) GeneratePipe(GetPreviousMove(), playerScript.CurrentPosition, playerScript.TargetPosition);
                CheckSocketEndPoint(playerScript.TargetPosition);
                if (playerScript.IsNotPickWire && playerScript.IsAtSocket) GeneratePipe(GetPreviousMove(), playerScript.CurrentPosition, playerScript.TargetPosition);
                CheckSocketStartPoint(playerScript.TargetPosition);
                //CheckPipeEffect();
                SetPreviousMove(GetPreviousMove());
            }
        }*/

        StepMove();
    }

    private void UpdateLocation(){
        playerScript.CurrentPosition = this.transform.position;
        playerScript.TargetPosition = playerScript.TempTargetPosition;
    }

    private void GenerateWire(){      
        if (bridgeType.ContainsKey(playerScript.CurrentPosition) && !playerScript.IsNotPickWire){
            Wire w = new Wire();
            w.Start();
            w.wireZAxis = bridgeType[playerScript.CurrentPosition].GetZAxisWire(previousMove);
            w.GenerateWire(playerScript, previousMove);
        }     
        else if (!playerScript.IsNotPickWire || playerScript.IsAtSocket){
            Wire w = new Wire();
            w.Start();
            w.GenerateWire(playerScript, previousMove);
        }      
    }

    /*private void CheckPipeEffect(){
        if(activatePipeEffect){
            List<GameObject> pipeObjects = pipes.Where(p => p.name == "Pipe" + playerScript.HandleWireColor).ToList();

            foreach(GameObject pipe in pipeObjects){
                pipe.GetComponent<ChangeColor>().StartPipeEffect(pipe, playerScript.HandleWireColor);
            }

            activatePipeEffect = false;
        }
    }*/

    private bool CanStepToPosition(Vector2 currentPosition, Vector2 targetPosition, string tempNextKey)
    {      
        bool totalCheck = true;
        if (obstaclePosition.ContainsKey(currentPosition) && obstaclePosition[currentPosition] == "Bridge")
        {
            Bridge bridge = bridgeType[currentPosition];
            totalCheck = bridge.CheckCurrentStep(bridge, playerScript, GetPreviousMove());
            if(!totalCheck) return false;
        }
        else if (obstaclePosition.ContainsKey(currentPosition) && obstaclePosition[currentPosition] == "DoorButton")
        {
            DoorButton button = doorButtonType[currentPosition];
            button.IsActive = false;
            if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Wall")
                button.IsActive = true;
            else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Pipe" && !playerScript.IsNotPickWire)
                button.IsActive = true;
            else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "PipePoint" && pointType[targetPosition].IsConnect == true && !playerScript.IsNotPickWire)
                button.IsActive = true;
            else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Door" && doorType[targetPosition].IsActive == false)
                button.IsActive = true;

            if (!playerScript.IsNotPickWire)
            {
                button.HasPipeOn = true;
            }
        }

        if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Bridge")
        {
            Bridge bridge = bridgeType[targetPosition];
            totalCheck = bridge.CheckNextStep(bridge, playerScript);
            if(totalCheck){
                UpdateLocation();          
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);
                GenerateWire();
            }
                
        }
        /*else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Dimension"){
            Dimension dimension = dimensionType[targetPosition];

            if(tempNextKey == "Right" && dimension.GetTargetTeleporterList().ContainsKey("Left")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Left"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire){
                    StopStepOnPool();
                    return false;
                }                 
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                    }
                }

                playerScript.TempCurrentPosition = player.transform.position;                
                playerScript.TempTargetPosition = entranceTeleporterPosition;
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = playerScript.TempTargetPosition;
                
                if(!playerScript.IsNotPickWire) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x-4,entranceTeleporterPosition.y);
                    RenderPipe(ladder, 0, 0);
                }                
            }
            else if(tempNextKey == "Left" && dimension.GetTargetTeleporterList().ContainsKey("Right")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Right"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire){
                    StopStepOnPool();
                    return false;
                }
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                    }
                }

                playerScript.TempCurrentPosition = player.transform.position;
                playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Right"];
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = playerScript.TempTargetPosition; 
                
                if(!playerScript.IsNotPickWire) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x+4,entranceTeleporterPosition.y);
                    RenderPipe(ladder, 0, 0);
                }   
            }
            else if(tempNextKey == "Up" && dimension.GetTargetTeleporterList().ContainsKey("Bottom")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Bottom"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                }
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                    }
                }

                playerScript.TempCurrentPosition = player.transform.position;
                playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Bottom"];
                entranceDimensionPosition = dimension.transform.position;
                player.transform.position = playerScript.TempTargetPosition;
                
                if(!playerScript.IsNotPickWire) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x,entranceTeleporterPosition.y-4);
                    RenderPipe(ladder, 0, 1);
                }  
            }
            else if(tempNextKey == "Down" && dimension.GetTargetTeleporterList().ContainsKey("Top")){
                Vector2 entranceTeleporterPosition = dimension.GetTargetTeleporterList()["Top"];
                if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "Pipe" && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                }
                else if(obstaclePosition.ContainsKey(entranceTeleporterPosition) && obstaclePosition[entranceTeleporterPosition] == "PipePoint"){
                    if (pointType.ContainsKey(entranceTeleporterPosition) && playerScript.HandleWireColor != pointType[entranceTeleporterPosition].GetColorType() && !playerScript.IsNotPickWire){
                        StopStepOnPool();
                        return false;
                    }
                }

                playerScript.TempCurrentPosition = player.transform.position;               
                playerScript.TempTargetPosition = dimension.GetTargetTeleporterList()["Top"];  
                entranceDimensionPosition = dimension.transform.position;              
                player.transform.position = playerScript.TempTargetPosition; 
                
                if(!playerScript.IsNotPickWire) {
                    Vector2 ladder = new Vector2(entranceTeleporterPosition.x,entranceTeleporterPosition.y+4);
                    RenderPipe(ladder, 0, 1);
                } 
            }else{
                return false;
            }

            dimension.SetTargetBaseCamera();    
        }*/
        /*else if(obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "DimensionTeleporter"){
            DimensionTeleporter dimensionTeleporter = dimensionTeleporterType[targetPosition];
            Dimension dimension = dimensionType[dimensionTeleporter.getBaseDimension()];
            Vector2 previousBaseDimensionEntrance = dimension.GetPreviousTeleporterList()[targetPosition];

            if(obstaclePosition.ContainsKey(previousBaseDimensionEntrance) && obstaclePosition[previousBaseDimensionEntrance] == "Pipe" && !playerScript.IsNotPickWire){
                StopStepOnPool();
                return false;
            }
            else if(obstaclePosition.ContainsKey(previousBaseDimensionEntrance) && obstaclePosition[previousBaseDimensionEntrance] == "PipePoint"){
                if (pointType.ContainsKey(previousBaseDimensionEntrance) && playerScript.HandleWireColor != pointType[previousBaseDimensionEntrance].GetColorType() && !playerScript.IsNotPickWire){
                    StopStepOnPool();
                    return false;
                }
            }

            playerScript.TempTargetPosition = previousBaseDimensionEntrance;
            entranceDimensionPosition = dimensionTeleporter.transform.position;
            player.transform.position = playerScript.TempTargetPosition;
            dimension.SetPreviousBaseCamera();   

            
            if(!playerScript.IsNotPickWire && (tempNextKey == "Right" || tempNextKey == "Left")) {
                Vector2 ladder = new Vector2(dimensionTeleporter.transform.position.x,dimensionTeleporter.transform.position.y);
                RenderPipe(ladder, 0, 0);
            } else if(!playerScript.IsNotPickWire && (tempNextKey == "Up" || tempNextKey == "Down")) {
                Vector2 ladder = new Vector2(dimensionTeleporter.transform.position.x,dimensionTeleporter.transform.position.y);
                RenderPipe(ladder, 0, 1);
            }    
        }*/
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "DoorButton")
        {
            DoorButton button = doorButtonType[targetPosition];
            button.IsActive = true;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Pool"
        || (poolType.ContainsKey(targetPosition) && playerScript.IsNotPickWire)
        )
        {
            isStepOnPool = true;
            return true;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "PipePoint")
        {
            totalCheck = false;
            Socket socket = pointType[targetPosition];
            
            if(playerScript.IsNotPickWire && socket.IsConnect){
                totalCheck = true;
                UpdateLocation();
            } 
            else if(socket.CheckSocketEndPoint(playerScript)){
                totalCheck = true;
                UpdateLocation();
                GenerateWire();     
                socket.ChangePlayerAttrEndPoint(playerScript);
                GenerateWire();               
            }  
            else if(socket.CheckSocketStartPoint(playerScript)){
                totalCheck = true;
                socket.ChangePlayerAttrStartPoint(playerScript);
                UpdateLocation();               
            }         
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Pipe")
        {
            if (!playerScript.IsNotPickWire) totalCheck = false;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Wall")
        {
            totalCheck = false;
        }
        else if (obstaclePosition.ContainsKey(targetPosition) && obstaclePosition[targetPosition] == "Door")
        {
            Door door = doorType[targetPosition];
            if (!door.IsActive)
            {
                StopStepOnPool();
                return false;
            }
            else if (!playerScript.IsNotPickWire)
            {
                door.HasPipeAtDoorPosition = true;
            }
        }
        else{
            if(totalCheck){
                UpdateLocation();
                GenerateWire();
            }
        }


        if (!totalCheck) StopStepOnPool();
        return totalCheck;
    }

    void StepMove()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerScript.TargetPosition.x, playerScript.TargetPosition.y, playerScript.DefaultZAxis), moveSpeed * Time.deltaTime);
        if (new Vector2(this.transform.position.x, this.transform.position.y) != playerScript.TargetPosition)
        {
            enableMove = false;
        }
        else
        {
            enableMove = true;
        }
    }

    private string GetPreviousMove()
    {
        return previousMove;
    }

    private void SetPreviousMove(string move)
    {
        previousMove = move;
    }

    private void StopStepOnPool()
    {
        isStepOnPool = false;
        moveSpeed = 5f;
    }
}
