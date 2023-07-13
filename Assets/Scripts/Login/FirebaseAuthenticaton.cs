using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
//using Firebase.Database;

public class FirebaseAuthenticaton : MonoBehaviour
{
    [Header("Panel")]
    public GameObject loginPanel;
    public GameObject RegisterPanel;

    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;
    //public DatabaseReference DBreference;


    // Login Variables
    [Space]
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public ErrorPopup errorPopup;


    private void Start()
    {
        ClearFields();
    }

    private void Awake()
    {
        // Check that all of the necessary dependencies for firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    //Clear the login feilds
    public void ClearFields()
    {
        nameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        confirmPasswordRegisterField.text = "";
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    void InitializeFirebase()
    {
        //Set the default instance object
        auth = FirebaseAuth.DefaultInstance;

        //auth.StateChanged += AuthStateChanged;
        //AuthStateChanged(this, null);
        //DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                auth.SignOut();
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void Login()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failedMessage = "Login Failed! Because ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    errorPopup.ShowPopup("Email is invalid! ");
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong Password";
                    errorPopup.ShowPopup("Wrong Password! ");
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    errorPopup.ShowPopup("Email is missing! ");
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    errorPopup.ShowPopup("Password is missing! ");
                    break;
                case AuthError.UserNotFound:
                    failedMessage = "Account does not exist";
                    errorPopup.ShowPopup("User not found ");
                    break;
                default:
                    errorPopup.ShowPopup("Something was wrong... ");
                    break;
            }
            Debug.Log(failedMessage);
        }
        else
        {
            // User is logged in now
            user = loginTask.Result.User;

            Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
            
            //UnityEngine.SceneManagement.SceneManager.LoadScene("PlayMode");
        }
    }

    public void Register()
    {
        //Call the register coroutine passing the email and password
        StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {

        if (email == "")
        {
            Debug.LogError("Email field is empty");
            errorPopup.ShowPopup("Email field is empty! ");

        }
        else if (name == "")
        {
            Debug.LogError("User Name is empty");
            errorPopup.ShowPopup("User Name is empty! ");

        }
        else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Password does not match");
            errorPopup.ShowPopup("Password does not match! ");
        }

        else if (confirmPasswordRegisterField.text != confirmPasswordRegisterField.text)
        {
            Debug.LogError("Password does not match");
            errorPopup.ShowPopup("Password does not match! ");
        }

        else
        {
            var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                Debug.LogError(registerTask.Exception);

                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;

                string failedMessage = "Registration Failed! Because ";
                switch (authError)
                {
                    case AuthError.InvalidEmail:
                        failedMessage += "Email is invalid";
                        errorPopup.ShowPopup("Email is invalid! ");
                        break;
                    case AuthError.WrongPassword:
                        failedMessage += "Wrong Password";
                        errorPopup.ShowPopup("Password must be longer than 6 characters! ");
                        break;
                    case AuthError.MissingEmail:
                        failedMessage += "Email is missing";
                        errorPopup.ShowPopup("Email is missing! ");
                        break;
                    case AuthError.MissingPassword:
                        failedMessage += "Password is missing";
                        errorPopup.ShowPopup("Password is missing! ");
                        break;
                    case AuthError.EmailAlreadyInUse:
                        failedMessage = "Email Already In Use";
                        errorPopup.ShowPopup("Email Already In Use! ");
                        break;
                    default:
                        errorPopup.ShowPopup("Password must be more than 6 characters! ");
                        break;
                }

                Debug.Log(failedMessage);
            }
            else
            {
                // Get The User After Registration Success
                user = registerTask.Result.User;

                UserProfile userProfile = new UserProfile { DisplayName = name };

                var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                if (updateProfileTask.Exception != null)
                {
                    // Delete the user if user update failed
                    user.DeleteAsync();

                    Debug.LogError(updateProfileTask.Exception);

                    FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;


                    string failedMessage = "Profile update Failed! Becuase ";
                    switch (authError)
                    {
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            errorPopup.ShowPopup("Email is invalid! ");
                            break;
                        case AuthError.WrongPassword:
                            failedMessage += "Wrong Password";
                            errorPopup.ShowPopup("Password must be longer than 6 characters! ");
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            errorPopup.ShowPopup("Email is missing! ");
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            errorPopup.ShowPopup("Password is missing! ");
                            break;
                        case AuthError.EmailAlreadyInUse:
                            failedMessage = "Email Already In Use";
                            errorPopup.ShowPopup("Email Already In Use! ");
                            break;
                        default:
                            errorPopup.ShowPopup("Password must be more than 6 characters! ");
                            break;
                    }

                    Debug.Log(failedMessage);
                }
                else
                {
                    Debug.Log("Registration Sucessful Welcome " + user.DisplayName);
                    RegisterPanel.SetActive(false);
                    loginPanel.SetActive(true);
                }
            }
        }
    }
    
}
