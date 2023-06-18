using UnityEngine;
using UnityEngine.UI;

public class RegisterUI : MonoBehaviour
{
    public InputField usernameInput;
    public InputField nicknameInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Button registerButton;

    private void Start()
    {
        registerButton.onClick.AddListener(Register);
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
            if (password == confirmPassword)
            {
                // Lưu thông tin đăng ký vào PlayerPrefs
                PlayerPrefs.SetString("Username", username);
                PlayerPrefs.SetString("Nickname", nickname);
                PlayerPrefs.SetString("Password", password);

                // Đăng ký thành công
                Debug.Log("Đăng ký thành công. Tên người dùng: " + username + ", Nickname: " + nickname);
            }
            else
            {
                // Hiển thị thông báo lỗi nếu mật khẩu không khớp
                Debug.Log("Mật khẩu và xác nhận mật khẩu không khớp!");
            }
        }
        else
        {
            // Hiển thị thông báo lỗi nếu có trường đầu vào bị để trống
            Debug.Log("Vui lòng điền đầy đủ thông tin!");
        }
    }
}
