using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFlight.Demo
{
    [RequireComponent(typeof(Camera))]
    public class PlaneCameraControl : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Plane plane;
        [SerializeField] private Camera targetCamera;

        [SerializeField] private float baseFov = 60;
        [SerializeField] private float targetFOVDelta = 5;
        [SerializeField] private AnimationCurve fovCurve;

        private void Awake()
        {
            targetCamera = GetComponent<Camera>();
        }

        void FixedUpdate()
        {
            if (plane == null && targetCamera != null) return;

            //Extremely basic camera perspective change to give the Player a scene of the speed based on maximum thrust being applied via the throttle
            var precent = plane.Velocity.magnitude / (plane.minThrust + plane.maximumThrust);
            var targetFov = fovCurve.Evaluate(precent);
            targetCamera.fieldOfView = baseFov + (targetFov * targetFOVDelta);
        }
    }
}
