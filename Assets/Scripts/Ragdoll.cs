using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    [SerializeField] private Collider[] ragdollColider;
    [SerializeField] private Rigidbody[] ragdollRigibody;

    private void Awake()
    {
        ragdollColider = GetComponentsInChildren<Collider>();
        ragdollRigibody = GetComponentsInChildren<Rigidbody>();
        RagdollActive(false);
    }
    public void RagdollActive(bool active)
    {
        foreach(Rigidbody rb in ragdollRigibody)
        {
            rb.isKinematic = !active;
        }
    }
    public void ColliderActive(bool active)
    {
        foreach(Collider cd in ragdollColider)
        {
            cd.enabled = active;
        }
    }

}
