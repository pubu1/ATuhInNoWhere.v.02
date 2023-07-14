using Photon.Pun;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Database;
using Firebase;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("Room Name")]
    [SerializeField] private TMP_Text roomNameText;

    // Firebase variable
    [Space]
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

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
