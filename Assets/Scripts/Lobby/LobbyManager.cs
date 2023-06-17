using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text roomNameText;
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
