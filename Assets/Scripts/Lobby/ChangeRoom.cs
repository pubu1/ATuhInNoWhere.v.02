using TMPro;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class ChangeRoom : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    [SerializeField]
    private GameObject optionSelectScreen; // screen to select create or join a room

    [SerializeField]
    private GameObject joinRoomScreen; // canvas screen to click the room number and enter the room

    [Header("Room")]
    [SerializeField]
    private TMP_InputField roomNameJoin; // input-field of text of the room to join

    [SerializeField]
    private TMP_Text errorText; // error text to display if the room exist or not

    // use for display the error text to be flicker
    public float flickerDuration = 2f;
    public float flickerInterval = 0.05f;
    private bool isFlickering = false;

    [SerializeField]
    private TMP_Text errorChoosePanelTxt; // error text to display if the room exist or not

    [Header("Event System")]
    [SerializeField]
    private EventSystem eventSystem;
    [SerializeField]
    private GameObject choosePanelButton;
    [SerializeField]
    private GameObject joinPanelButton;

    public RoomOptions roomOptions = new RoomOptions(); // new RoomOption to create a room

    [Header("PLayer")]
    [SerializeField]
    private TMP_Text playerName;

    [Header("Scale Selected Button")]
    private Button currSelectButton;
    private Vector2 selectedScale = new Vector2(1.2f, 1.2f);
    private Vector2 defaultScale = new Vector2(1f, 1f);

    private void Start()
    {
        StopFlickering(); // refresh the flickering

        PhotonNetwork.JoinLobby(); // auto join lobby as the scene load
        eventSystem.firstSelectedGameObject = choosePanelButton;
        currSelectButton = choosePanelButton.GetComponent<Button>();

        // display player name
        playerName.text = "";

        //lock the cursor 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        errorChoosePanelTxt.text = null;
    }

    private void Update()
    {
        if (currSelectButton != null)
        {
            RectTransform rectTransform = currSelectButton.GetComponent<RectTransform>();
            rectTransform.localScale = defaultScale;
        }

        // Update currently selected button
        currSelectButton = GetCurrentlySelectedButton();

        // Scale the currently selected button
        if (currSelectButton != null)
        {
            RectTransform rectTransform = currSelectButton.GetComponent<RectTransform>();
            rectTransform.localScale = selectedScale;
        }
    }

    private Button GetCurrentlySelectedButton()
    {
        GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;

        // Check if the selected GameObject has a Button component
        if (selectedGameObject != null)
        {
            Button selectedButton = selectedGameObject.GetComponent<Button>();
            if (selectedButton != null)
            {
                return selectedButton;
            }
        }

        return null;
    }

    // Random a room number (dont let player to create a room name), it is a room with xxxx (0<x<5)
    public string roomRandom()
    {
        string roomNumber = "";
        Random random = new Random();
        for (int i = 0; i < 4; i++)
        {
            int digit = random.Next(1, 5);
            roomNumber += digit.ToString();
        }

        return roomNumber;
    }

    // Create a room with maximum 2 player
    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            roomOptions.MaxPlayers = 2;
            roomOptions.IsOpen = true;
            string roomNum = roomRandom(); // after random a room code
            Debug.Log("Creating room: " + roomNum);
            PhotonNetwork.CreateRoom(roomNum, roomOptions, TypedLobby.Default); // enter the room
        } else
        {
            errorChoosePanelTxt.text = "There are some errors with the server!";
        }
        
    }

    // enter the lobby_dual after create a room successfully
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    // Click join to open the canvas enter the room code
    public void OnClickJoinRoom()
    {
        // set size of btn to default
        currSelectButton = GetCurrentlySelectedButton();
        RectTransform rectTransform = currSelectButton.GetComponent<RectTransform>();
        rectTransform.localScale = defaultScale;

        optionSelectScreen.SetActive(false);
        joinRoomScreen.SetActive(true);
        eventSystem.SetSelectedGameObject(joinPanelButton);
    }

    // after enter the text in canvas joinRoom, click enter to enter the room
    public void EnterRoomNum()
    {
        string roomEnter = roomNameJoin.text;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (string.IsNullOrEmpty(roomEnter) || roomEnter.Trim().Length < 4)
            {
                DisplayErrorText("Please enter a valid room with 4 digits.");
            }
            else 
            {
                PhotonNetwork.JoinRoom(roomEnter);
            }   
        }
        else
        {
            DisplayErrorText("Not connected to the server!");
        }

    }

    // Call if enter a room fail
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == Photon.Realtime.ErrorCode.GameDoesNotExist)
        {
            DisplayErrorText("The room does not exist!");
        }
        else
        {
            // Handle other join room failure cases
            DisplayErrorText("Failed to join the room: " + message);
        }
    }


    // this is for choosing number for the room code to enter 
    public void ClickRoomNumber(TMP_Text buttonText)
    {
        if (roomNameJoin.text.Length < 4)
        {
            roomNameJoin.text += buttonText.text;
        }
    }

    // this is for deleting if the player enter wrong number
    public void DeleteRoomNumber()
    {
        // Get the current text from the input field
        string currentText = roomNameJoin.text;

        // Check if the text is not empty
        if (!string.IsNullOrEmpty(currentText))
        {
            // Remove the last character from the text
            roomNameJoin.text = currentText.Substring(0, 0);
        }
    }

    public void OnBacModeScene()
    {
        SceneManager.LoadScene("PlayMode");
    }

    // return the chooseOptionPanel
    public void OnClickBackPanel()
    {
        // refresh data
        roomNameJoin.text = "";
        errorText.text = "";

        // change scene
        optionSelectScreen.SetActive(true);
        joinRoomScreen.SetActive(false);

        // set the zoom btn
        eventSystem.SetSelectedGameObject(choosePanelButton);
        currSelectButton = choosePanelButton.GetComponent<Button>();
    }


    // fuction for the flicker of error text
    public void DisplayErrorText(string errorMessage)
    {
        errorText.text = errorMessage;

        if (!isFlickering)
        {
            StartFlickering();
        }
    }

    private void StartFlickering()
    {
        isFlickering = true;
        StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            errorText.enabled = !errorText.enabled;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval;
        }

        errorText.enabled = true; // Ensure the error text is visible after flickering stops
        errorText.text = ""; // set it back to null when the flicker stop
        isFlickering = false;
    }

    // call in start to refresh 
    public void StopFlickering()
    {
        StopAllCoroutines();
        errorText.enabled = true; // Ensure the error text is permanently visible
        isFlickering = false;
    }

}
