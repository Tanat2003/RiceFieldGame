using System.Collections.Generic;
using UnityEngine;

public class Player_Interaction : MonoBehaviour
{
    private List<Interactable> interactables = new List<Interactable>();
    private Interactable closestInteractable;

    private void Start()
    {
        Player player = GetComponent<Player>();
        player.controls.Character.Interaction.performed += context 
            => InteractWithClosest();


    }
    private void InteractWithClosest()
    {
        closestInteractable?.InterAction();
        interactables.Remove(closestInteractable);
        UpdatateClosestInteractable();
    }
    public void UpdatateClosestInteractable()
    {
        closestInteractable?.HighlightActive(false);

        closestInteractable = null;
        float closestDistance = float.MaxValue;
        foreach (Interactable interactable in interactables)
        {
            //เช็คระยะห่าง(จุดต้น,จุดปลาย) (ผู้เล่น,ไอเทมinteractableที่อยู่ในลิสต์)
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;


            }



        }
        closestInteractable?.HighlightActive(true);

       

    }
    public List<Interactable> GetInteracbles() =>interactables;



}
