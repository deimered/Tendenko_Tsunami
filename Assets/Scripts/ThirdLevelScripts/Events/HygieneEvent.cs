using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HygieneEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public HygieneEvent(string challengeText, string ignoreText)
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
        return item.Function == CollectibleItemScriptableObject.ItemFunction.HYGIENE;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.HYGIENE)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You clean yourself.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return item.Function == CollectibleItemScriptableObject.ItemFunction.HYGIENE ? "You used the " + item.ItemName + " to keep yourself clean." :
            "The " + item.ItemName + " didn't clean you.";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
