using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Step : MonoBehaviourPun
{
    private static GameManager gameManager;
    private static Wire wireSpawner;

    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    private GameObject player;
    private Player playerScript, otherPlayerScript;
    private bool enableMove = true;
    private bool isPauseGame = false;
    private bool activatePipeEffect = false;
    private bool isStepOnIce = false;
    private bool isStepOnEscalator = false;

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
        wireSpawner = GameObject.Find("WireSpawner").GetComponent<Wire>();
        Debug.Log(wireSpawner + " is found!");
        photonViewID = PhotonNetwork.LocalPlayer.ActorNumber;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        Debug.Log("Photon View ID: " + photonViewID);
        if (view.IsMine)
        {
            if (photonViewID == 1) player = gameManager.PlayerM;
            else player = gameManager.PlayerF;
            playerScript = player.GetComponent<Player>();
            playerScript.HandleWireColor = "Default";

            currentMap = (int)playerScript.CurrentPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);

        }
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
        Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        //socket.ChangePlayerAttrStartPoint(playerScript);
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        socket.ChangePlayerAttrStartPoint(targetP);
        //Debug.Log("Socket found: " + socket + " - " + socket.transform.position);
    }

    [PunRPC]
    void CallChangePlayerAttrEndPoint(int currentMap, int xTarget, int yTarget, int photonTargetID)
    {
        Socket socket = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Socket>();
        //socket.ChangePlayerAttrStartPoint(playerScript);
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        socket.ChangePlayerAttrEndPoint(targetP);
        //Debug.Log("Socket found: " + socket + " - " + socket.transform.position);
    }

    [PunRPC]
    private void SetPreviousMove(int photonTargetID, string targetPreviousMove)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        targetP.PreviousMove = targetPreviousMove;
    }

    [PunRPC]
    private void GenerateWire(int mapIndex, int xAxis, int yAxis, string type, int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }
        Debug.Log("TargetP-------: " + targetP);
        if (type == "Bridge" && !targetP.IsNotPickWire)
        {
            // Wire w = new Wire();
            // w.Start();
            // w.wireZAxis = gameManager.PlayGridList[mapIndex][xAxis, yAxis].GetComponent<Bridge>().GetZAxisWire(playerScript.PreviousMove);
            // w.GenerateWire(targetP);
        }
        else if (type == "Wire" && !targetP.IsNotPickWire || targetP.IsAtSocket)
        {
            Debug.Log("I found wirespawner: " + wireSpawner);
            Debug.Log("TargetP-------: " + targetP.TempNextKey);
            GameObject w = wireSpawner.GenerateWire(targetP);
            Debug.Log(w + " is found wire!");

            Vector2 wirePosition = new Vector2(w.transform.position.x, w.transform.position.y);
            gameManager.WireMap[wirePosition] = true;
        }
        // else if (type == "Dimension" && !playerScript.IsNotPickWire)
        // {
        //     int wireRotation = (playerScript.TempNextKey == "Up" || playerScript.TempNextKey == "Down") ? 1 : 0;
        //     Wire w = new Wire();
        //     w.Start();
        //     w.RenderWire(obj.transform.position, 0, wireRotation, playerScript.HandleWireColor);
        // }
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
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Up");

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Up";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Up");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.DownArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Down");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Down";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Down");
                }
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.LeftArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Left");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Left";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Left");
                }
                this.transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
            }
            else if (!isStepOnIce && Input.GetKeyDown(KeyCode.RightArrow) && enableMove && !isPauseGame)
            {
                view.RPC("SetTempTargetMove", RpcTarget.All, photonViewID, "Right");
                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.TempNextKey))
                {
                    //playerScript.PreviousMove = "Right";
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, "Right");
                }
                this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else if (isStepOnEscalator && enableMove)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);

                if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Escalator")
                {
                    Escalator escalator = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Escalator>();
                    playerScript.TempTargetPosition = escalator.GetNextPosition(playerScript);
                    playerScript.TempNextKey = escalator.Direction;
                }
                else
                {
                    StopStepOnEscalator();
                    return;
                }

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.PreviousMove))
                {
                    view.RPC("SetPreviousMove", RpcTarget.All, photonViewID, playerScript.TempNextKey);
                }
            }
            else if (isStepOnIce && enableMove)
            {
                playerScript.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
                if (playerScript.PreviousMove == "Left")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
                }
                else if (playerScript.PreviousMove == "Right")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
                }
                else if (playerScript.PreviousMove == "Up")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
                }
                else if (playerScript.PreviousMove == "Down")
                {
                    playerScript.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
                }

                if (CanStepToPosition(playerScript.TempCurrentPosition, playerScript.TempTargetPosition, playerScript.PreviousMove))
                {
                    //playerScript.PreviousMove = playerScript.PreviousMove;
                }
                else
                {
                    StopStepOnIce();
                }
            }
            StepMove();
        }
    }

    [PunRPC]
    private void SetTempTargetMove(int photonTargetID, string tempNextKey)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1) targetP = gameManager.PlayerM.GetComponent<Player>();
            else targetP = gameManager.PlayerF.GetComponent<Player>();
        }

        if (tempNextKey == "Up")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x, transform.position.y + moveSteps);
            targetP.TempNextKey = "Up";
        }
        else if (tempNextKey == "Left")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x - moveSteps, transform.position.y);
            targetP.TempNextKey = "Left";
        }
        else if (tempNextKey == "Down")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x, transform.position.y - moveSteps);
            targetP.TempNextKey = "Down";
        }
        else if (tempNextKey == "Right")
        {
            targetP.TempCurrentPosition = new Vector2(transform.position.x, transform.position.y);
            targetP.TempTargetPosition = new Vector2(transform.position.x + moveSteps, transform.position.y);
            targetP.TempNextKey = "Right";
        }
    }

    [PunRPC]
    private void UpdateLocation(int photonTargetID)
    {
        Player targetP = playerScript;
        if (photonTargetID != photonViewID)
        {
            if (photonTargetID == 1)
            {
                targetP = gameManager.PlayerM.GetComponent<Player>();
                targetP.CurrentPosition = gameManager.PlayerM.transform.position;
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
            else
            {
                targetP = gameManager.PlayerF.GetComponent<Player>();
                targetP.CurrentPosition = gameManager.PlayerF.transform.position;
                targetP.TargetPosition = targetP.TempTargetPosition;
            }
        }
        if (view.IsMine)
        {
            playerScript.CurrentPosition = this.transform.position;
            playerScript.TargetPosition = playerScript.TempTargetPosition;
            currentMap = (int)playerScript.CurrentPosition.x / 100;
            xCurrent = (int)(playerScript.CurrentPosition.x % 100);
            yCurrent = (int)(playerScript.CurrentPosition.y);
            xTarget = (int)(playerScript.TargetPosition.x % 100);
            yTarget = (int)(playerScript.TargetPosition.y);
        }
    }

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
            Bridge bridge = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
            totalCheck = bridge.CheckCurrentStep(bridge, playerScript, playerScript.PreviousMove);
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
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrEndPoint(playerScript);
                            view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                            //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xTarget, yTarget, "Wire", photonViewID);
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
            Bridge bridge = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Bridge>();
            totalCheck = bridge.CheckNextStep(playerScript);
            if (totalCheck)
            {
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
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
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
            }
            else if (socket.CheckSocketEndPoint(playerScript))
            {
                totalCheck = true;
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
                }
                //socket.ChangePlayerAttrEndPoint(playerScript);
                view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                //GenerateWire(currentMap, xTarget, yTarget, "Wire", null);
                view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
                gameManager.Score++;
            }
            else if (socket.CheckSocketStartPoint(playerScript))
            {
                totalCheck = true;
                //socket.ChangePlayerAttrStartPoint(playerScript);
                view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Wall")
        {
            totalCheck = false;
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionIn")
        {
            DimensionIn dIn = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DimensionIn>();

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
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Bridge", photonViewID);
                }
                else
                {
                    // GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xTarget, yTarget, "Wire", photonViewID);
                }

                if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                {
                    Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrEndPoint(playerScript);
                        view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                        view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xTarget, yTarget, "Wire", photonViewID);
                        gameManager.Score++;
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrStartPoint(playerScript);
                        view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                    }
                }
                GameObject dOut = dIn.GetDimensionOut(playerScript);
                //GenerateWire(0, 0, 0, "Dimension", dOut);
                //view.RPC("GenerateWire", RpcTarget.All, 0, 0, 0, "Dimension", dOut);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DimensionOut")
        {
            DimensionOut dOut = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DimensionOut>();

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
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }

                if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                {
                    Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                    if (socket.CheckSocketEndPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrEndPoint(playerScript);
                        view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                        view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xCurrent, yCurrent, "Wire", photonViewID);
                        gameManager.Score++;
                    }
                    else if (socket.CheckSocketStartPoint(playerScript))
                    {
                        //socket.ChangePlayerAttrStartPoint(playerScript);
                        view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                    }
                }
                else if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Bridge")
                {
                    Bridge bridge = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Bridge>();
                    bool changePlayerDefaultZAxis = bridge.CheckNextStep(playerScript);
                    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, playerScript.DefaultZAxis);
                }

                //GenerateWire(0, 0, 0, "Dimension", dOut.gameObject);
                //view.RPC("GenerateWire", RpcTarget.All, 0, 0, 0, "Dimension", dOut.gameObject);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;

            totalCheck = button.CheckNextStep(playerScript);
            if (totalCheck)
            {
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
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
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
            }
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "DoorButton")
        {
            DoorButton button = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<DoorButton>();
            button.IsActive = true;
            view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
            //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
            view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
        }
        else if (gameManager.PlayGridList[currentMap][xTarget, yTarget].tag == "Door")
        {
            Door door = gameManager.PlayGridList[currentMap][xTarget, yTarget].GetComponent<Door>();
            totalCheck = door.CheckNextStep(playerScript);

            if (totalCheck)
            {
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
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
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                    }

                    if (gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].tag == "Socket")
                    {
                        Socket socket = gameManager.PlayGridList[tempCurrentMap][xTarget, yTarget].GetComponent<Socket>();
                        if (socket.CheckSocketEndPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrEndPoint(playerScript);
                            view.RPC("CallChangePlayerAttrEndPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                            //GenerateWire(tempCurrentMap, xCurrent, yCurrent, "Wire", null);
                            view.RPC("GenerateWire", RpcTarget.All, tempCurrentMap, xCurrent, yCurrent, "Wire", photonViewID);
                            gameManager.Score++;
                        }
                        else if (socket.CheckSocketStartPoint(playerScript))
                        {
                            //socket.ChangePlayerAttrStartPoint(playerScript);
                            view.RPC("CallChangePlayerAttrStartPoint", RpcTarget.All, currentMap, xTarget, yTarget, photonViewID);
                        }
                    }
                }
                else
                {
                    view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                    if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                    }
                    else
                    {
                        //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                        view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
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
                view.RPC("UpdateLocation", RpcTarget.All, photonViewID);
                if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Bridge", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Bridge", photonViewID);
                }
                else
                {
                    //GenerateWire(currentMap, xCurrent, yCurrent, "Wire", null);
                    view.RPC("GenerateWire", RpcTarget.All, currentMap, xCurrent, yCurrent, "Wire", photonViewID);
                }
            }
        }

        if (!totalCheck)
        {
            if (gameManager.PlayGridList[currentMap][xCurrent, yCurrent].tag == "Bridge")
            {
                Bridge bridge = gameManager.PlayGridList[currentMap][xCurrent, yCurrent].GetComponent<Bridge>();
                bridge.CheckOpacity(playerScript, playerScript.PreviousMove);
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

    private void StopStepOnIce()
    {
        isStepOnIce = false;
        moveSpeed = 5f;
    }

    private void StopStepOnEscalator()
    {
        isStepOnEscalator = false;
        if (isStepOnIce)
            moveSpeed = 7f;
        else
            moveSpeed = 5f;
    }
}