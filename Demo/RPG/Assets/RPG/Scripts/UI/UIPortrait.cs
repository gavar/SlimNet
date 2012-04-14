using SlimNet;
using UnityEngine;

public class UIPortrait : UIBehaviour
{
    SynchronizedValue<string> playerName;

    [SerializeField]
    Texture image;

    protected override void DrawGUI()
    {
        if (GameSettings.PlayerActor != null)
        {
            if (playerName == null)
            {
                playerName = GameSettings.PlayerActor.GetValue<string>("Name");
            }

            GUIStyle nameplateStyle = new GUIStyle("box");
            nameplateStyle.normal.textColor = Color.green;

            GUI.Box(new Rect(10, 10, 128, 25), playerName.Value, nameplateStyle);
            GUI.Box(new Rect(10, 40, 131, 131), "");

            if (image != null)
            {
                GUI.DrawTexture(new Rect(11, 41, 128, 128), image);
            }
        }
    }
}