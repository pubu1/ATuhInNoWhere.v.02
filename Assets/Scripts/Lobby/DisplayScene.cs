using TMPro;
using System.Text;
using Random = System.Random;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScene : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_Text roomNameText;

    private void Start()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }
}
