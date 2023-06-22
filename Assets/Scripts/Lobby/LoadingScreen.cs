using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private float minimumLoadingTime = 2f;
    [SerializeField] private Slider progressBar;

    private bool isLoadingComplete = false;
    private bool isPlayersReady = false;

    private void Start()
    {
        StartCoroutine(LoadGameSceneAsync());
    }

    private IEnumerator LoadGameSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(gameSceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize progress to 0-1 range
            progressBar.value = progress;

            if (progress >= 1f)
            {
                isLoadingComplete = true;
            }

            if (isLoadingComplete && isPlayersReady)
            {
                operation.allowSceneActivation = true; // Activate the loaded scene
            }

            yield return null;
        }
    }

    private IEnumerator StartGame()
    {
        // Simulate minimum loading time
        yield return new WaitForSeconds(minimumLoadingTime);

        // Notify the server or game manager that the player is ready
        NotifyPlayerReady();

        // Check if all players are ready
        while (!isPlayersReady)
        {
            yield return null;
        }

        // Start the game
        SceneManager.LoadScene(gameSceneName);
    }

    private void NotifyPlayerReady()
    {
        // Replace this with your own implementation to notify the server or game manager that the player is ready
        // You can use networking APIs or custom messaging system to send the notification
        // Example: server.NotifyPlayerReady(playerID);
    }

    private void Update()
    {
        if (isLoadingComplete && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(StartGame());
        }
    }

    public void SetPlayersReady(bool ready)
    {
        isPlayersReady = ready;
    }
}
