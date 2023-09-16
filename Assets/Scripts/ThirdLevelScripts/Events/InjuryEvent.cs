using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InjuryEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public InjuryEvent(string challengeText, string ignoreText)
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
        return item.Function == CollectibleItemScriptableObject.ItemFunction.HEALTH;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.HEALTH)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You heal your injuries.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return item.Function == CollectibleItemScriptableObject.ItemFunction.HEALTH ? "With the " + item.ItemName + ", you were able to heal your injuries." :
        "You weren't able to heal your injuries with the " + item.ItemName + ".";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
