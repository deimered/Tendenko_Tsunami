using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ThirdLevelEnv : MonoBehaviour
{
    EventManager eventManager;
    UIControlerThirdLevel ui;
    UIGameEndControler score;
    UIPauseControler pause;
    GameManager gameManager;
    [SerializeField]
    ItensManagerScriptableObject itensManager;

    void Awake()
    {
        eventManager = GetComponentInChildren<EventManager>();
        ui = GetComponentInChildren<UIControlerThirdLevel>();
        pause = GetComponentInChildren<UIPauseControler>();
        gameManager = FindObjectOfType<GameManager>();
        score = FindObjectOfType<UIGameEndControler>();    
    }

    void Start()
    {
        ui.OnNextEvent += HandleNextChallenge;
        ui.OnUseItemEvent += HandleUseItem;
        ui.OnIgnoreEvent += HandleIgnoreChallenge;
        eventManager.OnDeath += HandleDeath;
        eventManager.OnGameEnd += HandleGameEnd;
        eventManager.OnLoseLife += HandleLoseLife;
        eventManager.OnLoseHunger += HandleLoseHunger;
        eventManager.OnLoseThirst += HandleLoseHydration;
        eventManager.OnLoseHygiene += HandleLoseHygiene;
        pause.OnExit += HandleExit;

        StartCoroutine(ReadyLevel());
    }

    void HandleNextChallenge(object sender, EventArgs e)
    {
        eventManager.NextEvent();
        ui.SetEventDetails(eventManager.GetCurrentEventChallenge(), eventManager.GetAllowedItens(itensManager.GetCollectibleItens()));
    }

    void HandleUseItem(object sender, EventArgs e)
    {
        CollectibleItemScriptableObject item = eventManager.GetAllowedItens(itensManager.GetCollectibleItens())[ui.GetLastButtonClickedIndex()];
        item.LoseDurability();
        if (item.Durability == 0)
            itensManager.RemoveItem(item);

        eventManager.IsSuccess(item);
        ui.SetEventDetails(eventManager.GetResultMessage(item), null);
    }

    void HandleIgnoreChallenge(object sender, EventArgs e)
    {
        eventManager.IsIgnoreEventSuccess();
        ui.SetEventDetails(eventManager.GetIgnoreEventText(), null);
    }

    void HandleDeath(object sender, EventArgs e)
    {
        ui.Deactivate();
        if(score != null)
        {
            score.AddScoreText("Days alive " + "(" + (eventManager.CurrentRound - 1) + ")", (eventManager.CurrentRound - 1) * 500);
            score.NextLevel();
            score.TotalScore();
            score.Activate();
        }
    }

    void HandleGameEnd(object sender, EventArgs e)
    {
        ui.Deactivate();
        if(score != null)
        {
            score.AddScoreText("Days alive " + "(" + (eventManager.CurrentRound - 1) + ")", (eventManager.CurrentRound - 1) * 500);
            score.AddScoreText("Completed", 1000);
            score.NextLevel();
            score.TotalScore();
            score.DisplayWinText();
            score.Activate();
        }  
    }

    void HandleExit(object sender, EventArgs e)
    {
        if (gameManager != null)
            gameManager.ResetLoadLevel("MainMenu");
    }

    void HandleLoseLife(object sender, EventArgs e)
    {
        ui.LoseLife();
        if(score != null)
            score.SetDeathCause("You have succumbed to your injuries");
    }

    void HandleLoseHunger(object sender, EventArgs e)
    {
        ui.LoseHunger();
        if (score != null)
            score.SetDeathCause("You have died from hunger");
    }

    void HandleLoseHydration(object sender, EventArgs e)
    {
        ui.LoseHydration();
        if (score != null)
            score.SetDeathCause("You have died from thirst");
    }

    void HandleLoseHygiene(object sender, EventArgs e)
    {
        ui.LoseHygiene();
        if (score != null)
            score.SetDeathCause("You failed to maintain your hygiene and died of illness");
    }

    IEnumerator ReadyLevel()
    {
        while (gameManager != null && !gameManager.IsLoadingReady)
        {
            yield return null;
        }

        pause.StartMusic();
    }
}
