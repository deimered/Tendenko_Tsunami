using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour, IUIControler
{

    private VisualElement root;
    private Button playButton;
    private Button shopButton;
    private Button settingsButton;

    public event EventHandler OnStart;
    public event EventHandler OnPause;
    public event EventHandler OnShop;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        playButton = root.Q<Button>("play");
        shopButton = root.Q<Button>("shop");
        settingsButton = root.Q<Button>("settings");

        playButton.clicked += StartGame;
        settingsButton.clicked += Settings;
        shopButton.clicked += Shop;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Settings()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    private void Shop()
    {
        OnShop?.Invoke(this, EventArgs.Empty);
    }

    public void Activate()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void Deactivate()
    {
        root.style.display = DisplayStyle.None;
    }

    private void StartGame()
    {
        //Retira o foco do botão e evita o warning "FocusController has unprocessed focus events. Clearing."
        playButton.Blur();
        OnStart?.Invoke(this, EventArgs.Empty);
    }
}
