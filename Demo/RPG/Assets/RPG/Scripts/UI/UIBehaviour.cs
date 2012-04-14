using UnityEngine;
using System.Collections;

public class UIBehaviour : MonoBehaviour
{
    void OnGUI()
    {
        // We only wanna show UIs if we're connected
        if (SlimNet.Unity.Client.Instance != null && SlimNet.Unity.Client.Instance.Connected)
        {
            GUI.matrix = UIMatrix;
            DrawGUI();
        }
    }

    public static Matrix4x4 UIMatrix
    {
        get
        {
            return Matrix4x4.TRS(
                Vector3.zero, 
                Quaternion.identity, 
                new Vector3(
                    Screen.width / GameSettings.UIWidth, 
                    Screen.height / GameSettings.UIHeight, 
                    1f
                )
            );
        }
    }

    protected virtual void DrawGUI()
    {

    }
}
