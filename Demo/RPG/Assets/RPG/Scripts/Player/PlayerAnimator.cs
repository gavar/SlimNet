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

public class PlayerAnimator : SlimNetMonoBehaviour
{
    [SerializeField]
    Animation target = null;

    byte state = 0;
    Quaternion rotation = Quaternion.identity;

    new void Start()
    {
        base.Start();

        // Setup animations
        target.wrapMode = WrapMode.Loop;

        target["Jump"].wrapMode = WrapMode.Once;
        target["Jump"].layer = 1;

        target["Land"].wrapMode = WrapMode.Once;
        target["Land"].layer = 1;

        target["Run"].speed = 1.75f;
        target["Walk"].speed = -1.25f;
    }

    void Update()
    {
        // Rotate
        switch (state)
        {
            case AnimationEvent.Forward:
                rotate(0);
                break;

            case AnimationEvent.Backward:
                rotate(0);
                break;

            case AnimationEvent.Left:
                rotate(-90);
                break;

            case AnimationEvent.Right:
                rotate(90);
                break;

            case AnimationEvent.ForwardLeft:
                rotate(-45);
                break;

            case AnimationEvent.ForwardRight:
                rotate(45);
                break;

            case AnimationEvent.BackwardLeft:
                rotate(45);
                break;

            case AnimationEvent.BackwardRight:
                rotate(-45);
                break;
        }

        switch (state)
        {
            case AnimationEvent.Idle:
                target.CrossFade("Idle");
                break;

            case AnimationEvent.Fall:
                target.CrossFade("Fall");
                break;

            case AnimationEvent.Forward:
            case AnimationEvent.Left:
            case AnimationEvent.Right:
            case AnimationEvent.ForwardLeft:
            case AnimationEvent.ForwardRight:
                target.CrossFade("Run");
                break;

            case AnimationEvent.Backward:
            case AnimationEvent.BackwardLeft:
            case AnimationEvent.BackwardRight:
                target.CrossFade("Walk");
                break;
        }

        target.transform.rotation = Quaternion.Slerp(target.transform.rotation, rotation, Time.deltaTime * 10f);
    }

    void rotate(float yaw)
    {
        rotation = Quaternion.LookRotation(Quaternion.Euler(0, yaw, 0) * transform.forward, Vector3.up);
    }

    void onPlayerMovement(AnimationEvent ev)
    {
        state = ev.State;

        switch (state)
        {
            case AnimationEvent.Land:
                target.CrossFade("Land");
                break;

            case AnimationEvent.Jump:
                target.CrossFade("Jump");
                break;
        }
    }
}