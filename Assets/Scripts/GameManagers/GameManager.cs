using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{

    public UILoadScreenControler loadScreen;
    public UIGameEndControler endScreen;
    public ItensManagerScriptableObject itensManager;
    public bool resetValue;
    //Flag ready controlada pelo level.
    public bool isReady = true;
    //Flag ready controlado pelo loading
    private bool isLoadingReady = true;

    public bool IsLoadingReady
    {
        get { return isLoadingReady; }
    }

    private void Awake()
    {
        if(resetValue)
            PlayerPrefs.DeleteAll();
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(loadScreen.gameObject);
        DontDestroyOnLoad(endScreen.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        RemoveItens();
        endScreen.OnPause += HandleReplay;
        endScreen.OnExit += HandleExit;
        loadScreen.OnAdvance += HandleOnAdvance;
        LoadLevelBlackScreen("MainMenu");
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadSceneAsync(levelName));
    }

    public void LoadLevelWithWaiting(string levelName)
    {
        isLoadingReady = false;
        StartCoroutine(LoadSceneAsync(levelName));
    }

    public void LoadLevelBlackScreen(string levelName)
    {
        loadScreen.DisableElements();
        StartCoroutine(LoadSceneAsync(levelName));
    }

    public void ResetLoadLevel(string levelName)
    {
        endScreen.ResetScore();
        RemoveItens();
        LoadLevel(levelName);
    }

    IEnumerator LoadSceneAsync(string levelName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

        loadScreen.Activate();

        while (!operation.isDone || !isReady || !isLoadingReady)
        {
            if (operation.isDone && isReady)
                loadScreen.LevelReady();
            yield return null;
        }
        Time.timeScale = 1;
        AudioListener.pause = false;
        loadScreen.Deactivate();
    }

    void HandleExit(object sender, EventArgs e)
    {
        ResetLoadLevel("MainMenu");
    }

    void HandleReplay(object sender, EventArgs e)
    {
        ResetLoadLevel("FirstLevel");
    }

    void HandleOnAdvance(object sender, EventArgs e)
    {
        LoadingIsReady();
    }

    private void RemoveItens()
    {
        itensManager.RemoveItens();
    }

    public void LoadingIsReady()
    {
        isLoadingReady = true;
    }
}
