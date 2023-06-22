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

    private void Start()
    {
        // Display the room name and default map name
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }


    public void ConfirmSelection()
    {
        // Transfer back to the room
        PhotonNetwork.LoadLevel("Game");
    }
}
