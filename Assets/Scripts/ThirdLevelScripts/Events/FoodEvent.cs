using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public FoodEvent(string challengeText, string ignoreText)
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
        return item.Function == CollectibleItemScriptableObject.ItemFunction.EAT;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.EAT)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You eat.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return item.Function == CollectibleItemScriptableObject.ItemFunction.EAT ? "You were able to eat for today." :
            "You weren't able to eat the " + item.ItemName + ".";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
