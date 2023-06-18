using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegisterUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField nicknameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField confirmPasswordInput;
    public Button registerButton;
    public ErrorPopup errorPopup;
    [SerializeField]
    private GameObject registerPanel;

    [SerializeField]
    private GameObject loginPanel;

    private void Start()
    {
        registerButton.onClick.AddListener(Register);
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
    }

    public void Register()
    {
        string username = usernameInput.text;
        string nickname = nicknameInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(nickname) &&
            !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
        {
            // Kiểm tra xem tài khoản đã tồn tại hay chưa
            bool accountExists = CheckAccountExistence(username);

            if (accountExists)
            {
                // Hiển thị thông báo lỗi nếu tài khoản đã tồn tại
                Debug.Log("Tài khoản đã tồn tại!");
                errorPopup.ShowPopup("Username is existed!");

            }
            else if (password == confirmPassword)
            {
                // Lưu thông tin đăng ký vào PlayerPrefs
                PlayerPrefs.SetString("Username", username);
                PlayerPrefs.SetString("Nickname", nickname);
                PlayerPrefs.SetString("Password", password);

                // Đăng ký thành công
                Debug.Log("Đăng ký thành công. Tên người dùng: " + username + ", Nickname: " + nickname);
                registerPanel.SetActive(false);
                loginPanel.SetActive(true);
            }
            else
            {
                // Hiển thị thông báo lỗi nếu mật khẩu không khớp
                Debug.Log("Mật khẩu và xác nhận mật khẩu không khớp!");
                errorPopup.ShowPopup("Password and confirm password do not match!");

            }
        }
        else
        {
            // Hiển thị thông báo lỗi nếu có trường đầu vào bị để trống
            Debug.Log("Vui lòng điền đầy đủ thông tin!");
            errorPopup.ShowPopup("Please fill in all the information!");

        }
    }

    private bool CheckAccountExistence(string username)
    {
        // Kiểm tra xem tài khoản có tồn tại trong PlayerPrefs hay không
        string savedUsername = PlayerPrefs.GetString("Username");

        return (username == savedUsername);
    }
}
