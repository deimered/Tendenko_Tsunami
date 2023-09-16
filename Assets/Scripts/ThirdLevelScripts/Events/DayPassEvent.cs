using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayPassEvent : IEvent
{
    private string challengeText;
    private string ignoreText;

    public DayPassEvent(string challengeText, string ignoreText)
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
        return new List<CollectibleItemScriptableObject>();
    }

    public string IgnoreEventText()
    {
        return ignoreText;
    }

    public string GetResultMessage()
    {
        return "You have received a notice indicating that it is now safe to return to the city. " +
            "You have successfully managed to survive the tsunami.";
    }

    public string GetResultMessage(CollectibleItemScriptableObject item)
    {
        return "You have received a notice indicating that it is now safe to return to the city. " +
            "You have successfully managed to survive the tsunami.";
    }

    public void SetItemInMessage(CollectibleItemScriptableObject item)
    {
    }
}
