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

//#define SLIMNET_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using SlimNet;
using SlimNet.Unity;
using SlimNet.Utils;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ActorProxy))]
public class SlimNetActorProxyEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        ActorProxy proxy = (ActorProxy)target;

        if (Application.isPlaying && PrefabUtility.GetPrefabType(proxy.gameObject) != PrefabType.Prefab && proxy.Actor != null)
        {
            GUIStyle indentStyle = new GUIStyle();
            indentStyle.margin = new RectOffset(20, 0, 0, 0);

            SlimNetGUILayout.Label("Runtime Settings", EditorStyles.boldLabel);

            proxy.DisplayCollider = EditorGUILayout.Toggle("Display Collider", proxy.DisplayCollider);

            SlimNetGUILayout.Label("Properties", EditorStyles.boldLabel);

            using (new SlimNetGUILayout.Horizontal())
            {
                // Labels
                using (new SlimNetGUILayout.Vertical())
                {
                    GUILayout.Label("Name");
                    GUILayout.Label("Owner Id");
                    GUILayout.Label("Role");
                    GUILayout.Label("Is Mine");
                    GUILayout.Label("Is Idle");
                    GUILayout.Label("Owned By Server");
                    GUILayout.Label("Definition Type");
                    GUILayout.Label("Transform Source");
                    GUILayout.Label("Stream Time");
                    GUILayout.Label("Simulation Offset");
                    GUILayout.Label("Collider");
                    GUILayout.Label("State Streamer");
                }

                // Values
                using (new SlimNetGUILayout.Vertical())
                {

                    GUILayout.Label(proxy.Actor.Name);
                    GUILayout.Label(proxy.Actor.PlayerId.ToString());
                    GUILayout.Label(proxy.Actor.Role.ToString());
                    GUILayout.Label(proxy.Actor.IsMine.ToString());
                    GUILayout.Label(proxy.Actor.IsIdle.ToString());
                    GUILayout.Label(proxy.Actor.IsOwnedByServer.ToString());
                    GUILayout.Label(proxy.Actor.Definition.GetType().GetPrettyName());
                    GUILayout.Label(proxy.Actor.TransformSource.ToString());
                    GUILayout.Label(proxy.Actor.StateStreamTime.ToString() + " seconds");
                    GUILayout.Label(proxy.Actor.SimulationTimeOffset.ToString() + " seconds");
                    GUILayout.Label(proxy.Actor.HasCollider ? proxy.Actor.Collider.GetTypeName() : "<Null>");
                    GUILayout.Label(proxy.Actor.HasStateStream ? proxy.Actor.StateStreamer.GetTypeName() : "<Null>");
                }
            }

            SlimNetGUILayout.Label("Synchronized Values", EditorStyles.boldLabel);

            if (proxy.Actor.SynchronizedValues.Length == 0)
            {
                SlimNetGUILayout.Label("No synchronized values attached", SlimNetGUIStyles.GrayText);
            }
            else
            {
                using (new SlimNetGUILayout.Horizontal())
                {
                    // Labels
                    using (new SlimNetGUILayout.Vertical())
                    {
                        foreach (string name in proxy.Actor.SynchronizedValues.Select(x => x.Key))
                        {
                            GUILayout.Label(name);
                        }
                    }

                    // Values
                    using (new SlimNetGUILayout.Vertical())
                    {
                        foreach (SynchronizedValue value in proxy.Actor.SynchronizedValues.Select(x => x.Value))
                        {
                            if (value.BoxedValue == null)
                            {
                                GUILayout.Label("<Null>");
                            }
                            else
                            {
                                GUILayout.Label(value.BoxedValue.ToString());
                            }
                        }
                    }
                }
            }

            SlimNetGUILayout.Label("Event Handlers", EditorStyles.boldLabel);

            List<Pair<Type, List<Delegate>>> attachedHandlers = proxy.Actor.ActiveEventHandlers;

            if (attachedHandlers.Count == 0)
            {
                SlimNetGUILayout.Label("No event handlers attached", SlimNetGUIStyles.GrayText);
            }
            else
            {
                using (new SlimNetGUILayout.Vertical())
                {
                    foreach (var pair in attachedHandlers)
                    {
                        SlimNetGUILayout.Label(pair.First.GetPrettyName(), SlimNetGUIStyles.GrayText);

                        foreach (Delegate handler in pair.Second)
                        {
                            SlimNetGUILayout.Label(handler.Method.GetPrettyName(), indentStyle);
                        }
                    }
                }
            }

            SlimNetGUILayout.Label("Behaviours", EditorStyles.boldLabel);

            Type[] types = proxy.Actor.AttachedBehaviourTypes.ToArray();

            if (types.Length == 0)
            {
                SlimNetGUILayout.Label("No custom behaviours attached", SlimNetGUIStyles.GrayText);
            }
            else
            {
                foreach (Type type in types)
                {
                    GUILayout.Label(type.GetPrettyName(), SlimNetGUIStyles.GrayText);
                    SlimNet.Pair<string, object>[] debugValues = proxy.Actor.GetBehaviour(type).DebugValues;

                    using (new SlimNetGUILayout.Horizontal(indentStyle))
                    {
                        // Labels
                        using (new SlimNetGUILayout.Vertical())
                        {
                            foreach (SlimNet.Pair<string, object> val in debugValues)
                            {
                                GUILayout.Label(val.First ?? "");
                            }
                        }

                        // Values
                        using (new SlimNetGUILayout.Vertical())
                        {
                            foreach (SlimNet.Pair<string, object> val in debugValues)
                            {
                                GUILayout.Label((val.Second != null ? val.Second : "<Null>").ToString());
                            }
                        }
                    }

                }
            }
        }
        else
        {
            GUILayout.Label("\"This is not the actor you're looking for.\"", SlimNetGUIStyles.GrayText);
        }
    }
}