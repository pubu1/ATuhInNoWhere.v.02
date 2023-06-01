using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Step : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject player;
    private Player playerScript;
    //[SerializeField] private GameObject body;
    private bool enableMove = true;
    private bool isPauseGame = false;
    private Vector2 entranceDimensionPosition;
    private List<string> path;
    private List<GameObject> pipes;
    private GameObject[,] grid;  // 2D grid for storing game objects
    private static float gridCellSize = 1.0f;
    int gridWidth = 5; // specify the grid width based on your game's requirements */;
    int gridHeight = 6; //specify the grid height based on your game's requirements */;
    List<GameObject[,]> mapGridList;
    List<GameObject[,]> playGridList;

    private string previousMove;

    private Dictionary<Vector2, string> obstaclePosition;
    private Dictionary<Vector2, DimensionIn> dimensionType;
    private Dictionary<Vector2, DimensionOut> dimensionTeleporterType;
    private Dictionary<Vector2, DoorButton> doorButtonType;
    private Dictionary<Vector2, Door> doorType;
    private Dictionary<Vector2, WaterPool> poolType;
    private bool activatePipeEffect = false;
    private bool isStepOnPool = false;
    
    int currentMap;
    int xCurrent;
    int yCurrent;
    int xTarget;
    int yTarget;       

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //gameManager.GetComponent<GameManager>().Start();
        mapGridList = gameManager.MapGridList;
        playGridList = gameManager.PlayGridList;

        for (int i = 0; i < mapGridList.Count; ++i)
        {
            foreach (GameObject item in mapGridList[i])
            {
                if (item.tag == "Player")
                {
                    if (this.gameObject.transform.position == item.transform.position)
                    {
                        player = item;
                        break;
                    }
                }
            }
        }

        //player = gameManager.GetPlayer();
        playerScript = player.GetComponent<Player>();

