using Firebase.Auth;
using Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;
using Firebase.Database;
using TMPro;

public class AllAceneSettingUI : MonoBehaviourPunCallbacks
{
    // Firebase variable
    [Header("Firebase")]
    [SerializeField]
    private DependencyStatus dependencyStatus;
    [SerializeField]
    private FirebaseAuth auth;
    [SerializeField]
    private FirebaseUser user;
    [SerializeField]
    FirebaseAuthenticaton firebaseAuth;
    private string acc_userID;

    [Header("Panel")]
    [SerializeField]
    private GameObject panel;

    [Header("User Name")]
    [SerializeField]
    private TMP_Text nickname;

    public void Start()
    {
        firebaseAuth = FirebaseAuthenticaton.GetInstance();
        firebaseAuth.InitializeFirebase();
        if (firebaseAuth != null)
        {
            auth = firebaseAuth.auth;
            user = firebaseAuth.user;
            acc_userID = firebaseAuth.acc_userID;
            nickname.text = PhotonNetwork.NickName;
        }
        Debug.Log("I'm in here "+acc_userID);
    }

    public void OnClickOpen()
    {
        panel.SetActive(true);
    }

    public void OnClickClose()
    {
        panel.SetActive(false);
    }

    public void OnClickBackToPrevious()
    {
        if (PhotonNetwork.IsConnected && SceneManager.GetActiveScene().name == "Game")
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        } else if (PhotonNetwork.OfflineMode && SceneManager.GetActiveScene().name == "Game") {
            SceneManager.LoadScene("Map");
        } else if (SceneManager.GetActiveScene().name == "Map" || SceneManager.GetActiveScene().name == "Lobby") 
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("PlayMode");
        } else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void OnClickBackToPlayMode()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("PlayMode");
    }

    //Logout Method
    public void LogOut()
    {
        if (acc_userID != null)
        {
            auth.SignOut();
            StartCoroutine(firebaseAuth.UpdateStatus(acc_userID, false));
            Debug.Log("Signed out " + acc_userID);
        }
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Login");
    }

    public void OnApplicationQuit()
    {
        if (acc_userID!=null)
        {
            auth.SignOut();
            StartCoroutine(firebaseAuth.UpdateStatus(acc_userID, false));
            Debug.Log("Signed out " + acc_userID);
        } else
        {
            Debug.Log("No one log in");
        }
    }
}
