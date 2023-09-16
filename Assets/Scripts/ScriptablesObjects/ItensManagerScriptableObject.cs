//using Codice.Client.Common;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ItensManagerScriptableObject", menuName = "ScriptableObjects/Itens Manager")]
public class ItensManagerScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<CollectibleItemScriptableObject> itens;

    [System.NonSerialized]
    public UnityEvent<CollectibleItemScriptableObject> addItemEvent;

    [System.NonSerialized]
    public UnityEvent<CollectibleItemScriptableObject> removeItemEvent;

    public void AddItem(CollectibleItemScriptableObject item)
    {
        itens.Add(item);
        addItemEvent?.Invoke(item);
    }

    public void RemoveItem(CollectibleItemScriptableObject item)
    {
        itens.Remove(item);
        item.ResetItem();
        removeItemEvent?.Invoke(item);
    }


    public List<CollectibleItemScriptableObject> GetCollectibleItens()
    {
        return itens;
    }

    public void RemoveItens()
    {
        foreach(CollectibleItemScriptableObject item in itens.ToArray())
            RemoveItem(item);
    }

    public void SetCollectibleItens(List<CollectibleItemScriptableObject> itens)
    {
        this.itens = itens;
    }

    private void OnEnable()
    { 
        addItemEvent ??= new UnityEvent<CollectibleItemScriptableObject>();

        removeItemEvent ??= new UnityEvent<CollectibleItemScriptableObject>();
    }
}
