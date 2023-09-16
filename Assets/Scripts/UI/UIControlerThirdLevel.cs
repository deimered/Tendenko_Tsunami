using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIControlerThirdLevel : MonoBehaviour, IUIControler
{
    private const string styleResource = "ThirdLevelStyle";
    private const string ussSlot = "item-slot-scroll";
    private const string ussButton = "item-button-scroll";
    private const string removeButtonAnimation = "pointRemove";

    private Label eventText;
    private Button ignoreNextEventButton;
    private int lastButtonClickedIndex = -1;
    private bool onChooseItemState;
    private Button settingButton;
    private ScrollView itemScroll;
    private float rangePerItem;
    private float range = -1;

    /// <summary>
    private List<VisualElement> hungerPoints;
    private List<VisualElement> hydrationPoints;
    private List<VisualElement> hygienePoints;
    private List<VisualElement> lifePoints;
    /// </summary>

    [Range(0, 1)]
    public float itemWidthPercentage;

    public Canvas canvas;
    public event EventHandler OnNextEvent;
    public event EventHandler OnUseItemEvent;
    public event EventHandler OnIgnoreEvent;
    public event EventHandler OnPause;

    private bool dragging = false;
    private float mouseXonDragStart = 0;

    private Coroutine currentCoroutine;
    [SerializeField]
    private float textPerSecond = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        root.styleSheets.Add(Resources.Load<StyleSheet>(styleResource));

        eventText = root.Q<Label>("eventText");
        ignoreNextEventButton = root.Q<Button>("ignoreNextEventButton");
        settingButton = root.Q<Button>("settings_button");

        itemScroll = root.Q<ScrollView>("itemScroll");

        hungerPoints = new List<VisualElement>();
        hydrationPoints = new List<VisualElement>();
        hygienePoints = new List<VisualElement>();
        lifePoints = new List<VisualElement>();

        for (int i = 1; i <= 3; i++)
        {
            hungerPoints.Add(root.Q<VisualElement>("foodPoint" + i));
            hydrationPoints.Add(root.Q<VisualElement>("drinkPoint" + i));
            hygienePoints.Add(root.Q<VisualElement>("hygienePoint" + i));
            lifePoints.Add(root.Q<VisualElement>("lifePoint" + i));
        }

        ignoreNextEventButton.clicked += NextEvent;
        settingButton.clicked += SettingsButton;
        NextEvent();

        itemScroll.RegisterCallback<MouseDownEvent>(OnMouseScrollDown, TrickleDown.TrickleDown);
        itemScroll.RegisterCallback<MouseUpEvent>(OnMouseScrollUp, TrickleDown.TrickleDown);
        itemScroll.RegisterCallback<MouseMoveEvent>(OnMouseScrollMove, TrickleDown.TrickleDown);

        StartCoroutine(UILoaded());
    }

    public void SetEventDetails(string text, List<CollectibleItemScriptableObject> itens)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(TypeSequence(text));

        if (itens != null)
        {
            for (int i = 0; i < itens.Count; i++)
            {
                VisualElement slot = new VisualElement();
                slot.AddToClassList(ussSlot);

                Button button = new Button();
                button.AddToClassList(ussButton);

                button.style.backgroundImage = itens[i].Icon;
                button.RegisterCallback<ClickEvent, int>(UseItem, i);

                slot.hierarchy.Add(button);

                itemScroll.Add(slot);
            }
            range = itens.Count > 6 ? (itens.Count - 6) * rangePerItem : 0;
        }
    }



    public int GetLastButtonClickedIndex()
    {
        return lastButtonClickedIndex;
    }

    private void NextEvent()
    {
        if (onChooseItemState)
        {
            CleanItemScroll();
            itemScroll.scrollOffset = Vector2.zero;
            OnIgnoreEvent?.Invoke(this, EventArgs.Empty);
        }
            
        else
            OnNextEvent?.Invoke(this, EventArgs.Empty);
        onChooseItemState = !onChooseItemState;
    }

    private void UseItem(ClickEvent evt, int nSlot)
    {
        CleanItemScroll();
        itemScroll.scrollOffset = Vector2.zero;
        lastButtonClickedIndex = nSlot;
        onChooseItemState = false;
        OnUseItemEvent?.Invoke(this, EventArgs.Empty);
    }

    private void SettingsButton()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }

    public void Activate()
    {
        if (canvas != null)
            canvas.sortingOrder = 1;
    }

    public void Deactivate()
    {
        if (canvas != null)
            canvas.sortingOrder = 0;
    }

    private void CleanItemScroll()
    {
        foreach (VisualElement slot in itemScroll.Children())
            slot.hierarchy.ElementAt(0).UnregisterCallback<ClickEvent, int>(UseItem);

        itemScroll.contentContainer.Clear();
    }

    private void OnMouseScrollMove(MouseMoveEvent evt)
    {
        if (dragging)
        {
            Vector2 oldOffet = itemScroll.scrollOffset;
            itemScroll.scrollOffset = new Vector2(mouseXonDragStart - evt.localMousePosition.x, 0);
            if (itemScroll.scrollOffset.x < 0)
                itemScroll.scrollOffset = oldOffet;
            else if (itemScroll.scrollOffset.x > range)
                itemScroll.scrollOffset = oldOffet;
        }
    }

    private void OnMouseScrollUp(MouseUpEvent evt)
    {
        dragging = false;
    }

    private void OnMouseScrollDown(MouseDownEvent evt)
    {
        dragging = true;
        mouseXonDragStart = evt.localMousePosition.x + itemScroll.scrollOffset.x;
    }

    private void LosePoint(List<VisualElement> points)
    {
        for(int i = points.Count - 1; i >= 0; i--)
        {
            if (!points[i].ClassListContains(removeButtonAnimation))
            {
                points[i].ToggleInClassList(removeButtonAnimation);
                break;
            }
        }
    }

    public void LoseLife()
    {
        LosePoint(lifePoints);
    }

    public void LoseHunger()
    {
        LosePoint(hungerPoints);
    }

    public void LoseHydration()
    {
        LosePoint(hydrationPoints);
    }

    public void LoseHygiene()
    {
        LosePoint(hygienePoints);
    }

    IEnumerator UILoaded()
    {
        while (itemScroll.resolvedStyle.width.ToString() == "NaN" || range == -1)
        {
            yield return null;
        }

        rangePerItem = itemScroll.resolvedStyle.width * itemWidthPercentage;
        range = itemScroll.childCount > 6 ? (itemScroll.childCount - 6) * rangePerItem : 0;
    }

    IEnumerator TypeSequence(string sequence)
    {
        eventText.text = "";
        float time = 0;
        int prevIdx = 0;

        while(eventText.text.Length != sequence.Length)
        {
            time += Time.deltaTime;
            int length = (int)(time / textPerSecond);
            if (length != 0)
            {
                
                time -= (length * textPerSecond);
                eventText.text += sequence.Substring(prevIdx, (prevIdx + length >= sequence.Length) ? sequence.Length - prevIdx: length);
                prevIdx = (prevIdx + length) % sequence.Length;
            }

            yield return null;
        }
    }
}
