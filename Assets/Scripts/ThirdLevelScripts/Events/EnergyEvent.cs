using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    private CollectibleItemScriptableObject itemInText;

    public EnergyEvent(string challengeText, string ignoreText)
    {
        this.challengeText = challengeText;
        this.ignoreText = ignoreText;
    }

    public string Challenge()
    {
        return challengeText;
    }

    public bool IgnoreEventResult()
    {
        return false;
    }

    public bool Result(CollectibleItemScriptableObject item)
    {
        return item.Type == CollectibleItemScriptableObject.ItemType.CHARGER;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.SUPPLY)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You replenished the energy of your equipment.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        if (itemInText != null)
            return item.Type == CollectibleItemScriptableObject.ItemType.CHARGER ? "The " + item.ItemName + " replenished the energy to the " + itemInText.ItemName + "." :
            "The " + item.ItemName + " wasn't able to replenished the energy to your equipment.\n The " + itemInText.ItemName + " is now useless.";
        else
            return item.Type == CollectibleItemScriptableObject.ItemType.CHARGER ? "The " + item.ItemName + " replenished the energy to your equipment." :
            "The " + item.ItemName + " wasn't able to replenished the energy to your equipment.\n The equipment is now useless.";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
        itemInText = item;
    }
}
