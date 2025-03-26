using UnityEngine;


[RequireComponent(typeof(Rigidbody))] //à¾ÔèÁcomponentÍÑµâ¹ÁÑµÔ
public class Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); //à»ÅÕèÂ¹layerMaskà»ç¹Enemy 
    }




}
