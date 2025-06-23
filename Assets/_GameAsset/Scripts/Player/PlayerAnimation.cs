using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float locomotionBlendSpeed = 4f;

    private PlayerLocamotionInput _playerLocomotionInput;
    private PlayerState _playerState;

    private static int inputXHash = Animator.StringToHash("inputX");
    private static int inputYHash = Animator.StringToHash("inputY");
    private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");

    private Vector3 _currentBlendInput = Vector3.zero;


    private void Awake()
    {
        _playerLocomotionInput = GetComponent<PlayerLocamotionInput>();
        _playerState = GetComponent<PlayerState>();

    }


    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isSprinting = _playerLocomotionInput.SprintOn && _playerState.CurrentMovementState == PlayerMovementState.Sprinting;

        Vector2 inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * 1.5f : _playerLocomotionInput.MovementInput;



        if ((inputTarget.x == -1 && inputTarget.y == 0) || (inputTarget.x == 1 && inputTarget.y == 0))
        {
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, 2.5f * locomotionBlendSpeed * Time.deltaTime);
        }
        else
        {
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
        }

        _animator.SetFloat(inputXHash, _currentBlendInput.x);
        _animator.SetFloat(inputYHash, _currentBlendInput.y);
        _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
    }
}
