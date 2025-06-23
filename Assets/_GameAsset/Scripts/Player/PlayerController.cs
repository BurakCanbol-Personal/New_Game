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
    [SerializeField] private float _drag = 30f;



    [Header("Camera Settings")]
    [SerializeField] private float _lookSensevityH = 0.1f;
    [SerializeField] private float _lookSensevityV = 0.2f;
    [SerializeField] private float _lookLimitV = 89f;





    private PlayerLocamotionInput _playerLocamotionInput;
    private Vector2 _cameraRotation = Vector2.zero;
    private Vector2 _playerTargetRotation = Vector2.zero;
    private float _yaw;   // left-right (around Y)
    private float _pitch; // up-down (around X)


    private void Awake()
    {
        _playerLocamotionInput = GetComponent<PlayerLocamotionInput>();
    }

    private void Update()
    {
        Vector3 cameraFowardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * _playerLocamotionInput.MovementInput.x + cameraFowardXZ * _playerLocamotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * _runAcceleration * Time.deltaTime;
        Vector3 newVelocity = _characterController.velocity + movementDelta;

        // Add drag to player
        Vector3 currentDrag = newVelocity.normalized * _drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > _drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, _runSpeed);

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
        _yaw   += look.x * _lookSensevityH;
        
        // vertical (invert if you like)
        _pitch -= look.y * _lookSensevityV;
        _pitch  = Mathf.Clamp(_pitch, -_lookLimitV, _lookLimitV);

        // rotate body (yaw) and camera (pitch) separately
        transform.rotation               = Quaternion.Euler(0f, _yaw,   0f);
        _playerCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }
}
