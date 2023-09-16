using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirstEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public ThirstEvent(string challengeText, string ignoreText)
    {
        this.challengeText = challengeText;
        this.ignoreText = ignoreText;
    }
    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.DRINK)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string Challenge()
    {
        return challengeText;
    }

    public bool IgnoreEventResult()
    {
        return false;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public bool Result(CollectibleItemScriptableObject item)
    {
        return item.Function == CollectibleItemScriptableObject.ItemFunction.DRINK;
    }

    public string GetResultMessage()
    {
        return "You drink your item.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return item.Function == CollectibleItemScriptableObject.ItemFunction.DRINK ? "You were able to drink for the day." :
            "You weren't able to drink the " + item.ItemName + ".";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
