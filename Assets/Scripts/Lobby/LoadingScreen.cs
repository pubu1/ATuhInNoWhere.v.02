using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private float minimumLoadingTime = 2f;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text loading;

    private float elapsedTime = 0f;
    public float duration = 2f;

    private bool isLoadingComplete = false;
    private bool isPlayersReady = false;

    private void Start()
    {
        //StartCoroutine(LoadGameSceneAsync(gameSceneName));
        StartCoroutine(LoadText());
        StartCoroutine(Load2s(gameSceneName));
    }


    // check the time for loading
    private IEnumerator LoadGameSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // Normalize progress to 0-1 range
            progressBar.value = progress;

            yield return null;
        }
    }

    private IEnumerator LoadText()
    {
        int dotCount = 0;
        string loadingTextBase = "Loading";
        string dots = "";

        while (true)
        {
            if (dotCount < 3)
            {
                dots += ".";
                dotCount++;
            }
            else
            {
                dots = "";
                dotCount = 0;
            }

            loading.text = loadingTextBase + dots;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator Load2s( string sceneName)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            progressBar.value = progress;

            yield return null;
        }

        progressBar.value = 1f;
        SceneManager.LoadSceneAsync(sceneName);
    }



    /*private IEnumerator StartGame()
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

    public void SetPlayersReady(bool ready)
    {
        isPlayersReady = ready;
    }*/
}
