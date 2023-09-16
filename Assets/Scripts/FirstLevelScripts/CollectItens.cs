using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CollectItens : MonoBehaviour
{
    //Tamanho padrão do inventário
    private int maxItens = 3;

    //Lista com os recursos dentro do alcance.
    List<ItemPointer> inReachItens;
    //Lista dos recursos coletados.
    List<ItemPointer> itens;

    //NPC dentro do alcance do Player.
    NpcObstacleController npcObstacleController;

    //O gerenciador de recursos.
    [SerializeField]
    private ItensManagerScriptableObject itensManager;

    // Start is called before the first frame update
    void Start()
    {
        inReachItens = new List<ItemPointer>();
        itens = new List<ItemPointer>();
        maxItens = PlayerPrefs.GetInt("BackPack", 0) == 1 ? 6 : maxItens;
    }

    private void OnEnable()
    {
        //Quando um item é removido, é atiada a função RemoveItem.
        itensManager.removeItemEvent.AddListener(RemoveItem);
    }

    private void OnDisable()
    {
        //Remoção do Listener.
        itensManager.removeItemEvent.RemoveListener(RemoveItem);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //Colisão com obsjetos colecionáveis e NPCs.
        other.TryGetComponent<ItemPointer>(out ItemPointer collectibleItens);
        other.TryGetComponent<NpcObstacleController>(out npcObstacleController);
        if (collectibleItens != null)
        {
            if (itens.Count < maxItens && itens.Count + collectibleItens.Item.Space <= maxItens)
                collectibleItens.Reachable();
            else
                collectibleItens.Impossible();
            inReachItens.Add(collectibleItens);
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        //Colisão com obsjetos colecionáveis e NPCs.
        other.TryGetComponent<ItemPointer>(out ItemPointer collectibleItens);
        if (collectibleItens != null)
        {
            collectibleItens.Unreachable();
            inReachItens.Remove(collectibleItens);
        }
        other.TryGetComponent<NpcObstacleController>(out NpcObstacleController npcObstacleControllerExit);
        if (npcObstacleController == npcObstacleControllerExit)
            npcObstacleController = null;
    }

    //Coletar um recurso.
    public void PickItem()
    {
        if (inReachItens.Count > 0 && itens.Count + inReachItens[0].Item.Space <= maxItens)
        {
            ItemPointer collectibleItem = inReachItens[0];
            inReachItens.Remove(collectibleItem);
            collectibleItem.PickItem();
            for (int i = 0; i < collectibleItem.Item.Space; i++)
                itens.Add(collectibleItem);
            
            itensManager.AddItem(collectibleItem.Item);
        }

        UpdateInReachItens();
    }

    //Salvar um NPC.
    public void SavePerson()
    {
        if (IsTherePerson())
            npcObstacleController.Save();
    }

    public bool IsTherePerson()
    {
        return (npcObstacleController != null && !npcObstacleController.IsSave);
    }

    //Remover um recurso.
    public void RemoveItem(CollectibleItemScriptableObject item)
    {
        ItemPointer collectibleItem = itens.Find(collectibleItem => collectibleItem.Item == item);
        if (collectibleItem != null)
        {
            collectibleItem.PutDownItem();
            itens.RemoveAll(item => item == collectibleItem);
        }

        UpdateInReachItens();
    }

    //Atualizar o estado dos recursos que estão perto do jogador.
    private void UpdateInReachItens()
    {
        foreach (ItemPointer collectibleItens in inReachItens)
        {
            if (itens.Count < maxItens && itens.Count + collectibleItens.Item.Space <= maxItens)
                collectibleItens.Reachable();
            else
                collectibleItens.Impossible();
        }
    }

    //Retornar os recursos coletados.
    public List<CollectibleItemScriptableObject> GetCollectibleItens()
    {
        return itensManager.GetCollectibleItens();
    }

    //Colocar novos valores para os itens.
    public void SetCollectibleItens(List<ItemPointer> itens)
    {
        this.itens = itens;
    }
}
