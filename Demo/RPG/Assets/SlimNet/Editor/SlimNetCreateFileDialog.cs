using UnityEditor;
using UnityEngine;

public class SlimNetCreateFileDialog : EditorWindow
{
    string filename = "";
    System.Action<string> onCreate;

    public static void Open(System.Action<string> onCreate)
    {
        if (onCreate != null)
        {
            SlimNetCreateFileDialog dialog = ScriptableObject.CreateInstance<SlimNetCreateFileDialog>();
            dialog.title = "Create File";
            dialog.maxSize = dialog.minSize = new Vector2(400, 35);
            dialog.onCreate = onCreate;
            dialog.ShowUtility();
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        GUI.SetNextControlName("SlimNetCreateFileDialogInput");
        filename = GUILayout.TextField(filename, GUILayout.Width(310));
        GUI.FocusControl("SlimNetCreateFileDialogInput");

        bool enterPressed = (Event.current.type == EventType.KeyUp) && (Event.current.keyCode == KeyCode.Return);
        if ((GUILayout.Button("Create", GUILayout.Width(75)) || enterPressed) && !string.IsNullOrEmpty(filename))
        {
            onCreate(filename);
            onCreate = null;
        }

        EditorGUILayout.EndHorizontal();

        if (onCreate == null)
        {
            Close();
        }
    }
}