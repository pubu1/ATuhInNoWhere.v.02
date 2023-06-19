using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviourPunCallbacks
{
    // [SerializeField]
    // private GameObject player;

    // [SerializeField]
    // private GameObject GameOverUI;

    // [SerializeField]
    // private GameObject PauseUI;

    // [SerializeField]
    // private GameObject GuideUI;
    [SerializeField] GameObject playerPrefabM;
    [SerializeField] GameObject playerPrefabF;

    private GameObject[,] grid;

    private GameObject[] prefabList;
    private List<string> path;
    private List<string[,]> inputList;

    private InputManager inputManager;


    public List<GameObject[,]> MapGridList { get; set; }
    public List<GameObject[,]> PlayGridList { get; set; }

    private Dictionary<int, GameObject> doorButtonList;
    public Dictionary<Vector2, Wire> WireMap {get; set;}

    // private Dictionary<Vector2, Socket> pointType;

    private bool openPauseUI = false;
    private bool openGuideUI = false;

    public int Score { get; set; }
    private int SocketAmount = 0;
    private bool IsCameraTargetPlayer{get; set;}

    /*Daviz*/
    GameObject btn;
    GameObject esc;

    public void Start()
    {
            inputManager = new InputManager();
            doorButtonList = new Dictionary<int, GameObject>();
            Score = 0;
            IsCameraTargetPlayer = false;
            inputList = inputManager.LoadGridFromFile();
            prefabList = FindAllPrefabs();
            if (PhotonNetwork.IsConnected)
            {
                //this.gameObject.GetComponent<PhotonView>().RPC("RPC_PunMap", RpcTarget.All);   
                InitializeMap();
                ConnectMap();
            }

            PlayGridList = MapGridList;  
    }

    // [PunRPC]
    // private void RPC_PunMap(){
    //     InitializeMap();
    //     ConnectMap();
    //     PlayGridList = MapGridList;
    // }

    private GameObject InstantiatePrefab(string prefabName, int x, int y)
    {
        GameObject prefab = prefabList.FirstOrDefault(o => o.name == prefabName);
        Quaternion rotation = prefab.transform.rotation;
        float z = prefab.transform.position.z;
        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, z), rotation) as GameObject;
        /*Daviz*/
        // GameObject instantiatedPrefab;
        // if(prefabName == "Socket"){
        //     instantiatedPrefab = PhotonNetwork.Instantiate(prefabName, new Vector3(x, y, z), rotation) as GameObject;
        // } else{
        //     instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, z), rotation) as GameObject;
        // }
        return instantiatedPrefab;
    }

    private GameObject InstantiatePlayerM(int id, int x, int y)
    {
        GameObject prefab = prefabList.FirstOrDefault(o => o.name == "Player");
        Quaternion rotation = playerPrefabM.transform.rotation;
        float z = playerPrefabM.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabM.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }
    private GameObject InstantiatePlayerF(int id, int x, int y)
    {
        GameObject prefab = prefabList.FirstOrDefault(o => o.name == "Player");
        Quaternion rotation = playerPrefabF.transform.rotation;
        float z = playerPrefabF.transform.position.z;
        int roundedX = Mathf.RoundToInt(x);
        int roundedY = Mathf.RoundToInt(y);
        Vector3 flooredPosition = new Vector3(roundedX, roundedY, z);
        GameObject instantiatedPrefab = PhotonNetwork.Instantiate(playerPrefabF.name, flooredPosition, rotation) as GameObject;
        instantiatedPrefab.transform.position = flooredPosition;
        return instantiatedPrefab;
    }

    private void InitializeMap()
    {
        WireMap = new Dictionary<Vector2, Wire>();
        MapGridList = new List<GameObject[,]>();
        int offset = 0;
        int currentMap = 0;
        foreach (string[,] inputMap in inputList)
        {
            int n = inputMap.GetLength(0);
            int m = inputMap.GetLength(1);
            //Debug.Log(n + " x " + m);
            string[,] randomMap = new string[m, n];

            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    randomMap[i, j] = inputMap[n - j - 1, i];
                }
            }
            grid = new GameObject[m, n];
            GameObject ground = prefabList.FirstOrDefault(o => o.name == "Ground");
            float groundZ = ground.transform.position.z;
            Quaternion groundRotate = ground.transform.rotation;
            for (int x = 0; x < m; ++x)
            {
                for (int y = 0; y < n; ++y)
                {
                    //Init ground
                    Instantiate(ground, new Vector3(x + offset, y, groundZ), groundRotate);
                    string item = randomMap[x, y];
                    GameObject prefab;
                    
                    if (item.Contains("Socket"))
                    {
                        string hexCode = item.Split("_")[1];
                        item = "Socket";

                        //change color
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        ChangeColor changeColor = new ChangeColor();
                        instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                        changeColor.ChangeSpriteColor(instantiatedPrefab, hexCode);
                        grid[x, y] = instantiatedPrefab;
                        SocketAmount++;
                    }
                    else if (item.Contains("PlayerM"))
                    {
                        int id = int.Parse(item.Split(':')[1]);
                        item = "Player";


                        GameObject instantiatedPrefab = InstantiatePlayerM(id, x + offset, y);
                        instantiatedPrefab.GetComponent<Player>().ID = id;
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("PlayerF"))
                    {
                        int id = int.Parse(item.Split(':')[1]);
                        item = "Player";


                        GameObject instantiatedPrefab = InstantiatePlayerF(id, x + offset, y);
                        instantiatedPrefab.GetComponent<Player>().ID = id;
                        grid[x, y] = instantiatedPrefab;

                    }
                    else if (item.Contains("Bridge"))
                    {
                        string direction = item.Split(':')[1];
                        item = "Bridge";

                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        if(direction == "H"){
                            instantiatedPrefab.GetComponent<Bridge>().Direction = "Horizontal";
                        } else{
                            instantiatedPrefab.GetComponent<Bridge>().Direction = "Vertical";
                            instantiatedPrefab.GetComponent<Bridge>().RenderSprite();
                        }
                        
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Dimension"))
                    {
                        string[] dimension = item.Split(':');
                        string dimensionWay = dimension[0];

                        GameObject instantiatedPrefab = InstantiatePrefab(dimensionWay, x + offset, y);
                        if (dimensionWay == "DimensionIn")
                        {
                            instantiatedPrefab.GetComponent<DimensionIn>().ID = int.Parse(dimension[1]);
                        } else
                        {
                            instantiatedPrefab.GetComponent<DimensionOut>().OutDirection = dimension[1];
                        }
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("DoorButton"))
                    {
                        int buttonID = int.Parse(item.Split(':')[1]);
                        item = "DoorButton";
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        instantiatedPrefab.GetComponent<DoorButton>().Start();
                        instantiatedPrefab.GetComponent<DoorButton>().ID = buttonID;
                        grid[x, y] = instantiatedPrefab;
                        doorButtonList[buttonID] = instantiatedPrefab;
                    } 
                    else if (item.Contains("Door"))
                    {
                        int doorID = int.Parse(item.Split(':')[1]);
                        GameObject instantiatedPrefab = InstantiatePrefab(item.Split(':')[0], x + offset, y);
                        instantiatedPrefab.GetComponent<Door>().ID = doorID;
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("EscButton"))
                    {
                        item = "EscButton";

                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        instantiatedPrefab.GetComponent<EscButton>().Start();
                        
                        btn=instantiatedPrefab;
                        
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Escalator"))
                    {
                        string direction = item.Split(':')[1];
                        item = "Escalator";

                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                     
                        if(direction == "U"){
                            instantiatedPrefab.GetComponent<Escalator>().Direction = "Up";
                        } else if(direction == "D"){
                            instantiatedPrefab.GetComponent<Escalator>().Direction = "Down";
                        } else if(direction == "L"){
                            instantiatedPrefab.GetComponent<Escalator>().Direction = "Left";
                        } else if(direction == "R"){
                            instantiatedPrefab.GetComponent<Escalator>().Direction = "Right";
                        }

                        instantiatedPrefab.GetComponent<Escalator>().Start();
                        instantiatedPrefab.GetComponent<Escalator>().RenderSprite();
                        
                        esc=instantiatedPrefab;
                        esc.GetComponent<Escalator>().button = btn.GetComponent<EscButton>();
                        
                        grid[x, y] = instantiatedPrefab;
                    }
                    else
                    {
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        grid[x, y] = instantiatedPrefab;
                    }
                }
            }
            MapGridList.Add(grid);
            offset += 100;
            ++currentMap;
        }
    }

    private void ConnectMap()
    {
        for (int i = 0; i < MapGridList.Count; ++i)
        {
            foreach (GameObject item in MapGridList[i])
            {
                //Connect Dimension In and Out
                if (item.tag == "DimensionIn") {
                    int dimension = item.GetComponent<DimensionIn>().ID;
                    foreach (GameObject insideItem in MapGridList[dimension])
                    {
                        int ok = 0;
                        if (insideItem.tag == "DimensionOut")
                        {
                            string dir = insideItem.GetComponent<DimensionOut>().OutDirection;
                            if (dir == "Left")
                            {
                                item.GetComponent<DimensionIn>().exitLeft = insideItem;
                            } else if (dir == "Top")
                            {
                                item.GetComponent<DimensionIn>().exitTop = insideItem;
                            } else if (dir == "Bottom")
                            {
                                item.GetComponent<DimensionIn>().exitBottom = insideItem;
                            } else
                            {
                                item.GetComponent<DimensionIn>().exitRight = insideItem;
                            }
                            insideItem.GetComponent<DimensionOut>().BaseDimension = item.GetComponent<DimensionIn>();
                        }
                    }
                    item.GetComponent<DimensionIn>().Start();
                    item.GetComponent<DimensionIn>().RenderSprite();
                }


                //Connect Button & Door
                else if (item.tag == "Door")
                {
                    int doorID = item.GetComponent<Door>().ID;
                    foreach (int btnID in inputManager.ListDoor[doorID])
                    {
                        item.GetComponent<Door>().Button = doorButtonList[btnID].GetComponent<DoorButton>();
                    }
                }
            }
        }

    }

    public List<string> GetPath() { return path; }
    public GameObject GetObjectFromGrid(int x, int y)
    {
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            return grid[x, y];
        }
        return null;
    }

    public List<GameObject[,]> GetMapGridList()
    {
        return MapGridList;
    }

    public GameObject GetPlayer()
    {
        for (int i = 0; i < MapGridList.Count; ++i)
        {
            foreach (GameObject item in MapGridList[i])
            {
                if (item.tag == "Player") return item;
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        //GameOverUI.SetActive(false);
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            PhotonView view = this.gameObject.GetComponent<PhotonView>();
            view.RPC("ResetTheGame", RpcTarget.All);
            //ResetTheGame();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Get the Canvas
            GameObject canvasObject = GameObject.Find("Canvas");
            Canvas canvasComponent = canvasObject.GetComponent<Canvas>();

            //Get Player Camera and World Camera
            Camera playerCamera = GetPlayer().transform.Find("Camera").gameObject.GetComponent<Camera>();
            Camera worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            if(!IsCameraTargetPlayer){
                worldCamera.enabled = false;
                playerCamera.enabled = true;
                
                canvasComponent.worldCamera = playerCamera;
                IsCameraTargetPlayer = true;
            } else {
                worldCamera.enabled = true;
                playerCamera.enabled = false;

                canvasComponent.worldCamera = worldCamera;
                IsCameraTargetPlayer = false;
            }
        }
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {
        //     openPauseUI = !openPauseUI;
        //     PauseUI.SetActive(openPauseUI);
        // }

        // if (Input.GetKeyDown(KeyCode.G))
        // {
        //     openGuideUI = !openGuideUI;
        //     GuideUI.SetActive(openGuideUI);
        // }
        if (Score == SocketAmount / 2)
        {
            //GameOverUI.SetActive(true);
        }
    }
    [PunRPC]
    private void ResetTheGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public GameObject[,] GetGrid()
    {
        return grid;
    }

    private static GameObject[] FindAllPrefabs()
    {
        string prefabFolder = "Prefabs";
        GameObject[] prefabList = Resources.LoadAll<GameObject>(prefabFolder);
        return prefabList;
    }

    public GameObject GetPrefabByName(string prefabName)
    {
        GameObject[] prefabList = FindAllPrefabs();

        foreach (GameObject prefab in prefabList)
        {
            if (prefab.name == prefabName) return prefab;
        }
        return null;
    }
}
