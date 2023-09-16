using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIControlerSecondLevel : MonoBehaviour, IUIControler
{
    private VisualElement root;
    private VisualElement timerBar;
    private VisualElement timerBarBackground;
    private VisualElement helpBar;
    private VisualElement helpBarBackground;
    private VisualElement waveIcon;

    private float previousBarWidth;
    private float currentBarWidth;
    private Button settingButton;
    private bool active = true;
    private bool isUiLoaded = false;
    private float currentHelpBarWidth;

    [SerializeField]
    private float helpBarSeconds = 10f;

    public Canvas canvas;
    public event EventHandler OnPause;
    public event EventHandler OnTsunamiFlag;
    public event EventHandler OnHelpBarFullFlag;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        timerBar = root.Q<VisualElement>("timerBar");
        timerBarBackground = root.Q<VisualElement>("timerBarBackground");
        helpBar = root.Q<VisualElement>("helpBar");
        helpBarBackground = root.Q<VisualElement>("helpBarCorners");
        waveIcon = root.Q<VisualElement>("waveSymbol");
        settingButton = root.Q<Button>("settings_button");

        settingButton.clicked += SettingsButton;
        StartCoroutine(UILoaded());
        StartCoroutine(TimeUntilTsunami());
    }

    private void SettingsButton()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    public void Activate()
    {
        active = true;
        if (canvas != null)
            canvas.sortingOrder = 1;
    }

    public void Deactivate()
    {
        active = false;
        if (canvas != null)
            canvas.sortingOrder = 0;
    }

    public void HideUI()
    {
        root.style.display = DisplayStyle.None;
    }

    public void ShowUI()
    {
        root.style.display = DisplayStyle.Flex;
    }

    IEnumerator UILoaded()
    {
        while (waveIcon.resolvedStyle.width.ToString() == "NaN")
        {
            yield return null;
        }

        waveIcon.style.left = waveIcon.resolvedStyle.left - 7 * waveIcon.resolvedStyle.width / 25;
        isUiLoaded = true;
    }

    IEnumerator TimeUntilTsunami()
    {
        while(Timer.TimeInSeconds != 0)
        {
            if (active && isUiLoaded)
            {
                Timer.UpdateTimer(Time.deltaTime);
                currentBarWidth = (1 - Timer.GetTimePercentage() / 100) * timerBarBackground.resolvedStyle.width;

                if (!float.IsNaN(currentBarWidth) && !float.IsNaN(previousBarWidth))
                {
                    timerBar.style.width = currentBarWidth;
                    waveIcon.style.left = waveIcon.style.left.value.value - previousBarWidth + currentBarWidth;
                }

                previousBarWidth = currentBarWidth;
            }

            yield return null;
        }

        currentBarWidth = timerBarBackground.resolvedStyle.width;
        timerBar.style.width = currentBarWidth;
        waveIcon.style.left = waveIcon.style.left.value.value - previousBarWidth + currentBarWidth;

        OnTsunamiFlag?.Invoke(this, EventArgs.Empty);
    }

   public IEnumerator HelpBar()
    {
        float time = 0;
        helpBarBackground.style.display = DisplayStyle.Flex;
        helpBar.style.width = 0;
        while (time < helpBarSeconds && isUiLoaded)
        {
            time += Time.deltaTime;
            currentHelpBarWidth = (time / helpBarSeconds) * helpBarBackground.resolvedStyle.width;
            helpBar.style.width = currentHelpBarWidth;
            yield return null;
        }

        helpBarBackground.style.display = DisplayStyle.None;
        OnHelpBarFullFlag?.Invoke(this, EventArgs.Empty);
    }
}
