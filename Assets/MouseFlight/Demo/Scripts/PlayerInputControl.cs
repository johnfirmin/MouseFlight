using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputControl : MonoBehaviour
{
    public MFlight.Demo.Plane plane;

    [SerializeField]
    private bool _throttleUp;

    [SerializeField]
    private bool _throttleDown;
    public void SetThrottleInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        var input = context.ReadValue<float>();
        if (context.canceled)
        {
            _throttleUp = false;
            _throttleDown = false;
        }

        if (input == 1)
        {
            _throttleUp = true;
            _throttleDown = false;

            Debug.Log($"_throttleUp {_throttleUp}");
        }
        else if (input == -1)
        {
            _throttleUp = false;
            _throttleDown = true; 
            Debug.Log($"_throttleDown {_throttleDown}");
        }
    }

    public void OnRollPitchInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;

        var input = context.ReadValue<Vector2>();
       // plane.Roll = input.x;
       // plane.Pitch = input.y;

       // Debug.Log($"Pitch {plane.Pitch} | Yaw {plane.Roll}");
    }

    public void OnYawInput(InputAction.CallbackContext context)
    {
        if (plane == null) return;
    }

    private void Update()
    {
        if (_throttleUp)
            plane.Throttle += 0.01f;
        else if (_throttleDown)
            plane.Throttle -= 0.01f;
    }
}
