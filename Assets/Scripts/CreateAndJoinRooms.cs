using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField roomNameCreate;
    public InputField roomNameJoin;
    public RoomOptions roomOptions = new RoomOptions();


    public void CreateRoom()
    {
        roomOptions.MaxPlayers = 2;
        Debug.Log("Creating room" + roomNameCreate.text);
        PhotonNetwork.CreateRoom(roomNameCreate.text, roomOptions, TypedLobby.Default);
    }
    public void JoinRoom()
    {
        Debug.Log("Joined room" + roomNameJoin.text);
        PhotonNetwork.JoinRoom(roomNameJoin.text);
    }
    
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }

}
