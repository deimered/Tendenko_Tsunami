using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UILoadScreenControler : MonoBehaviour, IUIControler
{
    private VisualElement root;
    private VisualElement wave_icon;
    private Label loadingText;
    private Label title;
    private Label text;
    private Button advanceButton;

    private bool active = true;
    private bool alphaFlag = true;
    private float alpha = 0;
    public float iconChangeSpeed;

    [SerializeField]
    private string[] titles;
    [SerializeField]
    private string[] texts;


#pragma warning disable 0067
    public event EventHandler OnPause;
#pragma warning restore 0067
    public event EventHandler OnAdvance;

    void Awake()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        wave_icon = root.Q<VisualElement>("wave_icon");
        loadingText = root.Q<Label>("loading");

        title = root.Q<Label>("title");
        text = root.Q<Label>("text");
        advanceButton = root.Q<Button>("advanceButton");

        advanceButton.clicked += AdvanceAction;

        Deactivate();
    }

    public void Activate()
    {
        if (titles.Length > 0)
        {
            int randIdx = UnityEngine.Random.Range(0, titles.Length);
            title.text = titles[randIdx];
            text.text = texts[randIdx];
        }
        root.style.display = DisplayStyle.Flex;
        active = true;
        alphaFlag = false;
        alpha = 0.9f;
        StartCoroutine(IconAlphaChange());
    }

    public void Deactivate()
    {
        active = false;
        root.style.display = DisplayStyle.None;
        wave_icon.style.display = DisplayStyle.Flex;
        title.style.display = DisplayStyle.Flex;
        text.style.display = DisplayStyle.Flex;
        loadingText.style.display = DisplayStyle.Flex;
        advanceButton.style.display = DisplayStyle.None;
    }

    IEnumerator IconAlphaChange()
    {
        while (active)
        {
            wave_icon.style.unityBackgroundImageTintColor = new Color(1, 1, 1, alpha);
            alpha += alphaFlag ? Time.deltaTime * iconChangeSpeed : Time.deltaTime * iconChangeSpeed * -1;
            alpha = Mathf.Clamp01(alpha);

            if (alpha <= 0 || alpha >= 1)
                alphaFlag = !alphaFlag;
            yield return null;
        }
    }

    private void AdvanceAction()
    {
        OnAdvance?.Invoke(this, EventArgs.Empty);
    }

    public void LevelReady()
    {
        advanceButton.style.display = DisplayStyle.Flex;
    }

    public void DisableElements()
    {
        wave_icon.style.display = DisplayStyle.None;
        title.style.display = DisplayStyle.None;
        text.style.display = DisplayStyle.None;
        loadingText.style.display = DisplayStyle.None;
    }
}
