// using Photon.Pun;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;

// public class ConnectToServer : MonoBehaviourPunCallbacks
// {
//     private bool connectedToMaster = false;

//     // Start is called before the first frame update
//     void Start()
//     {
//         PhotonNetwork.ConnectUsingSettings();
//     }

//     public override void OnConnectedToMaster()
//     {
//         connectedToMaster = true;
//         PhotonNetwork.JoinLobby();
//     }

//     public void OnButtonClick()
//     {
//         if (connectedToMaster)
//         {
//             AsyncManager.Instance.LoadLevelBtn("Lobby");
//         }
//     }
// }
