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

using SlimNet;
using SlimNet.Unity;
using UnityEngine;

public class NamePlate : MonoBehaviour
{
    TextMesh textMesh;
    SynchronizedValue<string> playerName;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        playerName = gameObject.GetActorRecursive().GetValue<string>("Name");
    }

    void LateUpdate()
    {
        if (textMesh.text != playerName.Value)
        {
            textMesh.text = playerName.Value;
        }

        Camera cam = Camera.mainCamera;

        if (cam != null)
        {
            // Direction from us to look at position
            Vector3 d = cam.transform.position - transform.position;

            // Use the inverse and create a look rotation
            transform.rotation = Quaternion.LookRotation(-d.normalized);
        }
    }
}