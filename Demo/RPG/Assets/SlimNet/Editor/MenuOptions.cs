using System.IO;
using UnityEditor;
using UnityEngine;

public class SlimNetMenuOptions : MonoBehaviour
{
    [MenuItem("Assets/Create/SlimNet - Shared Class")]
    public static void CreateSharedClass()
    {
        SlimNetCreateFileDialog.Open((filename) => {
            CreateScriptAsset("SharedClass", filename, (s) => s.Replace("{Class}", filename));
        });
    }

    [MenuItem("Assets/Create/SlimNet - Shared Behaviour")]
    public static void CreateSharedBheaviour()
    {
        SlimNetCreateFileDialog.Open((filename) =>
        {
            CreateScriptAsset("SharedBehaviour", filename, (s) => s.Replace("{Class}", filename));
        });
    }

    [MenuItem("Assets/Create/SlimNet - Shared Actor Event")]
    public static void CreateSharedActorEvent()
    {
        SlimNetCreateFileDialog.Open((filename) =>
        {
            CreateScriptAsset("SharedActorEvent", filename, (s) => s.Replace("{Class}", filename));
        });
    }

    [MenuItem("Assets/Create/SlimNet - Shared Actor Definition")]
    public static void CreateSharedActorDefinition()
    {
        SlimNetCreateFileDialog.Open((filename) =>
        {
            CreateScriptAsset("SharedActorDefinition", filename, (s) => s.Replace("{Class}", filename));
            CreateScriptAsset("SharedActorDefinition.Gen", filename + ".Gen", (s) => s.Replace("{Class}", filename));
        });
    }

    [MenuItem("Assets/Create/SlimNet - Client MonoBehaviour")]
    public static void CreateClientMonoBehaviour()
    {
        SlimNetCreateFileDialog.Open((filename) =>
        {
            CreateScriptAsset("ClientMonoBehaviour", filename, (s) => s.Replace("{Class}", filename));
        });
    }

    static void CreateScriptAsset(string template, string filename, System.Func<string, string> callback)
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (path == "")
        {
            // If empty, set it to root
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            // Remove filename if we have an actual asset and not just a folder selected
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        // Load template file
        TextAsset templateAsset = AssetDatabase.LoadAssetAtPath("Assets/SlimNet/Resources/Templates/" + template + ".txt", typeof(TextAsset)) as TextAsset;

        // If we got an asset, print it
        if (templateAsset != null)
        {
            // Write asset to disk
            SlimNet.Unity.Editor.Utils.WriteTextAsset(path + "/" + filename + ".cs", callback(templateAsset.text));
        }
    }
}