using System;
using JetBrains.Rider.Unity.Editor;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Camera _playerCamera;



    [Header("Movement Settings")]
    [SerializeField] private float _runAcceleration = 50f;
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private float _sprintAcceleration = 100f;
    [SerializeField] private float _sprintSpeed = 8f;
    [SerializeField] private float _drag = 30f;
    [SerializeField] private float _movingThreshold = 0.01f;



    [Header("Camera Settings")]
    [SerializeField] private float _lookSensevityH = 0.1f;
    [SerializeField] private float _lookSensevityV = 0.2f;
    [SerializeField] private float _lookLimitV = 89f;




    private PlayerState _playerState;
    private PlayerLocamotionInput _playerLocamotionInput;
    private Vector2 _cameraRotation = Vector2.zero;
    private Vector2 _playerTargetRotation = Vector2.zero;
    private float _yaw;   // left-right (around Y)
    private float _pitch; // up-down (around X)


    private void Awake()
    {
        _playerLocamotionInput = GetComponent<PlayerLocamotionInput>();
        _playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateMovementState();
        HandlerLateralMovement();
    }

    private void UpdateMovementState()
    {
        bool isMovementInput = _playerLocamotionInput.MovementInput != Vector2.zero;
        bool isMovingLaterally = IsMovingLaterally();
        bool isSprinting = _playerLocamotionInput.SprintOn && isMovingLaterally;

        PlayerMovementState lateralState =  isSprinting ? PlayerMovementState.Sprinting :
                                            isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

        _playerState.SetPlayerMovementState(lateralState);
    }

    private void HandlerLateralMovement()
    {
        // create quic reference for current state
        bool isSprinting = _playerState.CurrentMovementState == PlayerMovementState.Sprinting;

        // state dependent acceleration and top speed
        float lateralAcceleration = isSprinting ? _sprintAcceleration : _runAcceleration;
        float clampLateralSpeed = isSprinting ? _sprintSpeed : _runSpeed;


        Vector3 cameraFowardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * _playerLocamotionInput.MovementInput.x + cameraFowardXZ * _playerLocamotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        // Add drag to player
        Vector3 currentDrag = newVelocity.normalized * _drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > _drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralSpeed);

        _characterController.Move(newVelocity * Time.deltaTime);
    }

    private void LateUpdate()
    {
        // _cameraRotation.x += _playerLocamotionInput.LookInput.x * _lookSensevityH;
        // _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - _lookSensevityV - _playerLocamotionInput.LookInput.y, -_lookLimitV, _lookLimitV);

        // _cameraRotation.x += transform.eulerAngles.x + _lookSensevityH * _playerLocamotionInput.LookInput.x;
        // transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

        // _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

        Vector2 look = _playerLocamotionInput.LookInput; // mouse Δ or right-stick

        // horizontal
        _yaw += look.x * _lookSensevityH;

        // vertical (invert if you like)
        _pitch -= look.y * _lookSensevityV;
        _pitch = Mathf.Clamp(_pitch, -_lookLimitV, _lookLimitV);

        // rotate body (yaw) and camera (pitch) separately
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        _playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);
        return lateralVelocity.magnitude > _movingThreshold;
    }
}
