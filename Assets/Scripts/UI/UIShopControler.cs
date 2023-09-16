using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIShopControler : MonoBehaviour, IUIControler
{
    public event EventHandler OnPause;

    public List<CollectibleItemScriptableObject> itens;
    [SerializeField]
    [Min(0)]
    private int currency;
    [SerializeField]
    [Min(0)]
    private int maxCurrency;
    private CollectibleItemScriptableObject currentItem;
    private ShopItem currentShopItem;

    private VisualElement root;
    private Label currencyText;
    private Button backButton;
    private VisualElement shopBackground;
    private VisualElement detailItem;
    private VisualElement itemIcon;
    private Label itemCost;
    private Label itemDescription;
    private Button buyButton;
    private Button exitButton;


    // Start is called before the first frame update
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        currencyText = root.Q<Label>("currency_text");
        backButton = root.Q<Button>("back");
        shopBackground = root.Q<VisualElement>("shop_background");
        detailItem = root.Q<VisualElement>("detail_item_information");
        itemIcon = root.Q<VisualElement>("item_icon");
        itemCost = root.Q<Label>("item_cost");
        itemDescription = root.Q<Label>("item_description");
        buyButton = root.Q<Button>("buy_button");
        exitButton = root.Q<Button>("exit_button");

        ChangeCurrency(PlayerPrefs.GetInt("score"));

        foreach (CollectibleItemScriptableObject item in itens)
        {
            ShopItem itemShop = new ShopItem(item.Icon);
            if (item.Bought)
                itemShop.OnBought();
                
            itemShop.click += () =>
            {
                currentItem = item;
                currentShopItem = itemShop;
                itemIcon.style.backgroundImage = item.Icon;
                itemDescription.text = item.ItemDescription;
                itemCost.text = item.ItemPrice.ToString();
                buyButton.style.display = item.Bought ? DisplayStyle.None : DisplayStyle.Flex;
                shopBackground.style.display = DisplayStyle.None;
                detailItem.style.display = DisplayStyle.Flex;
            };
            shopBackground.Add(itemShop);
        }

        exitButton.clicked += ExitDetailView;
        buyButton.clicked += BuyItem;
        backButton.clicked += ShopBack;
        Deactivate();
    }

    public void Activate()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void Deactivate()
    {
        root.style.display = DisplayStyle.None;
    }

    public void ChangeCurrency(int newValue)
    {
        newValue = Mathf.Clamp(newValue, 0, maxCurrency);
        currencyText.text = newValue.ToString();
        currency = newValue;
    }

    private void ExitDetailView()
    {
        currentItem = null;
        detailItem.style.display = DisplayStyle.None;
        shopBackground.style.display = DisplayStyle.Flex;
    }

    private void BuyItem()
    {
        if (currentItem != null && currency >= currentItem.ItemPrice)
        {
            currency -= currentItem.ItemPrice;
            ChangeCurrency(currency);
            currentItem.BuyItem();
            currentShopItem.OnBought();
            ExitDetailView();
        }  
    }

    private void ShopBack()
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
}
