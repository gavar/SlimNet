/*
 * SlimNet - Networking Middleware For Games
 * Copyright (C) 2011-2012 Fredrik Holmström
 * 
 * This notice may not be removed or altered.
 * 
 * This software is provided 'as-is', without any expressed or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software. 
 * 
 * Attribution
 * The origin of this software must not be misrepresented; you must not
 * claim that you wrote the original software. For any works using this 
 * software, reasonable acknowledgment is required.
 * 
 * Noncommercial
 * You may not use this software for commercial purposes.
 * 
 * Distribution
 * You are not allowed to distribute or make publicly available the software 
 * itself or its source code in original or modified form.
 */

using UnityEngine;

public class RPGThirdPersonCamera : MonoBehaviour
{
    public static Transform StaticTarget;
    public static RPGThirdPersonCamera Instance { get; private set; }

    float targetYaw;
    float targetPitch;
    float targetDistance;
    float yClamp = float.MinValue;

    float currentYaw;
    float currentPitch;
    float currentDistance;
    float currentMinDistance;
    float currentMaxDistance;

    float realDistance = 0f;

    public Camera Camera = null;
    public Transform Target = null;

    public float MinDistance = 1f;
    public float MaxDistance = 32f;
    public float MinPitch = -80f;
    public float MaxPitch = 80f;
    public float ZoomSpeed = 16f;
    public float RotationMouseSpeed = 4f;

    public bool SmoothZoom = true;
    public float SmoothZoomSpeed = 8f;

    public bool SmoothRotation = true;
    public float SmoothRotationSpeed = 8f;

    public bool SmoothAutoRotation = true;
    public float SmoothAutoRotationSpeed = 4f;

    public LayerMask Obstacles = 0;
    public Vector3 TargetOffset = Vector3.zero;

    public string ZoomAxis = "Mouse ScrollWheel";
    public string YawAxis = "Mouse X";
    public string PitchAxis = "Mouse Y";
    public string MouseRotateButton = "Fire1";
    public string MouseLookButton = "Fire2";

    public bool DisplayDebugGizmos = true;
    public Color DebugColor = Color.white;

    public bool LockCameraBehindTarget { get; set; }
    public bool RotateCameraBehindTarget { get; set; }

    public bool HasCamera { get { return Camera != null; } }
    public bool HasTarget { get { return Target != null; } }
    public Vector3 TargetPosition { get { return HasTarget ? Target.position + TargetOffset : TargetOffset; } }

    void Start()
    {
        Instance = this;

        if (!HasCamera)
        {
            Camera = GetComponentInChildren<Camera>();
        }

        if (!HasTarget)
        {
            Target = StaticTarget;

            if (!HasTarget)
            {
                try
                {
                    Target = GameObject.FindGameObjectWithTag("CameraTarget").transform;
                }
                catch
                {

                }
            }
        }

        MinPitch = Mathf.Clamp(MinPitch, -85f, 0f);
        MaxPitch = Mathf.Clamp(MaxPitch, 0f, 85f);
        MinDistance = Mathf.Max(0f, MinDistance);

        currentMinDistance = MinDistance;
        currentMaxDistance = MaxDistance;

        currentYaw = targetYaw = 0f;
        currentPitch = targetPitch = Mathf.Lerp(MinPitch, MaxPitch, 0.6f);
        currentDistance = targetDistance = realDistance = Mathf.Lerp(MinDistance, MaxDistance, 0.5f);
    }

    void LateUpdate()
    {
        Instance = this;

        if (!HasCamera)
        {
            return;
        }

        if (!HasTarget)
        {
            return;
        }

        bool rotate = RPGControllerUtils.GetButtonSafe(MouseRotateButton, false);
        bool mouseLook = RPGControllerUtils.GetButtonSafe(MouseLookButton, false);

        bool smoothRotation = SmoothRotation || SmoothAutoRotation;
        float smoothRotationSpeed = SmoothRotationSpeed;

        // This defines our "real" distance to the player
        realDistance -= RPGControllerUtils.GetAxisRawSafe(ZoomAxis, 0f) * ZoomSpeed;
        realDistance = Mathf.Clamp(realDistance, MinDistance, MaxDistance);

        // This is the distance we want to (clamped to what is viewable)
        targetDistance = realDistance;
        targetDistance = Mathf.Clamp(targetDistance, currentMinDistance, currentMaxDistance);

        // This is our current distance
        if (SmoothZoom)
        {
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * SmoothZoomSpeed);
        }
        else
        {
            currentDistance = targetDistance;
        }
        
