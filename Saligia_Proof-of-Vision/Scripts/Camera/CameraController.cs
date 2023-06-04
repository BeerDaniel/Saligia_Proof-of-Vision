using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [System.Serializable]
    public struct CameraSettings
    {
        [Range(30, 90), Tooltip("The tilt angle of the camera. -1 = ignore")] public float xRotation;
        [Range(-180, 180)] public float yRotation;
        [Tooltip("Zoom of the Camera. -1 = ignore")] public float ortographicSize;
    }

    [SerializeField] private Camera _indicatorCam;
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private float _turnSpeed;
    private CameraSettings _spawnSettings;
    private Vector3 _eulerVec;
    private float _rotateValue;
    private Vector3 _eulerAngles;

    private void Start()
    {
        _spawnSettings = new CameraSettings();
        _spawnSettings.xRotation = transform.rotation.eulerAngles.x;
        _spawnSettings.yRotation = transform.rotation.eulerAngles.y;
        _spawnSettings.ortographicSize = _cinemachineVirtualCamera.m_Lens.OrthographicSize;
    }

    private void OnEnable()
    {
        GameEvents.changeCameraSettingsEvent += OnChangeCameraSettings;
        GameEvents.cameraRotateEvent += OnCameraRotate;
    }

    private void OnDisable()
    {
        GameEvents.changeCameraSettingsEvent -= OnChangeCameraSettings;
        GameEvents.cameraRotateEvent -= OnCameraRotate;
    }

    private void Update()
    {
        if (_rotateValue != 0)
        {
            _eulerAngles = transform.eulerAngles;
            _eulerAngles.y += _rotateValue * Time.deltaTime * _turnSpeed;
            transform.eulerAngles = _eulerAngles;
            _indicatorCam.transform.rotation = transform.rotation;
        }
        _indicatorCam.transform.position = _cinemachineVirtualCamera.transform.position;
    }

    private void OnChangeCameraSettings(CameraSettings settings)
    {
        if (settings.ortographicSize != -1)
            _cinemachineVirtualCamera.m_Lens.OrthographicSize = settings.ortographicSize;

        _eulerVec = Vector3.zero;
        _eulerVec.z = transform.eulerAngles.z;

        if (settings.xRotation != -1)
            _eulerVec.x = settings.xRotation;
        if (settings.yRotation != -1)
            _eulerVec.y = settings.yRotation;

        transform.eulerAngles = _eulerVec;
    }

    private void OnCameraRotate(float obj)
    {
        _rotateValue = obj;
    }
}
