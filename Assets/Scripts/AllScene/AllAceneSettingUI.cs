using Firebase.Auth;
using Firebase;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AllAceneSettingUI : MonoBehaviour
{
    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Panel")]
    [SerializeField]
    private GameObject panel;

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void OnClickBackToPlayMode()
    {
        SceneManager.LoadScene("PlayMode");
    }

    //Logout Method
    public void LogOut()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
            Debug.Log("Signed out " + user.UserId);
        }
        SceneManager.LoadScene("Login");
    }


}
