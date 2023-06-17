using TMPro;
using System.Text;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public RoomOptions roomOptions = new RoomOptions(); // new RoomOption to create a room

    private void Start()
    {
        PhotonNetwork.JoinLobby(); // auto join lobby as the scene load

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
        roomOptions.MaxPlayers = 2;
        string roomNum = roomRandom(); // after random a room code
        Debug.Log("Creating room: " + roomNum); 
        PhotonNetwork.CreateRoom(roomNum, roomOptions, TypedLobby.Default); // enter the room
    }

    // Click join to open the canvas enter the room code
    public void OnClickJoinRoom()
    {
        optionSelectScreen.SetActive(false);
        joinRoomScreen.SetActive(true);
    }

    // enter the lobby_dual after create a room successfully
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Lobby_Dual");
    }

    // after enter the text in canvas joinRoom, click enter to enter the room
    public void EnterRoomNum()
    {
        PhotonNetwork.JoinRoom(roomNameJoin.text);
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
            // Create a StringBuilder from the current text
            StringBuilder stringBuilder = new StringBuilder(currentText);

            // Remove the last character from the StringBuilder
            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            // Update the text in the input field
            roomNameJoin.text = stringBuilder.ToString();
        }
    }
}
