using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;
using UnityEngine.Playables;

public class FirstLevelEnv : MonoBehaviour
{
    Animator animator;
    UIControlerFirstLevel level;
    UIHelpControler helpUI;
    UIGameEndControler score;
    UIPopUpWindowControler popUp;
    UIPauseControler pause;
    Waypoint waypoint;
    GameManager gameManager;
    Canvas canvas;
    CameraShaker shaker;
    PlayableDirector[] playables;

    [SerializeField]
    private ItensManagerScriptableObject itensManager;
    [SerializeField]
    private CollectibleItemScriptableObject[] importantResourses;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        level = GetComponentInChildren<UIControlerFirstLevel>();
        helpUI = GetComponentInChildren<UIHelpControler>();
        waypoint = GetComponentInChildren<Waypoint>();
        popUp = GetComponentInChildren<UIPopUpWindowControler>();
        pause = GetComponentInChildren<UIPauseControler>();
        shaker = GetComponentInChildren<CameraShaker>();
        gameManager = FindObjectOfType<GameManager>();
        canvas = GetComponentInChildren<Canvas>();
        score = FindObjectOfType<UIGameEndControler>();
        playables = FindObjectsOfType<PlayableDirector>();

        canvas.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        level.Deactivate();
        level.HideUI();

        helpUI.HideUI();

        level.OnGameOver += HandleGameOver;
        level.OnReload += HandleReloadLevel;
        waypoint.OnEnter += HandlePopUp;
        popUp.OnConfirm += HandleCompleteLevel;
        popUp.OnPause += HandleClosePopUp;
        pause.OnExit += HandleExit;
        StartCoroutine(ReadyLevel());
    }

    void HandleCompleteLevel(object sender, EventArgs e)
    {
        level.Deactivate();
        popUp.Deactivate();
        if(score != null)
        {
            LevelPoints();
            score.AddScoreText("Completed", 1000);
            score.NextLevel();
        }
        if (gameManager != null)
            gameManager.LoadLevelWithWaiting("SecondLevel");
    }

    void HandleGameOver(object sender, EventArgs e)
    {
        level.Deactivate();
        if(score != null)
        {
            LevelPoints();
            score.SetDeathCause("You were too slow. The tsunami hit you before you could leave the house");
            score.TotalScore();
            score.Activate();
        }
    }

    void HandlePopUp(object sender, EventArgs e)
    {
        level.Deactivate();
        popUp.Activate();
    }

    void HandleClosePopUp(object sender, EventArgs e)
    {
        popUp.Deactivate();
        level.Activate();
    }

    void HandleReloadLevel(object sender, EventArgs e)
    {
        if (gameManager != null)
        {
            canvas.gameObject.SetActive(false);
            gameManager.LoadLevel(SceneManager.GetActiveScene().name);
        }
            
    }

    void HandleExit(object sender, EventArgs e)
    {
        if (gameManager != null)
            gameManager.ResetLoadLevel("MainMenu");
    }

    void LevelPoints()
    {
        if (itensManager != null)
        {
            int resourcesScore = 0;
            foreach (CollectibleItemScriptableObject cso in itensManager.GetCollectibleItens())
                if (importantResourses.Contains(cso))
                    resourcesScore += 100;
            score.AddScoreText("Resources of the emergency kit", resourcesScore);
        }

        score.AddScoreText("Time left", (int)(1000 * Timer.GetTimePercentage() / 100));
    }

    public void StartMusic()
    {
        pause.StartMusic();
    }

    public void ShowUIControls()
    {
        animator.applyRootMotion = false;
        level.ShowUI();
        level.Activate();
        helpUI.ShowUI();
        canvas.gameObject.SetActive(true);
    }

    public void ShakeCamera()
    {
        StartCoroutine(shaker.Shake(2.5f));
    }

    IEnumerator ReadyLevel()
    {
        while (gameManager != null && !gameManager.IsLoadingReady)
        {
            yield return null;
        }

        foreach(PlayableDirector playable in playables)
            playable.Play(); 
    }
}
