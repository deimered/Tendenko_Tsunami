using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class ItemPointer : MonoBehaviour
{
    //For the itens that are available by default
    [SerializeField]
    private bool isDefaultItem;

    [SerializeField]
    private CollectibleItemScriptableObject collectibleItem;

    public CollectibleItemScriptableObject Item
    {
        get { return collectibleItem; }
    }

    [SerializeField]
    private GameObject pointer;
    [SerializeField]
    private Material AlternateMaterial;
    [SerializeField]
    private Material DefaultMaterial;
    [SerializeField]
    private Material ImpossibleMaterial;

    private Renderer pointerRenderer;

    [SerializeField]
    private float pointerRotationSpeed = 1f;
    [SerializeField]
    private float pointerMovementSpeed = 1f;
    [SerializeField]
    private float pointerScale = 1f;
    [SerializeField]
    private Vector3 pointerOffset;

    private float posY;

    private void Awake()
    {
        if (!isDefaultItem)
            this.gameObject.SetActive(collectibleItem.Bought);
    }

    // Start is called before the first frame update
    void Start()
    {
        pointer = Instantiate(pointer, this.transform.position + pointerOffset, Quaternion.Euler(180, 0, 0), this.transform);
        posY = pointer.transform.position.y;
        pointer.transform.localScale = new Vector3(pointerScale, pointerScale, pointerScale);
        pointerRenderer = pointer.GetComponent<Renderer>();
        if (pointerRenderer == null)
            pointerRenderer = pointer.AddComponent<Renderer>();
        pointerRenderer.material = DefaultMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        pointer.transform.Rotate(Vector3.up * pointerRotationSpeed * Time.deltaTime);
        pointer.transform.position = new Vector3(pointer.transform.position.x, posY + (Mathf.Sin(Time.time * pointerMovementSpeed) / 2), pointer.transform.position.z);

    }

    public void Reachable()
    {
        pointerRenderer.material = AlternateMaterial;
    }

    public void Unreachable()
    {
        pointerRenderer.material = DefaultMaterial;
    }

    public void Impossible()
    {
        pointerRenderer.material = ImpossibleMaterial;
    }

    public void PickItem()
    {
        this.gameObject.SetActive(false);
    }

    public void PutDownItem()
    {
        this.gameObject.SetActive(true);
        Unreachable();
    }
}
