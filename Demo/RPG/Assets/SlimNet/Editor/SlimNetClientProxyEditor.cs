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
using UnityEditor;
using System.Collections;
using SlimNet;

[CustomEditor(typeof(SlimNetClientProxy))]
public class SlimNetClientProxyEditor : Editor
{
    void OnGUI()
    {
        Repaint();
    }

    void Update()
    {
        Repaint();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SlimNetClientProxy proxy = target as SlimNetClientProxy;

        if (proxy != null)
        {
            SlimNet.Unity.Client client = proxy.Instance;

            if (client != null)
            {
                EditorGUILayout.LabelField("Spatial Partitioner", client.Context.HasSpatialPartitioner ? client.Context.SpatialPartitioner.GetTypeName() : "<Null>");
            }
        }
    }
}