using UnityEngine;
using Cinemachine;

public class ChangeCameraSettingsListener : MonoBehaviour
{
    [SerializeField] private CameraController.CameraSettings _cameraSettings;
    [SerializeField] private CinemachineVirtualCamera _previewCamera;
    [SerializeField] private bool _focusPreviewCamera;

    private Camera _mainCam;
    private CinemachineBrain _cinemachineBrain;
    private Vector3 _eulerVec;

    private void Start()
    {
        _mainCam = Camera.main;
        _cinemachineBrain = _mainCam.GetComponent<CinemachineBrain>();
        _focusPreviewCamera = false;
        _previewCamera.Priority = -1;
        _previewCamera.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _previewCamera.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;
        GameEvents.changeCameraSettingsEvent?.Invoke(_cameraSettings);
    }

    private void OnValidate()
    {
        if (_cameraSettings.ortographicSize != -1)
            _previewCamera.m_Lens.OrthographicSize = _cameraSettings.ortographicSize;

        _eulerVec = Vector3.zero;
        _eulerVec.z = transform.eulerAngles.z;

        if (_cameraSettings.xRotation != -1)
            _eulerVec.x = _cameraSettings.xRotation;
        if (_cameraSettings.yRotation != -1)
            _eulerVec.y = _cameraSettings.yRotation;

        _previewCamera.transform.parent.eulerAngles = _eulerVec;
        if (_focusPreviewCamera)
            _previewCamera.Priority = 100;
        else
            _previewCamera.Priority = -1;
    }
}
