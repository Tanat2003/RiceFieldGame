using UnityEngine;


[RequireComponent(typeof(Rigidbody))] //����component�ѵ��ѵ�
public class Target : MonoBehaviour
{
    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemy"); //����¹layerMask��Enemy 
    }




}
