using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurvivorsEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public SurvivorsEvent(string challengeText, string ignoreText)
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
        return true;
    }

    public bool Result(CollectibleItemScriptableObject item)
    {
        return true;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        return itens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You gave the your item to the other survivors.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return "You gave the " + item.ItemName + " to the other survivors.";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
