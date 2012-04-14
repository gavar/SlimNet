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
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : SlimNetMonoBehaviour
{
    bool grounded = false;
    byte currentState = 0;

    Rigidbody body;
    CapsuleCollider capsule;

    float runSpeed = 4f;
    float jumpForce = 5f;
    float groundedTime = 0f;

    [SerializeField]
    LayerMask walkable = 0;
    
    new void Start()
    {
        capsule = collider as CapsuleCollider;

        if (networkActorIsMine)
        {
            // We will rotate ourselves and we dont want any drag
            body = rigidbody;
            body.drag = 0f;
            body.freezeRotation = true;

            // Set camera to target this transform
            RPGThirdPersonCamera.StaticTarget = transform;
        }
        else
        {
            rigidbody.isKinematic = true;
            Destroy(this);
        }
    }

    void changeMovementState(byte state)
    {
        if (networkActorIsMine && currentState != state)
        {
            currentState = state;
            networkActor.RaiseEvent<AnimationEvent>((ev) => ev.State = state);
        }
    }

    void FixedUpdate()
    {
        if (networkActorIsMine && body != null && capsule != null)
        {
            bool wasGrounded = grounded;

            grounded = Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1.05f, walkable);

            if (grounded && !wasGrounded)
            {
                if (Time.time - groundedTime > 0.4f)
                {
                    changeMovementState(AnimationEvent.Land);
                }
            }

            if (grounded)
            {
                groundedTime = Time.time;
            }

            if (Input.GetMouseButton(1))
            {
                transform.Rotate(Vector3.up, Input.GetAxisRaw("Mouse X") * 200f * Time.fixedDeltaTime);
                RPGThirdPersonCamera.Instance.LockCameraBehindTarget = true;
            }

            if (grounded)
            {
                Vector3 movement =
                    Input.GetAxisRaw("Horizontal") * Vector3.right +
                    Input.GetAxisRaw("Vertical") * Vector3.forward;

                if (movement != Vector3.zero)
                {
                    RPGThirdPersonCamera.Instance.RotateCameraBehindTarget = true;

                    movement.Normalize();
                    movement = transform.rotation * movement * runSpeed;

                    var a = RPGControllerUtils.SignedAngle(transform.forward, movement.normalized, Vector3.up);
                    var r = a > 1;

                    switch (Mathf.RoundToInt(Mathf.Abs(a)))
                    {
                        case 0:
                            changeMovementState(AnimationEvent.Forward);
                            break;

                        case 45:
                            changeMovementState(r ? AnimationEvent.ForwardRight : AnimationEvent.ForwardLeft);
                            break;

                        case 90:
                            changeMovementState(r ? AnimationEvent.Right : AnimationEvent.Left);
                            break;

                        case 135:
                            changeMovementState(r ? AnimationEvent.BackwardRight : AnimationEvent.BackwardLeft);
                            break;

                        case 180:
                            changeMovementState(AnimationEvent.Backward);
                            break;
                    }

                    if (Mathf.Abs(a) > 91)
                    {
                        movement *= 0.5f;
                    }

                    body.velocity = movement;
                }
                else
                {
                    changeMovementState(AnimationEvent.Idle);
                    body.velocity = new Vector3(0, body.velocity.y, 0);
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    grounded = false;
                    body.velocity += Vector3.up * jumpForce;
                    changeMovementState(AnimationEvent.Jump);
                }
            }
            else
            {
                if (Time.time - groundedTime > 0.4f)
                {
                    changeMovementState(AnimationEvent.Fall);
                }
            }
        }
    }
}
