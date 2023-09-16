using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SearchEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    private CollectibleItemScriptableObject itemInText;

    public SearchEvent(string challengeText, string ignoreText)
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
        return item.Function == CollectibleItemScriptableObject.ItemFunction.SEARCH;
    }

    public List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens)
    {
        List<CollectibleItemScriptableObject> allowedItens = new List<CollectibleItemScriptableObject>();
        foreach (CollectibleItemScriptableObject item in itens)
            if (item.Function == CollectibleItemScriptableObject.ItemFunction.SEARCH
                || item.Function == CollectibleItemScriptableObject.ItemFunction.ENTERTAINMENT)
                allowedItens.Add(item);
        return allowedItens;
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You search for an item.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        if (itemInText != null)
            return item.Function == CollectibleItemScriptableObject.ItemFunction.SEARCH ? "You were able to use the " + item.ItemName + " to find some help." +
                "\nYou got " + itemInText.ItemName + " ." : "The " + item.ItemName + " wasn't useful to find anything.";
        else
            return item.Function == CollectibleItemScriptableObject.ItemFunction.SEARCH ? "You were able to use the " + item.ItemName + " to find some help." +
                "\nYou were given an item." : "The " + item.ItemName + " wasn't useful to find anything.";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
        itemInText = item;
    }
}
