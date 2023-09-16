using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIGameEndControler : MonoBehaviour, IUIControler
{
    private const string ussScore = "level_points";

    private VisualElement root;

    private Label gameEndText;
    private Label secondText;
    private Button replayButton;
    private Button exitButton;
    private Label totalPointsText;
    private VisualElement coinIcon;
    private VisualElement scroll;
    private VisualElement[] levels;

    private int totalScore = 0;
    private int currentLevel;

    public event EventHandler OnPause;
    public event EventHandler OnExit;

    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        gameEndText = root.Q<Label>("game_end_text");
        secondText = root.Q<Label>("second_text");
        totalPointsText = root.Q<Label>("total_points_text");

        coinIcon = root.Q<VisualElement>("coin_icon");

        replayButton = root.Q<Button>("replay_button");
        exitButton = root.Q<Button>("exit_button");

        scroll = root.Q<VisualElement>("scroll_points");

        levels = new VisualElement[scroll.childCount];

        currentLevel = 0;

        foreach (VisualElement child in scroll.Children())
        {
            levels[currentLevel] = child;
            currentLevel = (currentLevel + 1) % levels.Length;
        }
        levels[currentLevel].style.display = DisplayStyle.Flex;

        replayButton.clicked += Replay;
        exitButton.clicked += BackToMainMenu;

        Deactivate();
    }

    public void Activate()
    {
        Time.timeScale = 0;
        root.style.display = DisplayStyle.Flex;
    }

    public void Deactivate()
    {
        root.style.display = DisplayStyle.None;
    }

    public void AddScoreText(string text, int score)
    {
        totalScore += score;
        Label label = new Label(text + ": " + score);
        label.AddToClassList(ussScore);
        levels[currentLevel].Add(label);
    }

    public void NextLevel()
    {
        currentLevel = (currentLevel + 1) % levels.Length;
        levels[currentLevel].style.display = DisplayStyle.Flex;
    }

    public void TotalScore()
    {
        totalPointsText.text = "Total: " + totalScore;
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + totalScore);
        levels[levels.Length - 1].style.display = DisplayStyle.Flex;
        coinIcon.style.display = DisplayStyle.Flex;
    }

    public void ResetScore()
    {
        currentLevel = 0;
        totalScore = 0;

        for (int i = 0; i < levels.Length; i++)
        {
            while(levels[i].childCount > 1 && i != levels.Length - 1)
                levels[i].RemoveAt(1);
            
            levels[i].style.display = DisplayStyle.None;
        }
        levels[currentLevel].style.display = DisplayStyle.Flex;
        gameEndText.text = "Game Over";
        secondText.text = "";
    }

    public void SetDeathCause(string deathCause)
    {
        secondText.text = deathCause;
    }

    public void DisplayWinText()
    {
        gameEndText.text = "You Survived";
        secondText.text = "Congratulations!";
    }

    private void BackToMainMenu()
    {
        Deactivate();
        OnExit?.Invoke(this, EventArgs.Empty);
    }

    private void Replay()
    {
        Deactivate();
        OnPause?.Invoke(this, EventArgs.Empty);
    }
}
