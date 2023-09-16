using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Waypoint : MonoBehaviour
{
    public event EventHandler OnEnter;
    public Vector3 colliderSize;
    public bool isTrigger = true;
    public bool ignoreChildCollisions;

    private BoxCollider box;
    private Rigidbody rb;
    [SerializeField]
    private Vector3 boxPos;
    // Start is called before the first frame update
    void Start()
    {
        box = this.gameObject.AddComponent<BoxCollider>();
        box.isTrigger = isTrigger;
        box.size = colliderSize;
        rb = this.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        box.center = boxPos;
    }
    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke(this, EventArgs.Empty);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.IsChildOf(transform) && ignoreChildCollisions)
            OnEnter?.Invoke(this, EventArgs.Empty);
    }

    public void SetBoxSize(Vector3 size)
    {
        box.size = size;
    }
}
