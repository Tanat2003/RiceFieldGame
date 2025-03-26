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
        Physics.SyncTransforms(); //∑”„ÀÈphysicµË“ßÊ∑”ß“πµ≈Õ¥‡«≈“·¡È«Ë“®–≈∫ √È“ß«—µ∂ÿ„À¡Ë ∂È“„™Èphysic.‡©¬Ê∫“ß∑’¡—πÕ—æ‡¥∑‰¡Ë∑—π    
        
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
        AlignTo(entrancePoint, targetSnapPoint); //Alignto °ËÕπSnap
        SnapTo(entrancePoint, targetSnapPoint);

    }

    private void AlignTo(SnapPoint ownSnapPoint,SnapPoint targetSnapPoint)
    {;
        //§”π«≥µ”·ÀπËßrotation √–À«Ë“ßlevelpartª—®®ÿ∫—π°—∫Õ—π∂—¥‰ª(„π°√≥’∑’Ëlevelpart∂—¥‰ª¡’enter¡“°°«Ë“1)
        var rotationOffset =
            ownSnapPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y;
        //„ÀÈlevelpart rotation µ“¡targetsnappoint ®“°π—ÈπÀ¡ÿπ180Õß»“·°πy‡æ√“–snapPoint ‚¥π«“ß¡“„πµ”·ÀπËß-180Õß»“
        transform.rotation = targetSnapPoint.transform.rotation;
        transform.Rotate(0, 180, 0);
        transform.Rotate(0,-rotationOffset, 0); //Õ—ππ’È‰¥È®“°°“√playTest
        
    }
    private void SnapTo(SnapPoint ownSnapPoint,SnapPoint targetSnapPoint)
    {
      
        // ≈—∫µ”·ÀπËßexit,enter¢ÕßsnapPoint„ÀÈµËÕ°—∫Õ—π∂—¥‰ª
        //§”π«πoffset √–À«Ë“ß µ√ß°≈“ß¢ÕßsnapPart ·≈– µ”·ÀπËß¢Õß EnterPoint‡æ◊ËÕ∑’Ë‡«≈“π”snapPart¡“µËÕ ®–‰¥ÈµËÕ·∫∫ Exit -Enter
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

        foreach (SnapPoint snapPoint in snapPoints) //‡æ‘Ë¡snapPoint∑ÿ°Õ—π∑’Ë= pointtype‡¢È“‰ª„π≈‘ 
        {
            if(snapPoint.pointType == pointType)
            {
                filterSnapPoint.Add(snapPoint);
            }
        }
        if(filterSnapPoint.Count > 0)
        {
            int RandomIndex = Random.Range(0, filterSnapPoint.Count);
            return filterSnapPoint[RandomIndex]; //Return SnapPoint∑’Ërandom¡“
        } 
        return null;
    }
 
    
}
