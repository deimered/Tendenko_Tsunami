using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject ui;
    public GameObject pause;

    private IUIControler uiControler;
    private IUIControler pauseControler;

    private void Awake()
    {
        uiControler = ui.GetComponent<IUIControler>();
        pauseControler = pause.GetComponent<IUIControler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        uiControler.OnPause += HandlePause;
        pauseControler.OnPause += HandleUnpause;
    }

    private void HandlePause(object sender, EventArgs e)
    {
        uiControler.Deactivate();
        pauseControler.Activate();
        //Altera a escala que passa o tempo e para a execução do FixedUpdate.
        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    private void HandleUnpause(object sender, EventArgs e)
    {
        pauseControler.Deactivate();
        uiControler.Activate();
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
