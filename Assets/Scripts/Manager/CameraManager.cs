using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    private float targetCameraDistance;
    [Header("CameraDistance")]
    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float distanceChangeRate = .5f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("You Have more than 1 Camera Manager");
            Destroy(gameObject);
        }

        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

    }
    private void Update()
    {
        UpdateCameraDistance();

    }

    private void UpdateCameraDistance()
    {
        if (canChangeCameraDistance==false)
        {
            return;
        }
        float currentDistance = transposer.m_CameraDistance;
        //àªç¤¤èÒabsolute¢Í§ÃÐÂÐ¡ÅéÍ§-currentDistance ¶éÒÁÒ¡¡ÇèÒ0.1ãËé¤èÍÂæ»ÃÑºÃÐÂÐËèÒ§¡ÅéÍ§
        //à¾ÃÒÐ¶éÒ»ÃÑºàÅÂã¹VirtualCamera¤èÒ¨ÐÇÔè§ÍÂÙèµÅÍ´àÇÅÒÍÒ¨·ÓãËéà¡Á»Ô§ä´é
        if (Mathf.Abs(targetCameraDistance - currentDistance) > 0.1f)
        {
            transposer.m_CameraDistance =
                Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);

        }
    }


    //¿Ñ§ªÑè¹¹Õéãªéà«ç·ÃÐÂÐËèÒ§¢Í§¡ÅéÍ§
    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;




}
