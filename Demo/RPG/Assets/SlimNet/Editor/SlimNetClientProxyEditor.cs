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