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
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    public DatabaseReference accountsRef;
    FirebaseAuthenticaton firebaseAuth;

    [Header("Panel")]
    [SerializeField]
    private GameObject panel;

    [Header("User Name")]
    [SerializeField]
    private TMP_Text nickname;

    public void Start()
    {
        firebaseAuth = FirebaseAuthenticaton.GetInstance();
        if (firebaseAuth != null)
        {
            auth = firebaseAuth.auth;
            user = firebaseAuth.user;
        }
        nickname.text = PhotonNetwork.NickName;
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        auth.StateChanged += AuthStateChanged;
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                auth.SignOut();
                StartCoroutine(UpdateStatus(false));
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                nickname.text = PhotonNetwork.NickName;
            } else
            {
                nickname.text = "there's problem";
            }
        }
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
            SceneManager.LoadScene("Lobby");
        } else if (PhotonNetwork.OfflineMode && SceneManager.GetActiveScene().name == "Game") {
            SceneManager.LoadScene("Map");
        } else if (SceneManager.GetActiveScene().name == "Map" || SceneManager.GetActiveScene().name == "Lobby") 
        {
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
        if (auth != null && user != null)
        {
            auth.SignOut();
            StartCoroutine(UpdateStatus(false));
            Debug.Log("Signed out " + user.UserId);
        }
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Login");
    }

    private IEnumerator UpdateStatus(bool data)
    {
        //Set the currently logged in user deaths
        var DBTask = accountsRef.Child("Accounts").Child(user.UserId).Child("isActive").SetValueAsync(data);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }

    public void OnApplicationQuit()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
            StartCoroutine(UpdateStatus(false));
            Debug.Log("Signed out " + user.UserId);
        }
        Debug.Log("No one log in");
    }
}