//        entranceDimensionPosition = player.transform.position;
        // handlePipeColor = "Default";

        playerScript.HandleWireColor = "Default";
        previousMove = "";

        currentMap = (int) playerScript.CurrentPosition.x / 100;
        xCurrent = (int) (playerScript.CurrentPosition.x % 100);
        yCurrent = (int) (playerScript.CurrentPosition.y);
        xTarget = (int) (playerScript.TargetPosition.x % 100);
        yTarget = (int) (playerScript.TargetPosition.y);       

        // Create the 2D grid
        //grid = new GameObject[gridWidth, gridHeight];
    }


    private void Update()
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

    private void UpdateLocation()
    {
        playerScript.CurrentPosition = this.transform.position;
        playerScript.TargetPosition = playerScript.TempTargetPosition;

        currentMap = (int) playerScript.CurrentPosition.x / 100;
        xCurrent = (int) (playerScript.CurrentPosition.x % 100);
        yCurrent = (int) (playerScript.CurrentPosition.y);
        xTarget = (int) (playerScript.TargetPosition.x % 100);
        yTarget = (int) (playerScript.TargetPosition.y); 
    }

    private void GenerateWire(int mapIndex, int xAxis, int yAxis, string type)
    {        
        if (type == "Bridge" && !playerScript.IsNotPickWire)
        {
            Wire w = new Wire();
            w.Start();
            w.wireZAxis = playGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>().GetZAxisWire(previousMove);
            w.GenerateWire(playerScript, previousMove);
        }
        else if (type == "Wire" && !playerScript.IsNotPickWire || playerScript.IsAtSocket)
        {
            Wire w = new Wire();
            w.Start();
            w.GenerateWire(playerScript, previousMove);

            GameObject wire = w.GetWire();
            Vector2 wirePosition = new Vector2(wire.transform.position.x, wire.transform.position.y);
            gameManager.WireMap[wirePosition] = wire.GetComponent<Wire>();

            //playGridList[mapIndex][xAxis, yAxis] = wire;
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
        currentMap = (int) currentPosition.x / 100;
        xCurrent = (int) (currentPosition.x % 100);
        yCurrent = (int) (currentPosition.y);
        xTarget = (int) (targetPosition.x % 100);
        yTarget = (int) (targetPosition.y);  

        //Debug.Log(xTarget + " ---- " + yTarget + playGridList[currentMap][xTarget, yTarget].tag);     

        //check current position
        if (playGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
        {
            //Bridge bridge = bridgeType[currentPosition];
            Bridge bridge = mapGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
            totalCheck = bridge.CheckCurrentStep(bridge, playerScript, GetPreviousMove());
            if(!totalCheck){
                return false;
            }
        }

        //check target posotion
        if (playGridList[currentMap][xTarget, yTarget].tag == "Bridge")
        {
            Bridge bridge = mapGridList[currentMap][xTarget, yTarget].GetComponent<Bridge>();
            totalCheck = bridge.CheckNextStep(bridge, playerScript);
            if (totalCheck)
            {       
                UpdateLocation();
                if(playGridList[currentMap][xCurrent, yCurrent].tag == "Bridge") {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge");
                } else{
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire");
                }
                    
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);
                
            }
        }
        else if (playGridList[currentMap][xTarget, yTarget].tag == "Socket")
        {
            totalCheck = false;
            Socket socket = playGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();

            if (playerScript.IsNotPickWire && socket.IsConnect)
            {
                totalCheck = true;
                UpdateLocation();
            }
            else if (socket.CheckSocketEndPoint(playerScript))
            {
                totalCheck = true;
                UpdateLocation();
                GenerateWire(currentMap, xCurrent, yCurrent, "Wire");
                socket.ChangePlayerAttrEndPoint(playerScript);
                GenerateWire(currentMap, xTarget, yTarget, "Wire");
            }
            else if (socket.CheckSocketStartPoint(playerScript))
            {
                totalCheck = true;
                socket.ChangePlayerAttrStartPoint(playerScript);
                UpdateLocation();
            }
        }
        else if (playGridList[currentMap][xTarget, yTarget].tag == "Wall")
        {
            totalCheck = false;
        }
        else if (gameManager.WireMap.ContainsKey(targetPosition) && !playerScript.IsNotPickWire)
        {
            if (!playerScript.IsNotPickWire) totalCheck = false;
        }
        else if (playGridList[currentMap][xTarget, yTarget].tag == "DimensionIn")
        {
            DimensionIn dIn = mapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionIn>();

            Vector3 tempTargetPosition = dIn.GetNextPosition(playerScript);
            int tempCurrentMap = (int) tempTargetPosition.x / 100;
            int tempXTarget = (int) (tempTargetPosition.x % 100);
            int tempYTarget = (int) (tempTargetPosition.y);  
            
            totalCheck = dIn.CheckNextStep(playerScript, playGridList[tempCurrentMap][tempXTarget, tempYTarget]);
            
            if(totalCheck) {              
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TargetPosition = tempTargetPosition;
                playerScript.transform.position = tempTargetPosition;

                tempCurrentMap = (int) playerScript.TargetPosition.x / 100;
                xCurrent = (int) (playerScript.CurrentPosition.x % 100);
                yCurrent = (int) (playerScript.CurrentPosition.y);
                xTarget = (int) (playerScript.TargetPosition.x % 100);
                yTarget = (int) (playerScript.TargetPosition.y);

                if(playGridList[currentMap][xCurrent, yCurrent].tag == "Bridge"){                 
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge");             
                } else{
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire");    
                }

                if(playGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket"){                 
                    Socket socket = playGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        socket.ChangePlayerAttrEndPoint(playerScript);
                        GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire");
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        socket.ChangePlayerAttrStartPoint(playerScript);
                    }                              
                }                    
            }            
        }
        else if (playGridList[currentMap][xTarget, yTarget].tag == "DimensionOut")
        {            
            DimensionOut dOut = mapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionOut>();

            Vector3 tempTargetPosition = dOut.GetNextPosition(playerScript);
            int tempCurrentMap = (int) tempTargetPosition.x / 100;
            int tempXTarget = (int) (tempTargetPosition.x % 100);
            int tempYTarget = (int) (tempTargetPosition.y);  
            
            totalCheck = dOut.CheckNextStep(playerScript, playGridList[tempCurrentMap][tempXTarget, tempYTarget]);
            
            if(totalCheck) {              
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TargetPosition = tempTargetPosition;
                playerScript.transform.position = tempTargetPosition;
             
                tempCurrentMap = (int) playerScript.TargetPosition.x / 100;
                xCurrent = (int) (playerScript.CurrentPosition.x % 100);
                yCurrent = (int) (playerScript.CurrentPosition.y);
                xTarget = (int) (playerScript.TargetPosition.x % 100);
                yTarget = (int) (playerScript.TargetPosition.y);

                
                if(playGridList[currentMap][xCurrent, yCurrent].tag == "Bridge"){                 
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge");             
                } else{
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire");    
                }

                if(playGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket"){                 
                    Socket socket = playGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        socket.ChangePlayerAttrEndPoint(playerScript);
                        GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire");
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        socket.ChangePlayerAttrStartPoint(playerScript);                     
                    }                  
                } 
                else if(playGridList[tempCurrentMap][xTarget, yTarget].tag == "Bridge"){
                    Bridge bridge = playGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Bridge>();
                    bool changePlayerDefaultZAxis = bridge.CheckNextStep(bridge, playerScript);
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);
                }                  
            }
        }
        else if (playGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = mapGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;
            totalCheck = true;
            UpdateLocation();
        }
/*
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
        if (!totalCheck) StopStepOnPool();
*/
        else
        {
            if (totalCheck)
            {
                UpdateLocation();
                if(playGridList[currentMap][xCurrent, yCurrent].tag == "Bridge") {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge");
                } else{
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire");
                }
            }
        }
        return totalCheck;
    }

    void StepMove()
    {
        player.transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerScript.TargetPosition.x, playerScript.TargetPosition.y, playerScript.DefaultZAxis), moveSpeed * Time.deltaTime);
        if (new Vector2(player.transform.position.x, player.transform.position.y) != playerScript.TargetPosition)
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
