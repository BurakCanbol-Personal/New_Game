using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementState CurrentMovementState { get; private set; } = PlayerMovementState.Idling;

    public void SetPlayerMovementState(PlayerMovementState newState)
    {
        CurrentMovementState = newState;
    }

}

public enum PlayerMovementState
    {
        Idling = 0,
        Walking = 1,
        Running = 2,
        Sprinting = 3,
        Jumping = 4,
        Falling = 5,
        Strafing = 6,
    }
