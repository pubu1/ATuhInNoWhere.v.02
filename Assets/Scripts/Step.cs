using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private float moveSteps = 1.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Sprite[] pipeSprites;
    private GameObject player;
    [SerializeField] private GameObject body;
    private bool enableMove = true;
    private bool isPauseGame = false;
    private bool isNotPickPipe = true;
    private bool isAtPointPosition = false;
    private string handlePipeColor;
    private Vector2 positionOfStartPoint;
    private Vector2 currentPosition;
    private Vector2 targetPosition;
    private Vector2 tempCurrentPosition;
    private Vector2 tempTargetPosition;
    private Vector2 entranceDimensionPosition;
    private List<string> path;
    private List<GameObject> pipes;
    private GameObject[,] grid;  // 2D grid for storing game objects
    private static float gridCellSize = 1.0f;
    int gridWidth = 5; // specify the grid width based on your game's requirements */;
    int gridHeight = 6; //specify the grid height based on your game's requirements */;

    private string previousMove;
    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        //gameManager.Start();
        //player = gameManager.GetPlayer();
        currentPosition = player.transform.position;
        targetPosition = player.transform.position;
        tempCurrentPosition = player.transform.position;
        tempTargetPosition = player.transform.position;
        entranceDimensionPosition = player.transform.position;
        pipes = new List<GameObject>();
        handlePipeColor = "Default";
        previousMove = "";

        // Create the 2D grid
        grid = new GameObject[gridWidth, gridHeight];
        PopulateGrid();
    }

    private void PopulateGrid()
    {
        // Get the Game Manager's grid
/*        GameObject[,] gameManagerGrid = gameManager.GetGrid();
        //Dictionary<Vector2, GameObject> c = gameManager.GetGrid();

        // Copy the objects from Game Manager's grid to Step's grid
        Debug.Log("Step grid size: " + gameManagerGrid.Length);

        for (int x = 0; x < gameManagerGrid.GetLength(0); ++x)
        {
            for (int y = 0; y < gameManagerGrid.GetLength(1); ++y)
            {
                GameObject item = gameManagerGrid[x, y];
                if (item != null)
                    Debug.Log(x + " , " + y + ": " + item);
            }
        }*/
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPauseGame = !isPauseGame;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Handle the Up arrow key press
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Handle the Down arrow key press
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Handle the Left arrow key press
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Handle the Right arrow key press
        }
    }
}
