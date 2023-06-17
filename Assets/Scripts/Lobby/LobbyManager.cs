using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room Name")]
    [SerializeField] private TMP_Text roomNameText;

    /*[Header("Map Name")]
    [SerializeField] private List<string> mapList = new List<string>();
    private string mapFolderPath = "CustomMap/"; // Replace with the actual folder path
    private string[] mapFiles = Directory.GetFiles(mapFolderPath, "*.txt");*/

    [SerializeField] private TMP_Text mapNameText;

    private void Start()
    {
        // Display the room name and default map name
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        mapNameText.text = "Default Map";
    }



    public void SelectMap(string mapName)
    {
        // Set the selected map name
        mapNameText.text = mapName;
    }

    public void ConfirmSelection()
    {
        // Transfer back to the room
        PhotonNetwork.LoadLevel("Game");
    }
}
