using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenuManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject settings;
    public GameObject shop;

    private UIMainMenu menuControler;
    private UIPauseControler settingsControler;
    private UIShopControler shopControler;

    private GameManager gameManager;

    private void Awake()
    {
        Time.timeScale = 1;
        menuControler = menu.GetComponent<UIMainMenu>();
        settingsControler = settings.GetComponent<UIPauseControler>();
        shopControler = shop.GetComponent<UIShopControler>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        menuControler.OnStart += HandleStart;
        menuControler.OnPause += HandlePause;
        menuControler.OnShop += HandleShop;
        settingsControler.OnBack += HandleSettingsBack;
        shopControler.OnPause += HandleShopBack;
    }

    private void HandleStart(object sender, EventArgs e)
    {
        if(gameManager != null)
            //gameManager.LoadLevel("FirstLevel");
        gameManager.LoadLevelWithWaiting("FirstLevel");
    }

    private void HandlePause(object sender, EventArgs e)
    {
        menuControler.Deactivate();
        settingsControler.Activate();
    }

    private void HandleSettingsBack(object sender, EventArgs e)
    {
        settingsControler.Deactivate();
        menuControler.Activate();
    }

    private void HandleShop(object sender, EventArgs e)
    {
        menuControler.Deactivate();
        shopControler.Activate();
    }

    private void HandleShopBack(object sender, EventArgs e)
    {
        shopControler.Deactivate();
        menuControler.Activate();
    }
}
