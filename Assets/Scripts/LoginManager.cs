using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviourPunCallbacks
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;

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
            // Kiểm tra xem tài khoản đã được đăng ký hay không
            bool accountExists = CheckAccountExistence(username, password);

            if (accountExists)
            {
                // Gửi thông tin đăng nhập lên máy chủ Photon
                PhotonNetwork.AuthValues = new AuthenticationValues(username);
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                // Hiển thị thông báo lỗi
                Debug.Log("Tài khoản không tồn tại!");
            }
        }
        else
        {
            // Hiển thị thông báo lỗi
            Debug.Log("Tên người dùng và mật khẩu không thể để trống!");
        }
    }

    private bool CheckAccountExistence(string username, string password)
    {
        // Kiểm tra xem tài khoản có tồn tại trong PlayerPrefs hay không
        string savedUsername = PlayerPrefs.GetString("Username");
        string savedPassword = PlayerPrefs.GetString("Password");

        return (username == savedUsername && password == savedPassword);
    }

    public override void OnConnectedToMaster()
    {
        // Đăng nhập thành công, tiếp tục vào phòng chơi hoặc thực hiện các hành động khác
        Debug.Log("Đăng nhập thành công");
        SceneManager.LoadScene("Loading");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // Xử lý khi bị ngắt kết nối
        Debug.Log("Ngắt kết nối: " + cause.ToString());
    }
}
