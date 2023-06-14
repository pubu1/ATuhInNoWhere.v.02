using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginBtn : MonoBehaviour
{
    public GameObject loginUIPopup;

    public void OpenLoginUIPopup()
    {
        loginUIPopup.SetActive(true);
    }
}
