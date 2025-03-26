using UnityEngine;

public class Interactable : MonoBehaviour
{
    protected Player_WeaponController weaponController;
    protected MeshRenderer mesh;
    [SerializeField] private Material highlightMaterial;
    protected Material defaultMaterial;
    private Player player;

    private void Start()
    {
        if (mesh == null)
        {
            mesh = GetComponentInChildren<MeshRenderer>();
        }
        defaultMaterial = mesh.sharedMaterial;
    }

    protected void UpdateMeshAndMaterial(MeshRenderer newmesh)
    {
        mesh = newmesh;
        defaultMaterial = newmesh.sharedMaterial;
    }

    public void HighlightActive(bool active)
    {

        if (active)
        {
            mesh.material = highlightMaterial;
        }
        else
        {
            mesh.material = defaultMaterial;
        }



    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (weaponController == null)
        {
            weaponController = other.GetComponent<Player_WeaponController>();
        }

        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();
        if (playerInteraction == null)
        {
            return;
        }

        playerInteraction.GetInteracbles().Add(this);
        playerInteraction.UpdatateClosestInteractable();

    }
    protected virtual void OnTriggerExit(Collider other)
    {
        Player_Interaction playerInteraction = other.GetComponent<Player_Interaction>();
        if (playerInteraction == null)
        {
            return;
        }
        playerInteraction.GetInteracbles().Remove(this);
        playerInteraction.UpdatateClosestInteractable();
    }
    public virtual void InterAction()
    {
        Debug.Log("Interact with"+ gameObject.name);

    }
}