        // Calculate offset vector
        Vector3 offset = new Vector3(0, 0, -currentDistance);

        // LMB is not down, but we should rotate camera behind target
        if(!rotate && RotateCameraBehindTarget)
        {
            targetYaw = RPGControllerUtils.SignedAngle(offset.normalized, -Target.forward, Vector3.up);
            smoothRotation = SmoothAutoRotation;
            smoothRotationSpeed = SmoothAutoRotationSpeed;
        }

        // Only LMB down and no lock
        if (rotate && !mouseLook && !LockCameraBehindTarget)
        {
            targetYaw += (RPGControllerUtils.GetAxisRawSafe(YawAxis, 0f) * RotationMouseSpeed);
            targetPitch -= (RPGControllerUtils.GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
            smoothRotation = SmoothRotation;
            smoothRotationSpeed = SmoothRotationSpeed;
        }

        // RMB 
        if (mouseLook && LockCameraBehindTarget)
        {
            targetPitch -= (RPGControllerUtils.GetAxisRawSafe(PitchAxis, 0f) * RotationMouseSpeed);
            targetPitch = Mathf.Clamp(targetPitch, MinPitch, MaxPitch);
        }

        // Lock camera behind target, this overrides everything
        if (LockCameraBehindTarget)
        {
            targetYaw = RPGControllerUtils.SignedAngle(offset.normalized, -Target.transform.forward, Vector3.up);
            smoothRotation = false;
        }

        // Clamp targetYaw to -180, 180
        targetYaw = Mathf.Repeat(targetYaw + 180f, 360f) - 180f;

        if (!smoothRotation)
        {
            currentYaw = targetYaw;
            currentPitch = targetPitch;
        }
        else
        {
            // Clamp smooth currentYaw to targetYaw and clamp it to -180, 180
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, Time.deltaTime * smoothRotationSpeed);
            currentYaw = Mathf.Repeat(currentYaw + 180f, 360f) - 180f;

            // Smooth pitch
            currentPitch = Mathf.LerpAngle(currentPitch, targetPitch, Time.deltaTime * smoothRotationSpeed);
        }

        // ray hit
        RaycastHit hit = default(RaycastHit);

        // Rotate offset vector
        offset = Quaternion.Euler(currentPitch, currentYaw, 0f) * offset;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2048f, Obstacles) && hit.transform.GetComponent<TerrainCollider>() != null)
        {
            yClamp = hit.point.y + Camera.near;
        }
        else
        {
            yClamp = float.MinValue;
        }

        // Position camera holder correctly
        transform.position = TargetPosition + offset;

        Vector3 p = transform.position;

        transform.position = new Vector3(p.x, Mathf.Clamp(p.y, yClamp, float.MaxValue), p.z);

        //float y = Mathf.Clamp(p.y, yClamp, float.MaxValue)

        // And then have the camera look at our target
        Camera.transform.LookAt(TargetPosition);

        Vector3 targetToCam = transform.position - TargetPosition;

        if (Physics.Raycast(TargetPosition, targetToCam.normalized, out hit, MaxDistance, Obstacles))
        {
            currentMinDistance = Mathf.Min(currentMinDistance, 1f);
            currentMaxDistance = Mathf.Max(currentMinDistance + 0.05f, (hit.point - Target.position).magnitude * 0.95f);
        }
        else
        {
            currentMinDistance = MinDistance;
            currentMaxDistance = MaxDistance;
        }

        // Clear this flag
        LockCameraBehindTarget = false;
        RotateCameraBehindTarget = false;
    }

    void OnDrawGizmos()
    {
        if (DisplayDebugGizmos && HasTarget)
        {
            Vector3 realCamPos = TargetPosition + (Quaternion.Euler(currentPitch, currentYaw, 0f) * (Vector3.back * realDistance));

            Gizmos.color = DebugColor;
            Gizmos.DrawWireSphere(TargetPosition, currentMinDistance);
            Gizmos.DrawWireSphere(TargetPosition, currentMaxDistance);
            Gizmos.DrawLine(TargetPosition, realCamPos);
        }
    }
}
