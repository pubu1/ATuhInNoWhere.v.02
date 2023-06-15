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
    private GameObject choosingScreen;

    [SerializeField]
    private GameObject joinScreen;

    [Header("Room")]
    [SerializeField]
    private TMP_InputField roomNameJoin;

    [SerializeField]
    private TMP_Text errorText;

    // [SerializeField]
    // private Button btn;

    // private Color disabledColor  = new Color(1f, 1f, 1f, 0.5f);

    public RoomOptions roomOptions = new RoomOptions();

    public void Start()
    {
        // var textComponent = roomNameJoin.textComponent;
        // var backgroundComponent = roomNameJoin.GetComponent<Image>();

        // textComponent.color = disabledColor;
        // backgroundComponent.color = disabledColor;
    }

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

    public void CreateRoom()
    {
        roomOptions.MaxPlayers = 2;
        string roomNum = roomRandom();
        Debug.Log("Creating room: " + roomNum);
        PhotonNetwork.CreateRoom(roomNum, roomOptions, TypedLobby.Default);
    }

    public void JoinRoom()
    {
        choosingScreen.SetActive(false);
        joinScreen.SetActive(true);
    }

    public void EnterRoomNum()
    {
        Debug.Log("Joined room" + roomNameJoin.text);
        PhotonNetwork.JoinRoom(roomNameJoin.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

    public void ClickRoomNumber(string buttonText)
    {
        if (roomNameJoin.text.Length < 4)
        {
            roomNameJoin.text += buttonText;
        }
    }

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
