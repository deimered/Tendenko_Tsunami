using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEvent
{
    string Challenge();
    string IgnoreEventText();
    bool Result(CollectibleItemScriptableObject item);
    string GetResultMessage();
    string GetResultMessage(CollectibleItemScriptableObject item);
    void SetItemInMessage(CollectibleItemScriptableObject item);
    bool IgnoreEventResult();
    List<CollectibleItemScriptableObject> AllowedItens(List<CollectibleItemScriptableObject> itens);
}
