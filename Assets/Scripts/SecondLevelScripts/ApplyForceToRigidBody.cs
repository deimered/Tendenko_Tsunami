using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForceToRigidBody : MonoBehaviour
{
    public Vector3 force;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Rigidbody[] bodiesToApllieForce;

    private Rigidbody[] rigidBodies;
    private CharacterJoint[] joints;
    private Collider[] colliders;

    private void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        joints = GetComponentsInChildren<CharacterJoint>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void ApplyForce(Vector3 force, ForceMode forceMode)
    {
        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.AddForce(force, forceMode);
        }
    }

    public void EnableRagdoll()
    {
        if (animator != null)
            animator.enabled = false;
        foreach (Collider c in colliders)
        {
            c.enabled = true;
        }

        foreach (CharacterJoint j in joints)
        {
            j.enableCollision = true;
        }

        foreach(Rigidbody r in rigidBodies)
        {
            r.velocity = Vector3.zero;
            r.detectCollisions = true;
            r.useGravity = true;
        }
    }

    public void EnableAnimator()
    {
        if (animator != null)
            animator.enabled = true;
        foreach (Collider c in colliders)
        {
            c.enabled = false;
        }

        foreach (CharacterJoint j in joints)
        {
            j.enableCollision = false;
        }

        foreach (Rigidbody r in rigidBodies)
        {
            r.detectCollisions = false;
            r.useGravity = false;
        }
    }
}
