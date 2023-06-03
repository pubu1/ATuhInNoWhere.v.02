using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class GameManager : MonoBehaviour
{
    // [SerializeField]
    // private GameObject player;

    // [SerializeField]
    // private GameObject GameOverUI;

    // [SerializeField]
    // private GameObject PauseUI;

    // [SerializeField]
    // private GameObject GuideUI;

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

/*Daviz*/
    private Teleporter tele01;
    private Teleporter tele02;
    int indd=0;
    int indd1=0, indd2=0, indd3=0, indd4=0;
    public void Start()
    {
        inputManager = new InputManager();
        doorButtonList = new Dictionary<int, GameObject>();
        //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 6);
        Score = 0;
        inputList = inputManager.LoadGridFromFile();
        prefabList = FindAllPrefabs();
        InitializeMap();
        ConnectMap();
        PlayGridList = MapGridList;
    }

    private GameObject InstantiatePrefab(string prefabName, int x, int y)
    {
        GameObject prefab = prefabList.FirstOrDefault(o => o.name == prefabName);
        Quaternion rotation = prefab.transform.rotation;
        float z = prefab.transform.position.z;
        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, z), rotation) as GameObject;
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
                    //Debug.Log(x + " - " + y);
                    string item = randomMap[x, y];
                    GameObject prefab;
                    
                    if (item.Contains("Socket"))
                    {
                        string hexCode = item.Split("_")[1];
                        item = "Socket";
                        /*                        //find the prefab
                                                prefab = prefabList.FirstOrDefault(o => o.name == item);
                                                Quaternion rotation = prefab.transform.rotation;
                                                float z = prefab.transform.position.z;
                                                //render prefab
                                                GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;*/
                        //change color
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        ChangeColor changeColor = new ChangeColor();
                        instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                        changeColor.ChangeSpriteColor(instantiatedPrefab, hexCode);
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Player"))
                    {
                        int id = int.Parse(item.Split(':')[1]);
                        item = "Player";
/*                        prefab = prefabList.FirstOrDefault(o => o.name == item);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;*/
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        instantiatedPrefab.GetComponent<Player>().ID = id;
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Dimension"))
                    {
                        string[] dimension = item.Split(':');
                        string dimensionWay = dimension[0];
/*                        prefab = prefabList.FirstOrDefault(o => o.name == dimensionWay);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;*/
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
                    /*Daviz*/
                    else if(item.Contains("Teleporter")){
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        if(indd == 0){
                            indd++;
                            indd1=x;
                            indd2=y;
                            tele01 = instantiatedPrefab.GetComponent<Teleporter>();
                        } else{
                            tele02 = instantiatedPrefab.GetComponent<Teleporter>();
                            indd3=x;
                            indd4=y;
                        }
                        grid[x, y] = instantiatedPrefab;
                    }
                    else
                    {
                        /*                        prefab = prefabList.FirstOrDefault(o => o.name == item);
                                                Quaternion rotation = prefab.transform.rotation;
                                                float z = prefab.transform.position.z;
                                                GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;*/
                        Debug.Log("I found this: " + item);
                        GameObject instantiatedPrefab = InstantiatePrefab(item, x + offset, y);
                        grid[x, y] = instantiatedPrefab;
                    }
                }
            }
            MapGridList.Add(grid);
            offset += 100;
            ++currentMap;
        }

        /*Daviz*/
        MapGridList[0][indd1,indd2].GetComponent<Teleporter>().TargetTeleporter = tele02;
        MapGridList[0][indd3, indd4].GetComponent<Teleporter>().TargetTeleporter  = tele01;
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
                            //Debug.Log("insideItem!");
                            ok = 1;
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
                        //if (ok == 0) Debug.Log("Not found any!");
                    }
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
    //public Dictionary<Vector2, Socket> GetPointType() { return pointType; }
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
        // GameOverUI.SetActive(false);
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     ResetTheGame();
        // }
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
        // if (Score == pointType.Count / 2)
        // {
        //     GameOverUI.SetActive(true);
        // }
    }

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
