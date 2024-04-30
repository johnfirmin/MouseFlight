using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MFlight.Demo
{
    public class PlayerInputControl : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Plane plane;
        [SerializeField] private MouseFlightController controller;


        [Header("Autopilot")]
        [Tooltip("Sensitivity for autopilot flight.")] public float sensitivity = 5f;
        [Tooltip("Angle at which airplane banks fully into target.")] public float aggressiveTurnAngle = 10f;

        private bool _throttleUp;
        private bool _throttleDown;

        [Header("Runtime Flags")]
        [SerializeField] private bool rollOverride = false;
        [SerializeField] private float keyboardRoll = 0;
        [SerializeField] private bool pitchOverride = false;
        [SerializeField] private float keyboardPitch = 0;
        [SerializeField] private bool yawOverride = false;
        [SerializeField] private float keyboardYaw = 0;

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
            }
            else if (input == -1)
            {
                _throttleUp = false;
                _throttleDown = true;
            }
        }

        public void SetYawInput(InputAction.CallbackContext context)
        {
            keyboardYaw = context.ReadValue<float>();
            if (Mathf.Abs(keyboardYaw) > .25)
                yawOverride = true;
        }

        public void SetPitchInput(InputAction.CallbackContext context)
        {
            keyboardPitch = context.ReadValue<float>();
            if (Mathf.Abs(keyboardPitch) > .25)
                pitchOverride = true;
        }

        public void SetRollInput(InputAction.CallbackContext context)
        {
            keyboardRoll = context.ReadValue<float>();
            if (Mathf.Abs(keyboardRoll) > .25)
                rollOverride = true;
        }

        public void DeployAirBreak(InputAction.CallbackContext context)
        {
            if (plane == null) return;

            //Toggle the air break;
            plane.AirBreakDeployed = !plane.AirBreakDeployed;
        }

        public void DeployFlaps(InputAction.CallbackContext context)
        {
            if (plane == null) return;

            //Toggle the flaps;
            plane.FlapsDeployed = !plane.FlapsDeployed;
        }

        private void Awake()
        {
            plane = GetComponent<Plane>();

            if (controller == null)
                Debug.LogError(name + ": Plane - Missing reference to MouseFlightController!");
        }

        private void Update()
        {
            rollOverride = keyboardRoll != 0;
            pitchOverride = keyboardPitch != 0;
            yawOverride = keyboardYaw != 0;

            var autoYaw = 0f;
            var autoPitch = 0f;
            var autoRoll = 0f;

            // When the player commands their own stick input, it should override what the
            // autopilot is trying to do.
            var autoOff = yawOverride || pitchOverride || rollOverride;
            if (controller != null && !autoOff)
                RunAutopilot(controller.MouseAimPos, out autoYaw, out autoPitch, out autoRoll);


            // Use either keyboard or autopilot input.
            plane.Yaw = autoOff ? keyboardYaw : autoYaw;
            plane.Pitch = autoOff ? keyboardPitch : autoPitch;
            plane.Roll = autoOff ? keyboardRoll : autoRoll;


            if (_throttleUp)
                plane.Throttle += 0.01f;
            else if (_throttleDown)
                plane.Throttle -= 0.01f;
        }

        private void RunAutopilot(Vector3 flyTarget, out float yaw, out float pitch, out float roll)
        {
            // This is my usual trick of converting the fly to position to local space.
            // You can derive a lot of information from where the target is relative to self.
            var localFlyTarget = transform.InverseTransformPoint(flyTarget).normalized * sensitivity;
            var angleOffTarget = Vector3.Angle(transform.forward, flyTarget - transform.position);

            // IMPORTANT!
            // These inputs are created proportionally. This means it can be prone to
            // overshooting. The physics in this example are tweaked so that it's not a big
            // issue, but in something with different or more realistic physics this might
            // not be the case. Use of a PID controller for each axis is highly recommended.

            // ====================
            // PITCH AND YAW
            // ====================

            // Yaw/Pitch into the target so as to put it directly in front of the aircraft.
            // A target is directly in front the aircraft if the relative X and Y are both
            // zero. Note this does not handle for the case where the target is directly behind.
            yaw = Mathf.Clamp(localFlyTarget.x, -1f, 1f);
            pitch = -Mathf.Clamp(localFlyTarget.y, -1f, 1f);

            // ====================
            // ROLL
            // ====================

            // Roll is a little special because there are two different roll commands depending
            // on the situation. When the target is off axis, then the plane should roll into it.
            // When the target is directly in front, the plane should fly wings level.

            // An "aggressive roll" is input such that the aircraft rolls into the target so
            // that pitching up (handled above) will put the nose onto the target. This is
            // done by rolling such that the X component of the target's position is zeroed.
            var agressiveRoll = Mathf.Clamp(localFlyTarget.x, -1f, 1f);

            // A "wings level roll" is a roll commands the aircraft to fly wings level.
            // This can be done by zeroing out the Y component of the aircraft's right.
            var wingsLevelRoll = transform.right.y;

            // Blend between auto level and banking into the target.
            var wingsLevelInfluence = Mathf.InverseLerp(0f, aggressiveTurnAngle, angleOffTarget);
            roll = Mathf.Lerp(wingsLevelRoll, agressiveRoll, wingsLevelInfluence);
        }
    }
}
