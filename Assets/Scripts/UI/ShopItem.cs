using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class ShopItem : VisualElement
{
    [UnityEngine.Scripting.Preserve]

    public new class UxmlFactory : UxmlFactory<ShopItem, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get
            {
                yield return new UxmlChildElementDescription(typeof(VisualElement));
            }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
        }
    }

    private const string styleResource = "ShopStyle";
    private const string ussIcon = "shop_item";
    private const string ussBackground = "item_background";

    public event Action click;

    public ShopItem()
    {
        styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
        AddToClassList(ussBackground);
        Button item_icon = new Button();
        item_icon.name = "item_icon";
        item_icon.AddToClassList(ussIcon);
        hierarchy.Add(item_icon);
    }

    public ShopItem(Texture2D tex)
    {

        styleSheets.Add(Resources.Load<StyleSheet>(styleResource));
        AddToClassList(ussBackground);

        Button item_icon = new Button();
        item_icon.name = "item_icon";
        item_icon.AddToClassList(ussIcon);
        hierarchy.Add(item_icon);

        item_icon.style.backgroundImage = tex;
        item_icon.clicked += OnClick;
    }

    private void OnClick()
    {
        click?.Invoke();
    }

    public void OnBought()
    {
        style.borderBottomColor = Color.green;
        style.borderTopColor = Color.green;
        style.borderLeftColor = Color.green;
        style.borderRightColor = Color.green;
    }
}
