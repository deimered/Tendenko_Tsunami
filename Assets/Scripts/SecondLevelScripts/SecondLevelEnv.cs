using System.Collections;
using UnityEngine;
using System;
using StarterAssets;
using UnityEngine.Playables;

public class SecondLevelEnv : MonoBehaviour
{
    public Transform objective;
    public GameObject volume;

    CollectItens collect;
    ThirdPersonController player;
    UIControlerSecondLevel level;
    UIPopUpWindowControler popUp;
    Waves waves;
    UIGameEndControler score;
    Waypoint waypoint;
    GameManager gameManager;
    UIPauseControler pause;
    UIHelpControler helpUi;
    CityGenerator cityGenerator;
    ApplyForceToRigidBody body;
    PlayableDirector playable;
    Canvas canvas;

    private bool endLevel = false;
    private bool isSaving = false;
    private float timeToEnd = 1.5f;

    private void Awake()
    {
        collect = GetComponentInChildren<CollectItens>();
        player = GetComponentInChildren<ThirdPersonController>();
        level = GetComponentInChildren<UIControlerSecondLevel>();
        helpUi = GetComponentInChildren<UIHelpControler>();
        waves = GetComponentInChildren<Waves>();
        popUp = GetComponentInChildren<UIPopUpWindowControler>();
        waypoint = GetComponentInChildren<Waypoint>();
        pause = GetComponentInChildren<UIPauseControler>();
        body = GetComponentInChildren<ApplyForceToRigidBody>();
        cityGenerator = GetComponentInChildren<CityGenerator>();
        playable = GetComponentInChildren<PlayableDirector>();
        canvas = GetComponentInChildren<Canvas>();
        gameManager = FindObjectOfType<GameManager>();
        score = FindObjectOfType<UIGameEndControler>();

        if (cityGenerator != null && player != null && level != null && pause != null && waypoint != null)
        {
            level.Deactivate();
            player.Deactivate();
            player.gameObject.SetActive(false);
            pause.PauseMusic();
            StartCoroutine(ReadyLevel());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player.OnSaving += HandleOnSaving;
        waves.OnReachPoint += HandleOnDeath;
        level.OnTsunamiFlag += HandleTsunamiFlag;
        level.OnHelpBarFullFlag += HandleHelpBarFull;
        waypoint.OnEnter += HandlePopUp;
        popUp.OnConfirm += HandleCompleteLevel;
        popUp.OnPause += HandleClosePopUp;
        pause.OnExit += HandleExit;
    }

    private void Update()
    {
        if (volume != null)
            volume.SetActive(waves.GetHeight(Camera.main.transform.position) >= Camera.main.transform.position.y);
    }

    void HandleOnDeath(object sender, EventArgs e)
    {
        player.Deactivate();
        body.EnableRagdoll();
        
        if (!endLevel)
        {
            endLevel = true;
            StartCoroutine(EndLevel());
        }
            
    }

    void HandleCompleteLevel(object sender, EventArgs e)
    {
        //level.Deactivate();
        popUp.Deactivate();
        if (!endLevel)
        {
            endLevel = true;
            player.Deactivate();
            Vector3 dist = (waypoint.gameObject.transform.position - player.transform.position).normalized;
            player.transform.localRotation = Quaternion.Euler(player.transform.localRotation.eulerAngles + (Vector3.up * Vector3.SignedAngle(player.transform.forward, dist, Vector3.up)));
            playable.Play();
        }
    }

    void HandleTsunamiFlag(object sender, EventArgs e)
    {
        waves.ActivateTsunami();
    }

    void HandleExit(object sender, EventArgs e)
    {
        if (gameManager != null)
            gameManager.ResetLoadLevel("MainMenu");
    }

    void HandleHelpBarFull(object sender, EventArgs e)
    {
        collect.SavePerson();
        player.EndSaving();
        isSaving = false;
    }

    void HandleOnSaving(object sender, EventArgs e)
    {
        if (!isSaving)
            StartCoroutine(level.HelpBar());
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

    private IEnumerator ReadyLevel()
    {
        if (gameManager != null)
            gameManager.isReady = false;

        while (!cityGenerator.IsDone) { yield return null; }

        yield return new WaitForSeconds(1f);

        if (gameManager != null)
            gameManager.isReady = true;

        waypoint.transform.localPosition = new Vector3(cityGenerator.ExitPos.x, waypoint.transform.localPosition.y, cityGenerator.ExitPos.z);
        waypoint.SetBoxSize(new Vector3(cityGenerator.TileSize, waypoint.transform.localScale.y, cityGenerator.TileSize));
        player.gameObject.transform.parent.localPosition = new Vector3(cityGenerator.StartPos.x, player.gameObject.transform.parent.localPosition.y, cityGenerator.StartPos.z);
        player.gameObject.SetActive(true);

        RunnerAgent[] agents = GetComponentsInChildren<RunnerAgent>();
        foreach (RunnerAgent agent in agents) { agent.objective = objective; }

        while (gameManager != null && !gameManager.IsLoadingReady) { yield return null; }

        player.Activate();
        level.Activate();
        pause.StartMusic();
    }

    private IEnumerator EndLevel()
    {
        float seconds = 0f;

        while(seconds < timeToEnd)
        {
            seconds += Time.deltaTime;
            body.ApplyForce(waves.GetTsunamiSpeed(waves.FloatPoint.position), ForceMode.VelocityChange);
            yield return null;
        }

        level.Deactivate();
        if (score != null)
        {
            score.SetDeathCause("You weren't able to escape the city in time");
            score.AddScoreText("Time left", (int)(1000 * Timer.GetTimePercentage() / 100));
            score.TotalScore();
            score.Activate();
        }
    }

    public void HideElements()
    {
        level.HideUI();
        helpUi.HideUI();
        canvas.gameObject.SetActive(false);
        waypoint.gameObject.SetActive(false);
        Vector3 dist = (waypoint.gameObject.transform.position - player.transform.position).normalized;
        player.transform.rotation = Quaternion.Euler(Vector3.up * Vector3.Angle(player.transform.forward, dist));
    }

    public void NextLevel()
    {
        if (score != null)
        {
            score.AddScoreText("Time left", (int)(1000 * Timer.GetTimePercentage() / 100));
            score.AddScoreText("Completed", 1000);
            score.NextLevel();
            if (gameManager != null)
                gameManager.LoadLevelWithWaiting("ThirdLevel");
        }
    }
 }
