using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    public TMP_Text roomName;

    public void setRoomName(string _roomName)
    {
        roomName.text = _roomName;
    }
}
