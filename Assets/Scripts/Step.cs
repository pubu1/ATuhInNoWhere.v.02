using Photon.Pun;
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
    private string previousMove;
    private bool activatePipeEffect = false;
    private bool isStepOnIce = false;

    int currentMap;
    int xCurrent;
    int yCurrent;
    int xTarget;
    int yTarget;
    [SerializeField]
    private PhotonView view;

    private int photonViewID;

    // Start is called before the first frame update
    void Start()
    {
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Debug.Log("Photon View ID: " + photonViewID);
        if (view.IsMine)
        {
            if (photonViewID == 1) player = gameManager.PlayerM;
            else player = gameManager.PlayerF;

            if (gameManager.PlayerF == null) gameManager.PlayerF = player;
            else gameManager.PlayerM = player;

            Debug.Log(player);
            playerScript = player.GetComponent<Player>();
            playerScript.HandleWireColor = "Default";
            previousMove = "";

            currentMap = (int)playerScript.CurrentPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);

        }
        // if (view.IsMine)
        // {
        //     if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
        //     {
        //         for (int i = 0; i < gameManager.MapGridList.Count; ++i)
        //         {
        //             foreach (GameObject item in gameManager.MapGridList[i])
        //             {
        //                 if (item.tag == "Player" && item.name == "PlayerM(Clone)")
        //                 {
        //                     if (this.gameObject.transform.position == item.transform.position)
        //                     {
        //                         player = item;
        //                         break;
        //                     }
        //                 }
        //             }
        //         }

        //         if (player != null)
        //         {
        //             playerScript = player.GetComponent<Player>();
        //             playerScript.HandleWireColor = "Default";
        //             previousMove = "";

        //             currentMap = (int)playerScript.CurrentPosition.x / 100;
        //             xCurrent = (int)(playerScript.CurrentPosition.x % 100);
        //             yCurrent = (int)(playerScript.CurrentPosition.y);
        //             xTarget = (int)(playerScript.TargetPosition.x % 100);
        //             yTarget = (int)(playerScript.TargetPosition.y);

        //             photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        //         }
        //     }
        //     else
        //     {
        //         for (int i = 0; i < gameManager.MapGridList.Count; ++i)
        //         {
        //             foreach (GameObject item in gameManager.MapGridList[i])
        //             {
        //                 if (item.tag == "Player" && item.name == "PlayerF(Clone)")
        //                 {
        //                     if (this.gameObject.transform.position == item.transform.position)
        //                     {
        //                         player = item;
        //                         break;
        //                     }
        //                 }
        //             }
        //         }
        //         if (player != null)
        //         {
        //             playerScript = player.GetComponent<Player>();
        //             playerScript.HandleWireColor = "Default";
        //             previousMove = "";

        //             currentMap = (int)playerScript.CurrentPosition.x / 100;
        //             xCurrent = (int)(playerScript.CurrentPosition.x % 100);
        //             yCurrent = (int)(playerScript.CurrentPosition.y);
        //             xTarget = (int)(playerScript.TargetPosition.x % 100);
        //             yTarget = (int)(playerScript.TargetPosition.y);

        //             photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        //         }
        //     }
        // }
    }

    //check if 2 player get the same function
    [PunRPC]
    private void LogMove()
    {
        Debug.Log(photonViewID + " move!");
    }

    [PunRPC]
    void CallChangePlayerAttrStartPoint(int currentMap, int xTarget, int yTarget, int photonTargetID)
    {
        // Debug.Log("map grid: " + gameManager.PlayGridList);
        // Socket socket = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        // //socket.ChangePlayerAttrStartPoint(playerScript);
        // Debug.Log("Socket found: " + socket + " - " + socket.transform.position);
        ChangePlayerAttrStartPoint(currentMap, xTarget, yTarget, photonTargetID);
    }

    private void ChangePlayerAttrStartPoint(int currentMap, int xTarget, int yTarget, int photonTargetID)
    {
        Socket socket = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        Debug.Log("Socket found: " + socket);
        GameObject targetP = (photonTargetID == 1) ? gameManager.PlayerM : gameManager.PlayerF;
        socket.ChangePlayerAttrStartPoint(targetP.GetComponent<Player>());
    }

    private void Update()
    {
        if (view.IsMine && player != null)
        {
            //check player move
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                view.RPC("LogMove", RpcTarget.Others);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPauseGame = !isPauseGame;
            }

            if (!isStepOnIce && Input.GetKeyDown(KeyCode.UpArrow) && enableMove && !isPauseGame)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
                playerScript.TempNextKey = "Up";
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    SetPreviousMove("Up");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.DownArrow) && enableMove && !isPauseGame)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
                playerScript.TempNextKey = "Down";
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    SetPreviousMove("Down");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.LeftArrow) && enableMove && !isPauseGame)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
                playerScript.TempNextKey = "Left";
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    SetPreviousMove("Left");
                }
                this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.RightArrow) && enableMove && !isPauseGame)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
                playerScript.TempNextKey = "Right";
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    SetPreviousMove("Right");
                }
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (isStepOnIce && enableMove)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                if (GetPreviousMove() == "Left")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
                }
                else if (GetPreviousMove() == "Right")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
                }
                else if (GetPreviousMove() == "Up")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
                }
                else if (GetPreviousMove() == "Down")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
                }

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, GetPreviousMove()))
                {
                    SetPreviousMove(GetPreviousMove());
                }
                else
                {
                    StopStepOnIce();
                }
            }
            StepMove();
        }
    }

    private void UpdateLocation()
    {
        playerScript.CurrentPosition = this.transform.position;
        playerScript.TargetPosition = playerScript.TempTargetPosition;

        currentMap = (int)playerScript.CurrentPosition.x / 100;
        xCurrent = (int)(playerScript.CurrentPosition.x % 100);
        yCurrent = (int)(playerScript.CurrentPosition.y);
        xTarget = (int)(playerScript.TargetPosition.x % 100);
        yTarget = (int)(playerScript.TargetPosition.y);
    }

    private void GenerateWire(int mapIndex, int xAxis, int yAxis, string type, GameObject obj)
    {
        if (type == "Bridge" && !playerScript.IsNotPickWire)
        {
            Wire w = new Wire();
            w.Start();
            w.wireZAxis = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>().GetZAxisWire(previousMove);
            w.GenerateWire(playerScript, previousMove);
        }
        else if (type == "Wire" && !playerScript.IsNotPickWire || playerScript.IsAtSocket)
        {
            if (!view.IsMine || view.IsMine)
            {
                Wire w = new Wire();
                w.Start();
                w.GenerateWire(playerScript, previousMove);


                GameObject wire = w.GetWire();
                Vector2 wirePosition = new Vector2(wire.transform.position.x, wire.transform.position.y);
                gameManager.WireMap[wirePosition] = wire.GetComponent<Wire>();
            }

        }
        else if (type == "Dimension" && !playerScript.IsNotPickWire)
        {
            int wireRotation = (playerScript.TempNextKey == "Up" || playerScript.TempNextKey == "Down") ? 1 : 0;
            Wire w = new Wire();
            w.Start();
            w.RenderWire(obj.transform.position, 0, wireRotation, playerScript.HandleWireColor);
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
        currentMap = (int)currentPosition.x / 100;
        xCurrent = (int)(currentPosition.x % 100);
        yCurrent = (int)(currentPosition.y);
        xTarget = (int)(targetPosition.x % 100);
        yTarget = (int)(targetPosition.y);

        //Debug.Log(xTarget + " ---- " + yTarget + gameManager.PlayGridList[currentMap][xTarget, yTarget].tag);     

        //check current position
        if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
        {
            Bridge bridge = gameManager.MapGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
            totalCheck = bridge.CheckCurrentStep(bridge, playerScript, GetPreviousMove());
            if (!totalCheck)
            {
                return false;
            }
        }
        else if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "DoorButton")
        {
            int tempCurrentMap = (int)targetPosition.x / 100;
            int tempXTarget = (int)(targetPosition.x % 100);
            int tempYTarget = (int)(targetPosition.y);
            DoorButton button = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<DoorButton>();
            button.CheckCurrentStep(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);
        }
        else if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Teleporter")
        {
            Teleporter teleporter = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Teleporter>();

            Vector3 tempTargetPosition = teleporter.GetNextPositionInside(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = teleporter.CheckNextStepInside(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {
                if (teleporter.IsTeleport(tempTargetPosition))
                {
                    playerScript.CurrentPosition = playerScript.transform.position;
                    playerScript.TargetPosition = tempTargetPosition;
                    playerScript.transform.position = tempTargetPosition;

                    tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                    xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                    yCurrent = (int)(playerScript.CurrentPosition.y);
                    xTarget = (int)(playerScript.TargetPosition.x % 100);
                    yTarget = (int)(playerScript.TargetPosition.y);

                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    }
                    else
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            socket.ChangePlayerAttrEndPoint(playerScript);
                            GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            gameManager.Score++;
                        }
                        else if (socket.CheckSocketStartPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrStartPoint(playerScript);    
                            view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        }
                    }
                    return true;
                }
            }
        }

        //check target posotion
        if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Bridge")
        {
            Bridge bridge = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<Bridge>();
            totalCheck = bridge.CheckNextStep(bridge, playerScript);
            if (totalCheck)
            {
                UpdateLocation();
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                }
                else
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                }

                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);

            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Socket")
        {
            totalCheck = false;
            Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();

            if (playerScript.IsNotPickWire && socket.IsConnect)
            {
                totalCheck = true;
                UpdateLocation();
            }
            else if (socket.CheckSocketEndPoint(playerScript))
            {
                totalCheck = true;
                UpdateLocation();
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                }
                else
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                }
                socket.ChangePlayerAttrEndPoint(playerScript);
                GenerateWire(currentMap, xTarget, yTarget, "Wire", null);
                gameManager.Score++;
            }
            else if (socket.CheckSocketStartPoint(playerScript))
            {
                totalCheck = true;
                //socket.ChangePlayerAttrStartPoint(playerScript);
                view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                UpdateLocation();
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Wall")
        {
            totalCheck = false;
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionIn")
        {
            DimensionIn dIn = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionIn>();

            Vector3 tempTargetPosition = dIn.GetNextPosition(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = dIn.CheckNextStep(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TargetPosition = tempTargetPosition;
                playerScript.transform.position = tempTargetPosition;

                GameObject mainCamera = GameObject.Find("Main Camera");
                mainCamera.transform.position = playerScript.TargetPosition;

                tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                yCurrent = (int)(playerScript.CurrentPosition.y);
                xTarget = (int)(playerScript.TargetPosition.x % 100);
                yTarget = (int)(playerScript.TargetPosition.y);

                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                }
                else
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                }

                if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                {
                    Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        socket.ChangePlayerAttrEndPoint(playerScript);
                        GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                        gameManager.Score++;
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrStartPoint(playerScript);
                        //view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, socket, playerScript);
                    }
                }
                GameObject dOut = dIn.GetDimensionOut(playerScript);
                GenerateWire(0, 0, 0, "Dimension", dOut);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionOut")
        {
            DimensionOut dOut = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<DimensionOut>();

            Vector3 tempTargetPosition = dOut.GetNextPosition(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = dOut.CheckNextStep(playerScript, gameManager.PlayGridList[tempCurrentMap][tempXTarget, tempYTarget], gameManager.WireMap);

            if (totalCheck)
            {
                playerScript.CurrentPosition = playerScript.transform.position;
                playerScript.TargetPosition = tempTargetPosition;
                playerScript.transform.position = tempTargetPosition;

                GameObject mainCamera = GameObject.Find("Main Camera");
                mainCamera.transform.position = playerScript.TargetPosition;

                tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                yCurrent = (int)(playerScript.CurrentPosition.y);
                xTarget = (int)(playerScript.TargetPosition.x % 100);
                yTarget = (int)(playerScript.TargetPosition.y);


                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                }
                else
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                }

                if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                {
                    Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        socket.ChangePlayerAttrEndPoint(playerScript);
                        GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                        gameManager.Score++;
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrStartPoint(playerScript);
                        //view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, socket, playerScript);
                    }
                }
                else if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Bridge")
                {
                    Bridge bridge = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Bridge>();
                    bool changePlayerDefaultZAxis = bridge.CheckNextStep(bridge, playerScript);
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);
                }

                GenerateWire(0, 0, 0, "Dimension", dOut.gameObject);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = gameManager.MapGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;

            totalCheck = button.CheckNextStep(playerScript);
            if (totalCheck)
            {
                UpdateLocation();
                GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Ice")
        {
            IcePallete icePallete = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<IcePallete>();
            totalCheck = icePallete.CheckNextStep(playerScript, gameManager.WireMap);

            if (totalCheck)
            {
                moveSpeed = 7f;
                isStepOnIce = true;
                UpdateLocation();
                GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;
            UpdateLocation();
            GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Door")
        {
            Door door = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Door>();
            totalCheck = door.CheckNextStep(playerScript);

            if (totalCheck)
            {
                UpdateLocation();
                GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Teleporter")
        {
            Teleporter teleporter = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Teleporter>();

            Vector3 tempTargetPosition = teleporter.GetNextPositionOutside(playerScript);
            int tempCurrentMap = (int)tempTargetPosition.x / 100;
            int tempXTarget = (int)(tempTargetPosition.x % 100);
            int tempYTarget = (int)(tempTargetPosition.y);

            totalCheck = teleporter.CheckNextStepOutside(playerScript, gameManager.WireMap);

            if (totalCheck)
            {
                if (teleporter.IsTeleport(tempTargetPosition))
                {
                    playerScript.CurrentPosition = playerScript.transform.position;
                    playerScript.TargetPosition = tempTargetPosition;
                    playerScript.transform.position = tempTargetPosition;

                    tempCurrentMap = (int)playerScript.TargetPosition.x / 100;
                    xCurrent = (int)(playerScript.CurrentPosition.x % 100);
                    yCurrent = (int)(playerScript.CurrentPosition.y);
                    xTarget = (int)(playerScript.TargetPosition.x % 100);
                    yTarget = (int)(playerScript.TargetPosition.y);

                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    }
                    else
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            socket.ChangePlayerAttrEndPoint(playerScript);
                            GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            gameManager.Score++;
                        }
                        else if (socket.CheckSocketStartPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrStartPoint(playerScript);
                            //view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, socket, playerScript);
                        }
                    }
                }
                else
                {
                    UpdateLocation();
                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    }
                    else
                    {
                        GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    }
                }
            }
        }
        else if (gameManager.WireMap.ContainsKey(targetPosition) && !playerScript.IsNotPickWire)
        {
            if (!playerScript.IsNotPickWire) totalCheck = false;
        }
        else
        {
            if (totalCheck)
            {
                UpdateLocation();
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                }
                else
                {
                    GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                }
            }
        }

        if (!totalCheck)
        {
            if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
            {
                Bridge bridge = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
                bridge.CheckOpacity(playerScript, GetPreviousMove());
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

    private void StopStepOnIce()
    {
        isStepOnIce = false;
        moveSpeed = 5f;
    }
}
