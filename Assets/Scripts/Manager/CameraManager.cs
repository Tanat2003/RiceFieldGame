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
        //�礤��absolute�ͧ���С��ͧ-currentDistance ����ҡ����0.1��������Ѻ������ҧ���ͧ
        //���ж�һ�Ѻ����VirtualCamera��Ҩ���������ʹ�����Ҩ��������ԧ��
        if (Mathf.Abs(targetCameraDistance - currentDistance) > 0.1f)
        {
            transposer.m_CameraDistance =
                Mathf.Lerp(currentDistance, targetCameraDistance, distanceChangeRate * Time.deltaTime);

        }
    }


    //�ѧ��蹹������������ҧ�ͧ���ͧ
    public void ChangeCameraDistance(float distance) => targetCameraDistance = distance;




}
