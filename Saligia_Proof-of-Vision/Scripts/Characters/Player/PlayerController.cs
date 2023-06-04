using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _camera;
    public Animator Animator => _animator;

    private bool _isGrounded;

    public bool IsRespawning { get; private set; }

    #region Movement Variables
    [SerializeField] private float _gravity = 20f;
    [SerializeField] private float _maxForwardSpeed = 8f;
    private Vector2 _moveInput;
    private float _forwardSpeed;
    private float _desiredForwardSpeed;
    private float _verticalSpeed;
    private bool _ignoreMoveInput;

    private bool HasMoveInput => _moveInput != Vector2.zero;

    const float k_GroundedRayDistance = 1f;
    const float k_GroundAcceleration = 20f;
    const float k_GroundDeceleration = 25f;
    const float k_StickingGravityProportion = 0.3f;
    #endregion

    private void Start()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        GameEvents.moveEvent += OnMove;
        GameEvents.targetingStartEvent += OnAbilityTargetingStart;
        GameEvents.targetingEndEvent += OnAbilityTargetingEnd;
    }

    private void OnDisable()
    {
        GameEvents.moveEvent -= OnMove;
        GameEvents.targetingStartEvent -= OnAbilityTargetingStart;
        GameEvents.targetingEndEvent -= OnAbilityTargetingEnd;
    }

    private void FixedUpdate()
    {
        CalculateForwardMovement();
        CalculateVerticalMovement();
    }

    private void OnAnimatorMove()
    {
        Vector3 movement;

        if (_isGrounded)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, -Vector3.up);
            if (Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                movement = Vector3.ProjectOnPlane(_animator.deltaPosition, hit.normal);
            }
            else
            {
                movement = _animator.deltaPosition;
            }
        }
        else
        {
            movement = _forwardSpeed * transform.forward * Time.deltaTime;
        }

        movement += _verticalSpeed * Vector3.up * Time.deltaTime;
        //movement = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * movement;
        //_characterController.transform.rotation *= _animator.deltaRotation;

        var rot = new Vector3(_moveInput.x, 0, _moveInput.y);
        rot = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * rot;
        if (rot != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(rot, Vector3.up);

        _characterController.Move(movement);

        _isGrounded = _characterController.isGrounded;
    }

    private void CalculateForwardMovement()
    {
        Vector2 moveInput = _moveInput;
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();

        _desiredForwardSpeed = moveInput.magnitude * _maxForwardSpeed;

        float acceleration = HasMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

        _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredForwardSpeed, acceleration * Time.deltaTime);
        _animator.SetFloat("speed", _forwardSpeed / _maxForwardSpeed);
    }

    private void CalculateVerticalMovement()
    {
        if (_isGrounded)
        {
            _verticalSpeed = -_gravity * k_StickingGravityProportion;
        }
        else
        {
            _verticalSpeed -= _gravity * Time.deltaTime;
        }
    }

    private void OnMove(Vector2 vec)
    {
        if (_ignoreMoveInput)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = vec;
        //var rot = new Vector3(vec.x, 0, vec.y);
        //if (rot != Vector3.zero)
        //    transform.rotation = Quaternion.LookRotation(rot, Vector3.up);
    }

    private void OnAbilityTargetingStart()
    {
        _ignoreMoveInput = true;
    }

    private void OnAbilityTargetingEnd()
    {
        _ignoreMoveInput = false;
    }

    public Vector3 footIKOffset;

    private void OnAnimatorIK(int layerIndex)
    {
        var ikWeight = Mathf.Clamp(1 - (_forwardSpeed / _maxForwardSpeed), 0.01f, 1);

        Vector3 pLeftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
        Vector3 pRightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;

        pLeftFoot = GetHitPoint(pLeftFoot + Vector3.up, pLeftFoot - Vector3.up * 5) + footIKOffset;
        pRightFoot = GetHitPoint(pRightFoot + Vector3.up, pRightFoot - Vector3.up * 5) + footIKOffset;

        _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, ikWeight);
        _animator.SetIKPosition(AvatarIKGoal.LeftFoot, pLeftFoot);
        _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, ikWeight);
        _animator.SetIKPosition(AvatarIKGoal.RightFoot, pRightFoot);
    }

    private Vector3 GetHitPoint(Vector3 start, Vector3 end)
    {
        if (Physics.Linecast(start, end, out RaycastHit hit))
        {
            return hit.point;
        }
        return end;
    }
}
