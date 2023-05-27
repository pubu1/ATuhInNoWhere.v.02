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
    private List<GameObject[,]> gridList;
    private Dictionary<Vector2, PipePoint> pointType;

    private bool openPauseUI = false;
    private bool openGuideUI = false;

    public int Score { get; set; }

    public void Start()
    {
        //player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 6);
        Score = 0;
        gridList = new List<GameObject[,]>();  

        prefabList = FindAllPrefabs();
        InitializeGrid1();
        InitializeGrid2();

        for (int i = 0; i < gridList.Count; ++i)
        {
            foreach (GameObject item in gridList[i])
            {
                Vector3 position = item.transform.position;
                int x = Mathf.RoundToInt(position.x);
                int y = Mathf.RoundToInt(position.y);

                Debug.Log(x + " - " + y + ": " + item);
            }
        }


            // Populate the grid with objects
            /*        PopulateGrid("Wall", GameObject.FindGameObjectsWithTag("Wall"));
                    PopulateGrid("PipePoint", GameObject.FindGameObjectsWithTag("PipePoint"));
                    PopulateGrid("Bridge", GameObject.FindGameObjectsWithTag("Bridge"));
                    PopulateGrid("Dimension", GameObject.FindGameObjectsWithTag("Dimension"));
                    PopulateGrid("DimensionTeleporter", GameObject.FindGameObjectsWithTag("DimensionTeleporter"));
                    PopulateGrid("DoorButton", GameObject.FindGameObjectsWithTag("DoorButton"));
                    PopulateGrid("Door", GameObject.FindGameObjectsWithTag("Door"));
                    PopulateGrid("Pool", GameObject.FindGameObjectsWithTag("Pool"));*/
        }

    private void InitializeGrid1()
    {
        // Determine the size of the grid based on the level or desired dimensions
        int gridSizeX = 2; // set the appropriate size
        int gridSizeY = 3; // set the appropriate size

        string[,] inputMap = new string[,] {
            {"Wall", "Dimension", "Button"},
            {"Pool", "Player", "BluePipePoint"},
        };

        Debug.Log("Input map: " + inputMap.GetLength(0) + " / " + inputMap.GetLength(1));

        string[,] randomMap = new string[gridSizeY, gridSizeX];

        for (int i = 0; i < gridSizeY; ++i)
        {
            for (int j = 0; j < gridSizeX; ++j)
            {
                randomMap[i, j] = inputMap[gridSizeX-j-1, i];
            }
        }

        /*        randomMap = new string[,] {
                    {"Wall", "Wall", "Wall", "Wall"},
                    {"Wall", "Player", "BluePipePoint", "Wall"},
                    {"Wall", "BluePipePoint", "Socket_Yellow", "Wall"},
                    {"Wall", "Button", "Dimension", "Pool"},
                    {"Wall", "Wall", "Wall", "Wall"}
                };*/

        // Initialize the grid
        gridSizeX = 3; // set the appropriate size
        gridSizeY = 2;
        grid = new GameObject[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                string item = randomMap[x, y];
                GameObject prefab;
                if (item.Contains("Socket"))
                {
                    string hexCode = item.Split("_")[1];
                    Debug.Log(hexCode);
                    item = "Socket";
                    //find the prefab
                    prefab = prefabList.FirstOrDefault(o => o.name == item);
                    //render prefab
                    GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    //change color
                    ChangeColor changeColor = new ChangeColor();
                    instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                    changeColor.ChangeSpriteColor(instantiatedPrefab, hexCode);     
                    grid[x, y] = instantiatedPrefab;
                }
                else
                {
                    prefab = prefabList.FirstOrDefault(o => o.name == item);
                    GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    grid[x, y] = instantiatedPrefab;
                }
            }
        }
        gridList.Add(grid);
    }

    private void InitializeGrid2()
    {
        // Determine the size of the grid based on the level or desired dimensions
        int gridSizeX = 3; // set the appropriate size
        int gridSizeY = 3; // set the appropriate size

        string[,] inputMap = new string[,] {
            {"Wall", "Wall", "Wall"},
            {"Button", "Button", "V_Bridge"},
            {"Button", "H_Bridge", "Button"}
        };

        Debug.Log("Input map: " + inputMap.GetLength(0) + " / " + inputMap.GetLength(1));

        string[,] randomMap = new string[gridSizeY, gridSizeX];

        for (int i = 0; i < gridSizeY; ++i)
        {
            for (int j = 0; j < gridSizeX; ++j)
            {
                randomMap[i, j] = inputMap[gridSizeX - j - 1, i];
            }
        }

        // Initialize the grid
        gridSizeX = 3; // set the appropriate size
        gridSizeY = 2;
        grid = new GameObject[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                string item = randomMap[x, y];
                GameObject prefab;
                if (item.Contains("Socket"))
                {
                    string hexCode = item.Split("_")[1];
                    Debug.Log(hexCode);
                    item = "Socket";
                    //find the prefab
                    prefab = prefabList.FirstOrDefault(o => o.name == item);
                    //render prefab
                    GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(10+x, 10+y, 0), Quaternion.identity) as GameObject;
                    //change color
                    ChangeColor changeColor = new ChangeColor();
                    instantiatedPrefab.GetComponent<Socket>().Color = hexCode;

                    changeColor.ChangeSpriteColor(instantiatedPrefab, hexCode);
                    grid[x, y] = instantiatedPrefab;
                }
                else
                {
                    prefab = prefabList.FirstOrDefault(o => o.name == item);
                    GameObject instantiatedPrefab = Instantiate(prefab, new Vector3(10+x, 10+y, 0), Quaternion.identity) as GameObject;
                    grid[x, y] = instantiatedPrefab;
                }
            }
        }
        gridList.Add(grid);
    }

    private void PopulateGrid(string key, GameObject[] objects)
    {
        foreach (GameObject item in objects)
        {
            Vector3 position = item.transform.position;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            Debug.Log(x + " - " + y + ": " + item);

            // Store the object in the grid at the corresponding position
            grid[x, y] = item;

            // Store additional information if needed
/*            if (key == "PipePoint")
            {
                pointType[new Vector2(x, y)] = item.GetComponent<PipePoint>();
            }*/
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
    public Dictionary<Vector2, PipePoint> GetPointType() { return pointType; }
    // public GameObject GetPlayer() { return player; }

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
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        GameObject[] prefabList = new GameObject[guids.Length];

        int i = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            prefabList[i++] = prefab;

            //Debug.Log("Prefab name: " + prefab.name);
            // if (prefab.name == "Wall")
            // {
            //     Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
            // }
        }
        return prefabList;
    }

}
