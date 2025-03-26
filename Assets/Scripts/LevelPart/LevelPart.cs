using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelPart : MonoBehaviour
{
    [Header("InterSectionCheck")]
    [SerializeField] private LayerMask intersectionLayer;
    [SerializeField] private Collider[] intersectionCheckColliders;
    [SerializeField] private Transform intersectionParent;

    public bool InterSectionDetected()
    {
        Physics.SyncTransforms(); //�����physic��ҧ�ӧҹ��ʹ���������Ҩ�ź���ҧ�ѵ������ �����physic.���ҧ���ѹ�Ѿഷ���ѹ    
        
        foreach(var collider in intersectionCheckColliders)
        {
            Collider[] hitColliders = 
      Physics.OverlapBox(collider.bounds.center,collider.bounds.extents,Quaternion.identity,intersectionLayer);
            foreach(var hit in hitColliders)
            {
                IntersectionCheck intersectionCheck = hit.GetComponentInParent<IntersectionCheck>();
                if(intersectionCheck != null && intersectionParent != intersectionCheck.transform)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public void SnapAndAlignPartTo(SnapPoint targetSnapPoint)
    {
        SnapPoint entrancePoint = GetEnterPoint();
        AlignTo(entrancePoint, targetSnapPoint); //Alignto ��͹Snap
        SnapTo(entrancePoint, targetSnapPoint);

    }

    private void AlignTo(SnapPoint ownSnapPoint,SnapPoint targetSnapPoint)
    {;
        //�ӹǳ���˹�rotation �����ҧlevelpart�Ѩ�غѹ�Ѻ�ѹ�Ѵ�(㹡óշ��levelpart�Ѵ���enter�ҡ����1)
        var rotationOffset =
            ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        //���levelpart rotation ���targetsnappoint �ҡ�����ع180ͧ��᡹y����snapPoint ⴹ�ҧ��㹵��˹�-180ͧ��
        transform.rotation = targetSnapPoint.transform.rotation;
        transform.Rotate(0, 180, 0);
        transform.Rotate(0,-rotationOffset, 0); //�ѹ�����ҡ���playTest
        
    }
    private void SnapTo(SnapPoint ownSnapPoint,SnapPoint targetSnapPoint)
    {
      
        //��Ѻ���˹�exit,enter�ͧsnapPoint����͡Ѻ�ѹ�Ѵ�
        //�ӹǹoffset �����ҧ �ç��ҧ�ͧsnapPart ��� ���˹觢ͧ EnterPoint���ͷ�����ҹ�snapPart�ҵ�� ������Ẻ Exit -Enter
        var offset = transform.position - ownSnapPoint.transform.position;
        var newPosition = targetSnapPoint.transform.position + offset;

        transform.position = newPosition;
        
    }
    public SnapPoint GetEnterPoint() => GetSnapPointOfType(SnapPointType.Enter);
    public SnapPoint GetExitPoint() => GetSnapPointOfType(SnapPointType.Exit);
    private SnapPoint GetSnapPointOfType(SnapPointType pointType)
    {
        SnapPoint[] snapPoints = GetComponentsInChildren<SnapPoint>();
        List<SnapPoint> filterSnapPoint = new List<SnapPoint>();

        foreach (SnapPoint snapPoint in snapPoints) //����snapPoint�ء�ѹ���= pointtype��������
        {
            if(snapPoint.pointType == pointType)
            {
                filterSnapPoint.Add(snapPoint);
            }
        }
        if(filterSnapPoint.Count > 0)
        {
            int RandomIndex = Random.Range(0, filterSnapPoint.Count);
            return filterSnapPoint[RandomIndex]; //Return SnapPoint���random��
        } 
        return null;
    }
 
    
}
