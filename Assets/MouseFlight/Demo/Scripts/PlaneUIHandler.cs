using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFlight.Demo
{
    public class PlaneUIHandler : MonoBehaviour
    {
        [SerializeField] private Plane plane;
        [SerializeField] private Slider throttle;

        [SerializeField] private Text airBreakNotification;
        [SerializeField] private Text flapNotification;
        [SerializeField] private Vector3UIField velocity;
        [SerializeField] private Vector3UIField drag;
        [SerializeField] private Vector3UIField input;
        [SerializeField] private Vector3UIField yawPitchRoll;
        [SerializeField] private ValueUIField speed;
        [SerializeField] private ValueUIField aoa;
        [SerializeField] private ValueUIField gForce;
        [SerializeField] private ValueUIField altitude;
        [SerializeField] private ValueUIField acceleration;

        void FixedUpdate()
        {
            if (plane == null) return;

            if (throttle != null)
                throttle.value = plane.Throttle;

            if (velocity != null)
                velocity.OnValueChanged(plane.LocalVelocity);

            if (drag != null)
                drag.OnValueChanged(plane.Drag);

            if (input != null)
                input.OnValueChanged(plane.EffectiveInput);

            if (yawPitchRoll != null)
                yawPitchRoll.OnValueChanged(new Vector3(plane.Yaw, plane.Pitch, plane.Roll));

            if (airBreakNotification != null)
                airBreakNotification.enabled = plane.AirBreakDeployed;

            if (flapNotification != null)
                flapNotification.enabled = plane.FlapsDeployed;

            if (speed != null)
                speed.OnValueChanged(plane.Velocity.magnitude.ToString());

            if (aoa != null)
                aoa.OnValueChanged(plane.AngleOfAttack.ToString());

            if (gForce != null)
                gForce.OnValueChanged(plane.LocalGForce.y.ToString());

            if (altitude != null)
                altitude.OnValueChanged(plane.transform.position.y.ToString());
        }
    }
}

