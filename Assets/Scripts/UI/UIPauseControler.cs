using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIPauseControler : MonoBehaviour, IUIControler
{
    public bool isInMainMenu;
    public Texture2D soundIcon;
    public Texture2D musicIcon;
    public Texture2D noSoundIcon;
    public Texture2D noMusicIcon;

    private VisualElement root;

    //Pause elements
    private VisualElement pauseMenu;
    private Button resumeButton;
    private Button settingsButton;
    private Button exitButton;

    //Settings elements
    private VisualElement settingsMenu;
    private Button soundButton;
    private Button musicButton;
    private SliderInt soundSlider;
    private SliderInt musicSlider;
    private Button backButton;

    private int previousSoundValue;
    private int previousMusicValue;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClip;
    [SerializeField]
    private bool isPause;

    public event EventHandler OnPause;
    public event EventHandler OnBack;
    public event EventHandler OnExit;


    void Start()
    {
        if (!TryGetComponent<AudioSource>(out audioSource))
            audioSource = this.gameObject.AddComponent<AudioSource>();

        root = GetComponent<UIDocument>().rootVisualElement;

        pauseMenu = root.Q<VisualElement>("pause_menu_background");
        resumeButton = root.Q<Button>("resume");
        settingsButton = root.Q<Button>("settings");
        exitButton = root.Q<Button>("exit");

        settingsMenu = root.Q<VisualElement>("settings_menu_background");
        soundButton = root.Q<Button>("sound_icon");
        musicButton = root.Q<Button>("music_icon");
        soundSlider = root.Q<SliderInt>("sound_slider");
        musicSlider = root.Q<SliderInt>("music_slider");
        backButton = root.Q<Button>("back");

        soundSlider.value = (int)(PlayerPrefs.GetFloat("Sound", 0.5f) * 100);
        musicSlider.value = (int)(PlayerPrefs.GetFloat("Music", 0.5f) * 100);

        if (musicSlider.value == 0)
            musicButton.style.backgroundImage = noMusicIcon;

        if (soundSlider.value == 0)
            soundButton.style.backgroundImage = noSoundIcon;

        resumeButton.clicked += Resume;
        settingsButton.clicked += ChangeToSettings;
        exitButton.clicked += BackToMainMenu;

        soundButton.clicked += SoundButton;
        musicButton.clicked += MusicButton;
        backButton.clicked += SettingsBack;
        soundSlider.RegisterValueChangedCallback<int>(OnChangeSoundSlider);
        musicSlider.RegisterValueChangedCallback<int>(OnChangeMusicSlider);

        previousMusicValue = musicSlider.value;
        previousSoundValue = soundSlider.value;

        if (isInMainMenu)
            ChangeToSettings();

        audioSource.loop = true;
        audioSource.volume = PlayerPrefs.GetFloat("Music", 0.5f);
        audioSource.clip = audioClip;

        if(!isPause)
            audioSource.Play();

        Deactivate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeToSettings()
    {
        pauseMenu.style.display = DisplayStyle.None;
        settingsMenu.style.display = DisplayStyle.Flex;
    }

    private void SettingsBack()
    {
        
        if (!isInMainMenu)
        {
            settingsMenu.style.display = DisplayStyle.None;
            pauseMenu.style.display = DisplayStyle.Flex;
        }   
        else
            OnBack?.Invoke(this, EventArgs.Empty);
    }

    private void SoundButton()
    {
        if (soundSlider.value == 0)
        {
            soundButton.style.backgroundImage = soundIcon;
            soundSlider.value = previousSoundValue;
            PlayerPrefs.SetFloat("Sound", previousSoundValue / 100f);
        }
        else
        {
            soundButton.style.backgroundImage = noSoundIcon;
            previousSoundValue = soundSlider.value;
            soundSlider.value = 0;
            PlayerPrefs.SetFloat("Sound", 0);
        }
    }

    private void MusicButton()
    {
        if (musicSlider.value == 0)
        {
            musicButton.style.backgroundImage = musicIcon;
            musicSlider.value = previousMusicValue;
            PlayerPrefs.SetFloat("Music", previousMusicValue / 100f);
            audioSource.volume = previousMusicValue / 100f;
        }
        else
        {
            musicButton.style.backgroundImage = noMusicIcon;
            previousMusicValue = musicSlider.value;
            musicSlider.value = 0;
            PlayerPrefs.SetFloat("Music", 0);
            audioSource.volume = 0;
        }
    }

    private void OnChangeSoundSlider(ChangeEvent<int> evt)
    {
        if (evt.newValue == 0)
            soundButton.style.backgroundImage = noSoundIcon;

        if (evt.previousValue == 0)
            soundButton.style.backgroundImage = soundIcon;

        PlayerPrefs.SetFloat("Sound", evt.newValue / 100f);
    }

    private void OnChangeMusicSlider(ChangeEvent<int> evt)
    {
        if (evt.newValue == 0)
            musicButton.style.backgroundImage = noMusicIcon;

        if (evt.previousValue == 0)
            musicButton.style.backgroundImage = musicIcon;

        PlayerPrefs.SetFloat("Music", evt.newValue / 100f);
        audioSource.volume = evt.newValue / 100f;
    }

    private void Resume()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    public void Activate()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void Deactivate()
    {
        root.style.display = DisplayStyle.None;
    }
    private void BackToMainMenu()
    {
        OnExit?.Invoke(this, EventArgs.Empty);
    }

    public void PauseMusic()
    {
        if (audioSource != null)
            audioSource.Pause();
        isPause = true;
    }

    public void StartMusic()
    {
        if (audioSource != null)
            audioSource.Play();
        isPause = false;
    }
}
