using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncManager : MonoBehaviourPunCallbacks
{
    private bool connectedToMaster = false;

    [Header("Menu Screens")]
    [SerializeField]
    private GameObject loadingScene;

    [SerializeField]
    private GameObject mainMenu;

    [Header("Slider")]
    [SerializeField]
    private Slider loadingSlider;

    private bool isLoading = false;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectedToMaster = true;
        PhotonNetwork.JoinLobby();
    }

    public void OnButtonClick()
    {
        Debug.Log("start");
        if (connectedToMaster)
        {
            LoadLevelBtn("Lobby");
        }
    }

    public void LoadLevelBtn(string levelToLoad)
    {
        if (!isLoading)
        {
            isLoading = true;
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            StartCoroutine(LoadLevelAsync(levelToLoad));
        }
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            loadingSlider.value = progress;
            yield return new WaitForSeconds(10.0f);
        }
        isLoading = false;
    }
}
