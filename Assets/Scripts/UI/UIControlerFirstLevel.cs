using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIControlerFirstLevel : MonoBehaviour, IUIControler
{
    private VisualElement root;
    private Button settingButton;
    private Button reloadButton;

    private Button[] slots;
    private List<Texture2D> icons;

    private Label timerText;
    [SerializeField]
    [Min(0)]
    private int minutes;
    [SerializeField]
    [Min(0)]
    private int seconds;

    private bool fullInventory;

    public Canvas canvas;
    private bool active = true;

    public event EventHandler OnPause;
    public event EventHandler OnGameOver;
    public event EventHandler OnReload;

    [SerializeField]
    private ItensManagerScriptableObject itensManager;

    // Start is called before the first frame update
    void Awake()
    {
        icons = new List<Texture2D>();
        fullInventory = PlayerPrefs.GetInt("BackPack", 0) == 1;
        root = GetComponent<UIDocument>().rootVisualElement;
        VisualElement inventory = root.Q<VisualElement>("inventory");

        slots = new Button[inventory.childCount];
        int idx = 0;
        foreach (VisualElement container in inventory.Children())
        {
            Button button = (Button)container.Children().First();
            button.RegisterCallback<ClickEvent, int>(RemoveItem, idx);
            slots[idx] = button;
            idx++;
            if (fullInventory)
                container.style.display = DisplayStyle.Flex;
        }

        settingButton = root.Q<Button>("settings_button");
        reloadButton = root.Q<Button>("reload_button");

        settingButton.clicked += SettingsButton;
        reloadButton.clicked += ReloadLevel;

        timerText = root.Q<Label>("timer");

        Timer.SetTime(minutes, seconds);

        Activate();
    }

    private void Update()
    {
        if (active)
        {
            Timer.UpdateTimer(Time.deltaTime);
            timerText.text = Timer.GetCurrentTime();
            if(Timer.TimeInSeconds == 0)
                OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnEnable()
    {
        itensManager.addItemEvent.AddListener(PutItemInSlot);
    }

    private void OnDisable()
    {
        itensManager.addItemEvent.RemoveListener(PutItemInSlot);
    }

    private void RemoveItem(ClickEvent evt, int index)
    {
        if (icons.Count > index)
        {
            itensManager.RemoveItem(itensManager.GetCollectibleItens().Find(item => item.Icon == icons[index]));
            RecolorSlots(index);
        }
    }

    private void RecolorSlots(int index)
    {
        Texture2D texture = icons[index];

        icons.RemoveAll(icon => icon == texture);

        for (int i = 0; i < slots.Length; i++)
        {
            if (i == slots.Length - 1 || icons.Count - 1 < i)
                PutNoItemBackground(i);

            else
                slots[i].style.backgroundImage = icons[i];
        }
    }

    private void PutItemInSlot(CollectibleItemScriptableObject item)
    {
        for(int x = 0; x < item.Space; x++)
        {
            icons.Add(item.Icon);
            slots[icons.Count - 1].style.backgroundImage = item.Icon;
            slots[icons.Count - 1].style.borderBottomLeftRadius = 0;
            slots[icons.Count - 1].style.borderBottomRightRadius = 0;
            slots[icons.Count - 1].style.borderTopLeftRadius = 0;
            slots[icons.Count - 1].style.borderTopRightRadius = 0;
            slots[icons.Count - 1].style.backgroundColor = Color.clear;
        }
    }

    private void PutNoItemBackground(int index)
    {
        slots[index].style.borderBottomLeftRadius = 15;
        slots[index].style.borderBottomRightRadius = 15;
        slots[index].style.borderTopLeftRadius = 15;
        slots[index].style.borderTopRightRadius = 15;
        slots[index].style.backgroundColor = new Color(1, 1, 1, 0.38f);
        slots[index].style.backgroundImage = null;
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

    private void ReloadLevel()
    {
        reloadButton.Blur();
        OnReload?.Invoke(this, EventArgs.Empty);
    }

    public void HideUI()
    {
        root.style.display = DisplayStyle.None;
    }

    public void ShowUI()
    {
        root.style.display = DisplayStyle.Flex;
    }
}
