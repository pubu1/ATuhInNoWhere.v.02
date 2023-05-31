using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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

    // private Dictionary<Vector2, Socket> pointType;

    private bool openPauseUI = false;
    private bool openGuideUI = false;

    public int Score { get; set; }

    private GameObject check;
    public void Start()
    {
        inputManager = new InputManager();
        //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 6);
        Score = 0;
        inputList = inputManager.LoadGridFromFile();
        prefabList = FindAllPrefabs();
        InitializeMap();
        ConnectMap();
        PlayGridList = MapGridList;
    }

    private void InitializeMap()
    {
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
            for (int x = 0; x < m; ++x)
            {
                for (int y = 0; y < n; ++y)
                {
                    //Debug.Log(x + " - " + y);
                    string item = randomMap[x, y];
                    //Debug.Log(" : " + item);
                    GameObject prefab;
                    if (item.Contains("Socket"))
                    {
                        string hexCode = item.Split("_")[1];
                        //Debug.Log(hexCode);
                        item = "Socket";
                        //find the prefab
                        prefab = prefabList.FirstOrDefault(o => o.name == item);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        //render prefab
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;
                        //change color
                        ChangeColor changeColor = new ChangeColor();
                        instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                        changeColor.ChangeSpriteColor(instantiatedPrefab, hexCode);
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Player"))
                    {
                        int id = int.Parse(item.Split(':')[1]);
                        item = "Player";
                        prefab = prefabList.FirstOrDefault(o => o.name == item);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;
                        instantiatedPrefab.GetComponent<Player>().ID = id;
                        grid[x, y] = instantiatedPrefab;
                    }
                    else if (item.Contains("Dimension"))
                    {
                        string[] dimension = item.Split(':');
                        string dimensionWay = dimension[0];
                        prefab = prefabList.FirstOrDefault(o => o.name == dimensionWay);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;
                        if (dimensionWay == "DimensionIn")
                        {
                            instantiatedPrefab.GetComponent<DimensionIn>().ID = int.Parse(dimension[1]);
                        } else
                        {
                            instantiatedPrefab.GetComponent<DimensionOut>().OutDirection = dimension[1];
                        }
                        grid[x, y] = instantiatedPrefab;
                    }
                    else
                    {
                        prefab = prefabList.FirstOrDefault(o => o.name == item);
                        //Debug.Log("I found this: " + prefab);
                        Quaternion rotation = prefab.transform.rotation;
                        float z = prefab.transform.position.z;
                        GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x + offset, y, z), rotation) as GameObject;
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
        //Connect Dimension In and Out
        for (int i = 0; i < MapGridList.Count; ++i)
        {
            foreach (GameObject item in MapGridList[i])
            {
                if (item.tag == "DimensionIn") {
                    int dimension = item.GetComponent<DimensionIn>().ID;
                    foreach (GameObject insideItem in MapGridList[dimension])
                    {
                        int ok = 0;
                        if (insideItem.tag == "DimensionOut")
                        {
                            Debug.Log("insideItem!");
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
                        if (ok == 0) Debug.Log("Not found any!");
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
        Debug.Log("Finding all prefabs....");
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
