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
            Debug.Log("start1");
            LoadLevelBtn("Lobby");
            Debug.Log("end");
        }
    }

    public void LoadLevelBtn(string levelToLoad)
    {
        Debug.Log("start2");
        if (!isLoading)
        {
            Debug.Log("start3");
            isLoading = true;
            mainMenu.SetActive(false);
            loadingScene.SetActive(true);
            Debug.Log("start4");
            StartCoroutine(LoadLevelAsync(levelToLoad));
        }
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        Debug.Log("start5");
        while (!loadOperation.isDone)
        {
            Debug.Log("start6");
            float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            loadingSlider.value = progress;
            Debug.Log("start7");
            yield return new WaitForSeconds(10.0f);
        }
        Debug.Log("start8");
        isLoading = false;
    }
}
