using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    private bool connectedToMaster = false;

    private void Start()
    {
        loginButton.onClick.AddListener(Login);
    }

    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            // Check if the account exists
            bool accountExists = CheckAccountExistence(username, password);

            if (accountExists)
            {
                // Send login information to the Photon server
                PhotonNetwork.AuthValues = new AuthenticationValues(username);
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                // Display an error message
                Debug.Log("Tài khoản không tồn tại!");
            }
        }
        else
        {
            // Display an error message
            Debug.Log("Tên người dùng và mật khẩu không thể để trống!");
        }
    }

    private bool CheckAccountExistence(string username, string password)
    {
        // Check if the account exists in PlayerPrefs
        string savedUsername = PlayerPrefs.GetString("Username");
        string savedPassword = PlayerPrefs.GetString("Password");

        return (username == savedUsername && password == savedPassword);
    }

    public override void OnConnectedToMaster()
    {
        // Successful login, continue to the loading scene or perform other actions
        Debug.Log("Đăng nhập thành công");
        connectedToMaster = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (connectedToMaster)
        {
            if (PhotonNetwork.NickName != usernameInput.text)
            {
                PhotonNetwork.NickName = usernameInput.text;
            }

            SceneManager.LoadScene("PlayMode");
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Handle disconnection
        Debug.Log("Ngắt kết nối: " + cause.ToString());
    }
}
