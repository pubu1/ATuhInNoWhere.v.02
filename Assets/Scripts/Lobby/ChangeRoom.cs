using TMPro;
using System.Text;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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

    [Header("Room List")]
    private RoomItem roomItem = new RoomItem();
    private List<RoomItem> roomItems = new List<RoomItem>();
    private bool existedRoom = false;

    [Header("Scale Selected Button")]
    private Button currSelectButton;
    private Vector2 selectedScale = new Vector2(1.2f, 1.2f);
    private Vector2 defaultScale = new Vector2(1f, 1f);

    private void Start()
    {
        roomItems.Clear();

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
            string roomNum = roomRandom(); // after random a room code
            Debug.Log("Creating room: " + roomNum);
            PhotonNetwork.CreateRoom(roomNum, roomOptions, TypedLobby.Default); // enter the room
        } else
        {
            errorChoosePanelTxt.text = "There are some errors with the server!";
        }
        
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

    // enter the lobby_dual after create a room successfully
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby_Dual");
    }

    // after enter the text in canvas joinRoom, click enter to enter the room
    public void EnterRoomNum()
    {
        string roomEnter = roomNameJoin.text;

        if (PhotonNetwork.IsConnectedAndReady)
        {
                
            /*CheckExisted();
            if (existedRoom)
            {
                // Room exists, join the room
                PhotonNetwork.JoinRoom(roomEnter);
            }
            else
            {
                // Room doesn't exist, show an error message
                errorText.text = "The room does not exist!";
            }*/
        }
        else
        {
            errorText.text = "Not connected to the server!";
        }

    }

    private void CheckExisted()
    {
        foreach (RoomItem roomItem in roomItems)
        {
            if (roomItem.roomName.Equals(roomNameJoin.text))
            {
                existedRoom = true;
                Debug.Log(existedRoom);
            }
            else
            {
                existedRoom = false;
                Debug.Log(existedRoom);
            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateRoomList(roomList);
    }

    private void UpdateRoomList(List<RoomInfo> roomList)
    {
        if (roomItems.Count > 0)
        {
            // Clear the existing room items
            foreach (RoomItem roomItem in roomItems)
            {
                Destroy(roomItem.gameObject);
            }
            roomItems.Clear();
        }
        else
        {
            Debug.Log("roomItem nothing");
        }

        if (roomList != null)
        {
            // Create new room items based on the room list
            foreach (RoomInfo roomInfo in roomList)
            {
                // Create a new room item using a prefab or instantiate it dynamically
                RoomItem newRoom = Instantiate(roomItem, transform);

                // Set the room item's properties based on the room info
                newRoom.name = roomInfo.Name;

                // Add the room item to the list
                roomItems.Add(newRoom);

                Debug.Log(newRoom.name);
            }
        } else
        {
            Debug.Log("there's nothing");
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

    public void OnClickBackPanel()
    {
        optionSelectScreen.SetActive(true);
        joinRoomScreen.SetActive(false);
        eventSystem.SetSelectedGameObject(choosePanelButton);
        currSelectButton = choosePanelButton.GetComponent<Button>();
    }

}
