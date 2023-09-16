using UnityEngine;

[CreateAssetMenu(fileName = "CollectibleItemScriptableObject", menuName = "ScriptableObjects/Collectible Object")]
public class CollectibleItemScriptableObject : ScriptableObject
{
    public enum ItemFunction { EAT, DRINK, SEARCH, HEALTH, SUPPLY, ENTERTAINMENT, HYGIENE};

    public enum ItemType { CHARGER, ELETRONIC, NONE};

    [SerializeField]
    private Texture2D icon;
    [SerializeField]
    private ItemType itemType;
    [SerializeField]
    private ItemFunction itemFunction;

    [SerializeField]
    [Min(1)]
    private int maxDurability;
    [SerializeField]
    [Min(1)]
    private int durability;
    [SerializeField]
    [Min(1)]
    private int space;
    [SerializeField]
    [Min(1)]
    private int itemPrice;

    [SerializeField]
    private bool bought;

    [SerializeField]
    private string itemDescription;

    [SerializeField]
    private string itemName;
    
    public string ItemDescription
    {
        get { return itemDescription; }
    }

    public int ItemPrice
    {
        get { return itemPrice; }
    }

    public int Durability
    {
        get { return durability; }
    }

    public int Space
    {
        get { return space; }
    }

    public Texture2D Icon
    {
        get { return icon; }
    }

    public ItemFunction Function
    {
        get { return itemFunction; }
    }

    public ItemType Type
    {
        get { return itemType; }
    }

    public bool Bought
    {
        get { return bought; }
    }

    public string ItemName
    {
        get { return itemName; }
    }

    private void OnEnable()
    {
        ResetItem();
    }

    private void OnDisable()
    {
        ResetItem();
    }

    public void LoseDurability()
    {
        durability -= 1;
    }

    public void BuyItem()
    {
        bought = true;
        PlayerPrefs.SetInt(itemName, 1);
    }

    public void ResetItem()
    {
        durability = maxDurability;
        bought = PlayerPrefs.GetInt(itemName, 0) == 1;
    }
}
